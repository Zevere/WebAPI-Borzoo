using System.Threading.Tasks;
using MongoDB.Driver;

namespace Borzoo.Data.Mongo.Tests.Framework
{
    public static class Helpers
    {
        static Helpers()
        {
            Initializer.RegisterClassMaps();
        }

        public static async Task<IMongoDatabase> GetTestDatabase()
        {
            var client = new MongoClient();
            await client.DropDatabaseAsync(MongoConstants.Database.Test);
            var db = client.GetDatabase(MongoConstants.Database.Test);
            return db;
        }
    }
}