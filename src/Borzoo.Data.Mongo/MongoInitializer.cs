using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    /// <summary>
    /// MongoDB initialization helper
    /// </summary>
    public static class MongoInitializer
    {
        /// <summary>
        /// Creates the database schema
        /// </summary>
        /// <param name="database">Database instance</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        public static async Task CreateSchemaAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken = default
        )
        {
            {
                // "users" Collection
                await database.CreateCollectionAsync("users", cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var usersCollection = database.GetCollection<User>("users");

                // create unique index "username" on the field "name"
                await usersCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<User>(
                        Builders<User>.IndexKeys.Ascending(u => u.DisplayId),
                        new CreateIndexOptions { Name = "username", Unique = true }
                    ), cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }

            {
                // "task-lists" Collection
                await database.CreateCollectionAsync("task-lists", cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var listsCollection = database.GetCollection<TaskList>("task-lists");
                var indexBuilder = Builders<TaskList>.IndexKeys;

                // create unique index "owner_list-name" on the fields "name" and "owner"
                var key = indexBuilder.Combine(
                    indexBuilder.Ascending(tl => tl.OwnerId),
                    indexBuilder.Ascending(tl => tl.DisplayId)
                );
                await listsCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<TaskList>(
                        key, new CreateIndexOptions { Name = "owner_list-name", Unique = true }
                    ), cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }

            {
                // "task-items" Collection
                await database.CreateCollectionAsync("task-items", cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
                var tasksCollection = database.GetCollection<TaskItem>("task-items");
                var indexBuilder = Builders<TaskItem>.IndexKeys;

                // create unique index "owner_list_task-name" on the fields "name", "list" and "owner"
                var key = indexBuilder.Combine(
                    indexBuilder.Ascending(ti => ti.OwnerId),
                    indexBuilder.Ascending(ti => ti.ListId),
                    indexBuilder.Ascending(tl => tl.DisplayId)
                );
                await tasksCollection.Indexes.CreateOneAsync(
                    new CreateIndexModel<TaskItem>(
                        key, new CreateIndexOptions { Name = "owner_list_task-name", Unique = true }
                    ), cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Registers all the mappings between data entities and the documents stored in MongoDB collections
        /// </summary>
        public static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(map =>
                {
                    map.MapIdProperty(u => u.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    map.MapProperty(u => u.DisplayId).SetElementName("name").SetOrder(1).SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.PassphraseHash).SetElementName("pass").SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.FirstName).SetElementName("fname").SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.JoinedAt).SetElementName("joined").SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.Token).SetElementName("token").SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.LastName).SetElementName("lname").SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.ModifiedAt).SetElementName("modified").SetIgnoreIfDefault(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TaskList)))
            {
                BsonClassMap.RegisterClassMap<TaskList>(map =>
                {
                    map.MapIdProperty(tl => tl.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    map.MapProperty(tl => tl.DisplayId).SetElementName("name");
                    map.MapProperty(tl => tl.OwnerId).SetElementName("owner");
                    map.MapProperty(tl => tl.Title).SetElementName("title");
                    map.MapProperty(tl => tl.CreatedAt).SetElementName("created");
                    map.MapProperty(tl => tl.Description).SetElementName("description").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.Collaborators).SetElementName("collaborators").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.ModifiedAt).SetElementName("modified").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.Tags).SetElementName("tags").SetIgnoreIfDefault(true);
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TaskItem)))
            {
                BsonClassMap.RegisterClassMap<TaskItem>(map =>
                {
                    map.MapIdProperty(ti => ti.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    map.MapProperty(ti => ti.DisplayId).SetElementName("name").SetOrder(1);
                    map.MapProperty(ti => ti.Title).SetElementName("title");
                    map.MapProperty(ti => ti.OwnerId).SetElementName("owner");
                    map.MapProperty(ti => ti.ListId).SetElementName("list");
                    map.MapProperty(ti => ti.Description).SetElementName("description").SetIgnoreIfDefault(true);
                    map.MapProperty(ti => ti.Due).SetElementName("due").SetIgnoreIfDefault(true);
                    map.MapProperty(ti => ti.Tags).SetElementName("tags").SetIgnoreIfDefault(true);
                    map.MapProperty(ti => ti.ModifiedAt).SetElementName("modified").SetIgnoreIfDefault(true);
                    map.MapProperty(ti => ti.CreatedAt).SetElementName("created");
                });
            }
        }
    }
}
