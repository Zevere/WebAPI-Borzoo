using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;
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
            await database.CreateCollectionAsync(MongoConstants.Collections.Users.Name, default, cancellationToken);
            var usersCollection = database.GetCollection<User>(MongoConstants.Collections.Users.Name);
            var key = Builders<User>.IndexKeys.Ascending("name");
            await usersCollection.Indexes.CreateOneAsync(key,
                new CreateIndexOptions {Name = MongoConstants.Collections.Users.Indexes.Username, Unique = true},
                cancellationToken
            );
        }

        public static void RegisterClassMaps()
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
    }
}