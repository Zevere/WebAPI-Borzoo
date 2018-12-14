using System;
using Borzoo.Web.Data;
using Borzoo.Web.Middlewares.BasicAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
using Borzoo.Web.Extensions;
using Microsoft.AspNetCore.Http;

namespace Borzoo.Web
{
    /// <summary>
    /// Application startup
    /// </summary>
    public class Startup
    {
        private IConfiguration Configuration { get; }

        /// <inheritdoc />
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Configures application services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMongoDb(Configuration.GetSection("Mongo"));

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
                    new[] { "Basic" });
            });

            #endregion

            services.AddGraphQl();

            services.AddCors();
        }

        /// <summary>
        /// Configures web application request pipeline
        /// </summary>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.SeedData();
            }

            app.UseCors(cors => cors
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetPreflightMaxAge(TimeSpan.FromDays(7))
            );

            app.UseAuthentication();

            app.UseGraphQl("/zv/GraphQL");
            app.UseGraphiql("/zv/GraphiQL", opts => { opts.GraphQlEndpoint = "/zv/GraphQL"; });

            app.Run(context => context.Response.WriteAsync("Hello, World! Welcome to Borzoo ;)"));
        }
    }
}
