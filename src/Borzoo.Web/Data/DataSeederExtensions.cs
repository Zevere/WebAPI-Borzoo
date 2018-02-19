using System.Linq;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Borzoo.Data.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Borzoo.Web.Data
{
    internal static class DataSeederExtensions
    {
        public static IApplicationBuilder SeedData(this IApplicationBuilder app, IConfigurationSection dataConfig)
        {
            string dataStore = dataConfig["use"];
            using (var _ = app.ApplicationServices.CreateScope())
            {
                var logger = _.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                if (dataStore == "sqlite")
                {
                    logger.LogInformation("Applying SQLite migrations...");
                    ApplySQLiteMigrations(dataConfig["sqlite:migrations"]);
                }
                else if (dataStore == "mongo")
                {
                    var db = _.ServiceProvider.GetRequiredService<IMongoDatabase>();
                    bool dbInitialized = InitMongoDbAsync(db).GetAwaiter().GetResult();
                    if (dbInitialized)
                        logger.LogInformation("Mongo database is initialized.");
                }

                var userRepo = _.ServiceProvider.GetRequiredService<IUserRepository>();
                bool seeded = SeedData(userRepo).GetAwaiter().GetResult();
                logger.LogInformation($"Database is{(seeded ? "" : " NOT")} seeded.");
            }

            return app;
        }

        private static async Task<bool> SeedData(IUserRepository userRepo)
        {
            if (await IsAlreadySeeded(userRepo))
            {
                return false;
            }

            User[] testUsers =
            {
                new User
                {
                    DisplayId = "alice0",
                    FirstName = "Alice",
                    PassphraseHash = "secret_passphrase"
                },
                new User
                {
                    DisplayId = "bobby",
                    FirstName = "Bob",
                    LastName = "Boo",
                    PassphraseHash = "secret_passphrase2"
                },
            };

            foreach (var user in testUsers)
            {
                await userRepo.AddAsync(user);
            }

            return true;
        }

        private static async Task<bool> IsAlreadySeeded(IUserRepository userRepo)
        {
            bool userExists;
            try
            {
                await userRepo.GetByNameAsync("alICE0");
                userExists = true;
            }
            catch (EntityNotFoundException)
            {
                userExists = false;
            }

            return userExists;
        }

        private static void ApplySQLiteMigrations(string migrationsSqlFile)
        {
            DatabaseInitializer.EnsureMigrationsApplied(migrationsSqlFile);
        }
        
        private static async Task<bool> InitMongoDbAsync(IMongoDatabase db)
        {
            var curser = await db.ListCollectionsAsync();
            if (curser.MoveNext() && curser.Current.Any())
            {
                return false;
            }

            await Initializer.CreateSchemaAsync(db);
            return true;
        }
    }
}