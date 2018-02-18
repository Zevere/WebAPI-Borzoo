using Borzoo.Data.Abstractions;
using Borzoo.Data.SQLite;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.Data
{
    public static class Extensions
    {
        public static IServiceCollection AddSQLite(this IServiceCollection services, string dbFile)
        {
            string connString = DatabaseInitializer.GetDbFileConnectionString(dbFile);
            DatabaseInitializer.ConnectionString = connString;

            services.AddTransient<IUserRepository, UserRepository>(delegate
            {
                var userRepo = new UserRepository(connString);
                userRepo.EnsureConnectinoOpened();
                return userRepo;
            });
            services.AddTransient<ITaskRepository, TaskRepository>(delegate
            {
                var taskRepo = new TaskRepository(connString);
                taskRepo.EnsureConnectinoOpened();
                return taskRepo;
            });

            services.AddSingleton<DataSeeder>();

            return services;
        }
    }
}