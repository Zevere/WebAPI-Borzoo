using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Borzoo.Data.SQLite;
using Borzoo.Web.Data;
using Borzoo.Web.GraphQL;
using Borzoo.Web.Middlewares.BasicAuth;
using Borzoo.Web.Middlewares.GraphQL;
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
                dbPath = "borzoo.db";

            services.AddSQLite(dbPath);

            #endregion

            services.AddGraphQL();

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

            app.UseMiddleware<GraphQLMiddleware>(new GraphQLSettings { Path = "/graphql" });
            
            app.UseGraphiQl("/graphql");
            
            app.UseMvc();
            
            app.Run(context =>
            {
                context.Response.Redirect("https://github.com/Zevere");
                return Task.CompletedTask;
            });
        }
    }
}