﻿using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Borzoo.Web.Data
{
    internal static class DataSeederExtensions
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var _ = app.ApplicationServices.CreateScope())
            {
                var logger = _.ServiceProvider.GetRequiredService<ILogger<Startup>>();

                var db = _.ServiceProvider.GetRequiredService<IMongoDatabase>();
                bool dbInitialized = InitMongoDbAsync(db).GetAwaiter().GetResult();
                if (dbInitialized)
                    logger.LogInformation("Mongo database is initialized.");

                var userRepo = _.ServiceProvider.GetRequiredService<IUserRepository>();
                bool seeded = SeedData(userRepo).GetAwaiter().GetResult();
                logger.LogInformation($"Database is{(seeded ? "" : " NOT")} seeded.");
            }
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

        private static async Task<bool> InitMongoDbAsync(IMongoDatabase db)
        {
            var cursor = await db.ListCollectionNamesAsync();
            var collections = await cursor.ToListAsync();

            bool collectionsExist = collections.Count > 2;

            if (!collectionsExist)
            {
                await MongoInitializer.CreateSchemaAsync(db);
            }

            return !collectionsExist;
        }
    }
}
