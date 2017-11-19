using System.Linq;
using Borzoo.Data.Abstractions;
using Borzoo.Data.SQLite;
using Borzoo.Web.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Borzoo.Web
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region SQLite

            string dbPath = Configuration["SQLite_Db_Path"];
            if (string.IsNullOrWhiteSpace(dbPath))
            {
                dbPath = "borzoo.db";
            }
            string connString = DatabaseInitializer.GetDbFileConnectionString(dbPath);
            DatabaseInitializer.ConnectionString = connString;

            services.AddTransient<IUserRepository, UserRepository>(delegate
            {
                var userRepo = new UserRepository(connString);
                userRepo.EnsureConnectinoOpened();
                return userRepo;
            });

            services.AddSingleton<DataSeeder>();

            #endregion

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger,
            DataSeeder seeder)
        {          
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            if (new[] {"Development", "Staging"}.Contains(env.EnvironmentName))
            {
                logger.LogInformation("SQLite Connection String: " + DatabaseInitializer.ConnectionString);
                seeder.SeedData(Configuration["SQLite_Migrations_Script"]);
            }

            app.UseMvc();
        }
    }
}