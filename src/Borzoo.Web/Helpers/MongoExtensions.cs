using Borzoo.Data.Abstractions;
using Borzoo.Data.Mongo;
using Borzoo.Data.Mongo.Entities;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Helpers
{
    internal static class MongoExtensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services, string connectionString)
        {
            string dbName = new ConnectionString(connectionString).DatabaseName;
            services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connectionString));
            services.AddTransient<IMongoDatabase>(_ => _.GetRequiredService<IMongoClient>().GetDatabase(dbName));

            services.AddTransient<IMongoCollection<UserEntity>>(_ =>
                _.GetRequiredService<IMongoDatabase>()
                    .GetCollection<UserEntity>(MongoConstants.Collections.Users.Name)
            );
            services.AddTransient<IMongoCollection<TaskListMongo>>(_ =>
                _.GetRequiredService<IMongoDatabase>()
                    .GetCollection<TaskListMongo>(MongoConstants.Collections.TaskLists.Name)
            );
            services.AddTransient<IMongoCollection<TaskItemMongo>>(_ =>
                _.GetRequiredService<IMongoDatabase>()
                    .GetCollection<TaskItemMongo>(MongoConstants.Collections.TaskItems.Name)
            );
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITaskListRepository, TaskListRepository>();
            services.AddTransient<ITaskItemRepository, TaskItemRepository>();
            
            Initializer.RegisterClassMaps();
            
            return services;
        }
    }
}