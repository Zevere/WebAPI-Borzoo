using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public static class Initializer
    {
        public static async Task CreateSchemaAsync(
            IMongoDatabase database,
            CancellationToken cancellationToken = default
        )
        {
            {
                // "users" Collection
                await database.CreateCollectionAsync(MongoConstants.Collections.Users.Name, null, cancellationToken);
                var usersCollection = database.GetCollection<User>(MongoConstants.Collections.Users.Name);

                // create unique index "username" on the field "name"
                var key = Builders<User>.IndexKeys.Ascending(u => u.DisplayId);
                await usersCollection.Indexes.CreateOneAsync(new CreateIndexModel<User>(
                        key,
                        new CreateIndexOptions
                            {Name = MongoConstants.Collections.Users.Indexes.Username, Unique = true}),
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }

            {
                // "task-lists" Collection
                await database.CreateCollectionAsync(MongoConstants.Collections.TaskLists.Name, null,
                    cancellationToken);
                var listsCollection = database.GetCollection<TaskListMongo>(MongoConstants.Collections.TaskLists.Name);
                var indexBuilder = Builders<TaskListMongo>.IndexKeys;

                // create unique index "owner-list_name" on the fields "name" and "owner"
                var key = indexBuilder.Combine(
                    indexBuilder.Ascending(tl => tl.OwnerDbRef.Id),
                    indexBuilder.Ascending(tl => tl.DisplayId)
                );
                await listsCollection.Indexes.CreateOneAsync(new CreateIndexModel<TaskListMongo>(
                        key,
                        new CreateIndexOptions
                            {Name = MongoConstants.Collections.TaskLists.Indexes.OwnerListName, Unique = true}),
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }

            {
                // "task-items" Collection
                await database.CreateCollectionAsync(MongoConstants.Collections.TaskItems.Name, null,
                    cancellationToken);
                var tasksCollection = database.GetCollection<TaskItemMongo>(MongoConstants.Collections.TaskItems.Name);
                var indexBuilder = Builders<TaskItemMongo>.IndexKeys;

                // create unique index "list-task_name" on the fields "name" and "list"
                var key = indexBuilder.Combine(
                    indexBuilder.Ascending(tl => tl.ListDbRef.Id),
                    indexBuilder.Ascending(tl => tl.DisplayId)
                );
                await tasksCollection.Indexes.CreateOneAsync(new CreateIndexModel<TaskItemMongo>(
                        key,
                        new CreateIndexOptions
                            {Name = MongoConstants.Collections.TaskItems.Indexes.ListTaskName, Unique = true}),
                    cancellationToken: cancellationToken
                ).ConfigureAwait(false);
            }
        }

        public static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(map =>
                {
                    map.MapIdProperty(u => u.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    map.MapProperty(u => u.DisplayId).SetElementName("name").SetOrder(1);
                    map.MapProperty(u => u.PassphraseHash).SetElementName("pass");
                    map.MapProperty(u => u.FirstName).SetElementName("fname");
                    map.MapProperty(u => u.JoinedAt).SetElementName("joined");
                    map.MapProperty(u => u.Token).SetElementName("token");
                    map.MapProperty(u => u.LastName).SetElementName("lname").SetIgnoreIfDefault(true);
                    map.MapProperty(u => u.IsDeleted).SetElementName("deleted").SetIgnoreIfDefault(true);
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
                    map.MapProperty(tl => tl.DisplayId).SetElementName("name").SetOrder(1);
                    map.MapProperty(tl => tl.Title).SetElementName("title");
                    map.MapProperty(tl => tl.CreatedAt).SetElementName("created");
                    map.MapProperty(tl => tl.IsDeleted).SetElementName("deleted").SetIgnoreIfDefault(true);
                });
                BsonClassMap.RegisterClassMap<TaskListMongo>(map =>
                {
                    map.MapProperty(tl => tl.OwnerDbRef)
                        .SetIsRequired(true)
                        .SetElementName("owner");
                });
            }

            if (!BsonClassMap.IsClassMapRegistered(typeof(TaskItem)))
            {
                BsonClassMap.RegisterClassMap<TaskItem>(map =>
                {
                    map.MapIdProperty(tl => tl.Id)
                        .SetIdGenerator(StringObjectIdGenerator.Instance)
                        .SetSerializer(new StringSerializer(BsonType.ObjectId));
                    map.MapProperty(tl => tl.DisplayId).SetElementName("name").SetOrder(1);
                    map.MapProperty(tl => tl.Title).SetElementName("title");
                    map.MapProperty(tl => tl.Description).SetElementName("description").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.Due).SetElementName("due").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.Tags).SetElementName("tags").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.ModifiedAt).SetElementName("modified").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.IsDeleted).SetElementName("deleted").SetIgnoreIfDefault(true);
                    map.MapProperty(tl => tl.CreatedAt).SetElementName("created");
                });
                BsonClassMap.RegisterClassMap<TaskItemMongo>(map =>
                {
                    map.MapProperty(tl => tl.ListDbRef)
                        .SetIsRequired(true)
                        .SetElementName("list");
                });
            }
        }
    }
}