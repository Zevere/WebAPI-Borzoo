using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo.Entities;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo
{
    public static class Initializer
    {
        public static async Task CreateSchemaAsync(IMongoDatabase database,
            CancellationToken cancellationToken = default)
        {
            {
                // "users" Collection
                await database.CreateCollectionAsync(MongoConstants.Collections.Users.Name, null, cancellationToken);
                var usersCollection = database.GetCollection<User>(MongoConstants.Collections.Users.Name);
                var key = Builders<User>.IndexKeys.Ascending(u => u.DisplayId);
                await usersCollection.Indexes.CreateOneAsync(key,
                    new CreateIndexOptions {Name = MongoConstants.Collections.Users.Indexes.Username, Unique = true},
                    cancellationToken
                );
            }

            {
                // "task-lists" Collection
                await database.CreateCollectionAsync(MongoConstants.Collections.TaskLists.Name, null,
                    cancellationToken);
                var collection = database.GetCollection<TaskListMongo>(MongoConstants.Collections.TaskLists.Name);
                var indexBuilder = Builders<TaskListMongo>.IndexKeys;
                var key = indexBuilder.Combine(
                    indexBuilder.Ascending(tl => tl.OwnerDbRef.Id),
                    indexBuilder.Ascending(tl => tl.DisplayId)
                );
                await collection.Indexes.CreateOneAsync(key,
                    new CreateIndexOptions
                    {
                        Name = MongoConstants.Collections.TaskLists.Indexes.OwnerListName,
                        Unique = true
                    },
                    cancellationToken
                );
            }
        }

        public static void RegisterClassMaps()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                BsonClassMap.RegisterClassMap<User>(map =>
                {
                    map.MapIdProperty(u => u.Id).SetIdGenerator(new StringObjectIdGenerator());
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
                    map.MapIdProperty(tl => tl.Id).SetIdGenerator(new StringObjectIdGenerator());
                    map.MapProperty(tl => tl.DisplayId).SetElementName("name").SetOrder(1);
                    map.MapProperty(tl => tl.Title).SetElementName("title");
                    map.MapProperty(tl => tl.CreatedAt).SetElementName("created");
                });
                BsonClassMap.RegisterClassMap<TaskListMongo>(map =>
                {
                    map.MapProperty(tl => tl.OwnerDbRef)
                        .SetIsRequired(true)
                        .SetElementName("owner");
                });
            }
        }
    }
}