using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Borzoo.Web.Data;
using Borzoo.Web.Helpers;
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
            #region Database

            string dataStore = Configuration["data:use"];
            if (dataStore == "sqlite")
            {
                string dbFile = Configuration["data:sqlite:db"];
                if (string.IsNullOrWhiteSpace(dbFile)) dbFile = "borzoo.db";
                services.AddSQLite(dbFile);
            }
            else if (dataStore == "mongo")
            {
                string connStr = Configuration["data:mongo:connection"];
                services.AddMongo(connStr);
            }

            #endregion

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

            services.AddGraphQL();

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILogger<Startup> logger)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            logger.LogInformation($@"Using database ""{Configuration["data:use"]}"".");
            if (new[] {"Development", "Staging"}.Contains(env.EnvironmentName))
            {
                app.SeedData(Configuration.GetSection("data"));
            }

            app.UseAuthentication();

            const string GraphQLPath = "/graphql";
            app.UseGraphQL(new GraphQLSettings {Path = GraphQLPath});
            app.UseGraphiQl(GraphQLPath);

            app.UseMvc();

            app.Run(context =>
            {
                context.Response.Redirect("https://github.com/Zevere");
                return Task.CompletedTask;
            });
        }
    }
}