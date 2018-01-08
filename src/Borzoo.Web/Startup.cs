using System.Linq;
using System.Security.Claims;
using Borzoo.Data.Abstractions;
using Borzoo.Data.SQLite;
using Borzoo.Web.Data;
using Borzoo.Web.Middlewares.BasicAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Borzoo.Web
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            #region Auth

            services.AddAuthentication("Basic")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("Basic", default);

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicy(
                    new[]
                    {
                        new AssertionRequirement(authContext => authContext.User.FindFirstValue("token") != default)
                    },
                    new[] {"Basic"});
            });

            #endregion

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

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}