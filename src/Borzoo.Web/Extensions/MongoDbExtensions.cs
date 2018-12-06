using System;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Borzoo.Web.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Extensions
{
    internal static class MongoDbExtensions
    {
        /// <summary>
        /// Adds MongoDB services to the app's service collection
        /// </summary>
        public static void AddMongoDb(
            this IServiceCollection services,
            IConfigurationSection dataSection
        )
        {
            string connectionString = dataSection.GetValue<string>(nameof(MongoOptions.ConnectionString));
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException($@"Invalid MongoDB connection string: ""{connectionString}"".");
            }

            services.Configure<MongoOptions>(dataSection);

            string dbName = new ConnectionString(connectionString).DatabaseName;
            services.AddSingleton<IMongoClient, MongoClient>(_ => new MongoClient(connectionString));
            services.AddTransient<IMongoDatabase>(provider =>
                provider.GetRequiredService<IMongoClient>().GetDatabase(dbName)
            );

            services.AddTransient<IMongoCollection<UserEntity>>(_ =>
                _.GetRequiredService<IMongoDatabase>()
                    .GetCollection<UserEntity>("users")
            );
            services.AddTransient<IMongoCollection<TaskList>>(_ =>
                _.GetRequiredService<IMongoDatabase>()
                    .GetCollection<TaskList>("task-lists")
            );
            services.AddTransient<IMongoCollection<TaskItem>>(_ =>
                _.GetRequiredService<IMongoDatabase>()
                    .GetCollection<TaskItem>("task-items")
            );

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ITaskListRepository, TaskListRepository>();
            services.AddTransient<ITaskItemRepository, TaskItemRepository>();

            MongoInitializer.RegisterClassMaps();
        }
    }
}
