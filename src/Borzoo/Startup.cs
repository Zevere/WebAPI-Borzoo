using System.IO;
using Borzoo.Data;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo
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

            // ToDo: Register all with an extension method
            string connectionString = Configuration["sqlite_connection_string"] ?? "borzoo.db";
            services.AddTransient<IEntityRepository<User>>(provider =>
            {
                var repo = new UserRepository(
                    connectionString,
                    Path.GetFullPath("../Borzoo.Data.SQLite/scripts/user.sql")
                );
                repo.InitializeDatabaseAsync().GetAwaiter().GetResult();
                return repo;
            });

            services.AddSingleton<DataSeeder>();

            #endregion

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var seeder = app.ApplicationServices.GetRequiredService<DataSeeder>();
            seeder.Seed().GetAwaiter().GetResult();

            app.UseMvc();
        }
    }
}