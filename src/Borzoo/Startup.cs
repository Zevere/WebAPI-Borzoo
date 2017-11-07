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

            DatabaseInitializer.ConnectionString = Configuration["SQLite_Connection_String"] ?? "borzoo.db";
            services.AddTransient<IEntityRepository<User>, UserRepository>();

            services.AddSingleton<DataSeeder>();

            #endregion

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                DatabaseInitializer.InitDatabase(Configuration["SQLite_Migrations_Script"]);
                app.UseDeveloperExceptionPage();
            }

//            var seeder = app.ApplicationServices.GetRequiredService<DataSeeder>();
//            seeder.Seed().GetAwaiter().GetResult();

            app.UseMvc();
        }
    }
}