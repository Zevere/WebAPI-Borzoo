using Borzoo.Data.Abstractions;
using Borzoo.Data.SQLite;
using Microsoft.Extensions.DependencyInjection;

namespace Borzoo.Web.Helpers
{
    public static class SQLiteExtensions
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
            services.AddTransient<ITaskListRepository, TaskListRepository>(provider =>
            {
                var userRepo = provider.GetRequiredService<IUserRepository>();
                var taskListRepo = new TaskListRepository(connString, userRepo);
                taskListRepo.EnsureConnectinoOpened();
                return taskListRepo;
            });
            services.AddTransient<ITaskItemRepository, TaskItemRepository>(provider =>
            {
                var tasklistRepo = provider.GetRequiredService<ITaskListRepository>();
                var taskRepo = new TaskItemRepository(connString, tasklistRepo);
                taskRepo.EnsureConnectinoOpened();
                return taskRepo;
            });

            return services;
        }
    }
}