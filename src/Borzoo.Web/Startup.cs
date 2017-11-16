using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite;
using Borzoo.Web.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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

            string dbPath = Configuration["SQLite_Connection_String"] ?? "borzoo.db";
            string connString = DatabaseInitializer.GetDbFileConnectionString(dbPath);
            services.AddTransient<IEntityRepository<User>, UserRepository>(delegate
            {
                var userRepo = new UserRepository(connString);
                userRepo.EnsureConnectinoOpened();
                return userRepo;
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