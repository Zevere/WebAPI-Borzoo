using System.Threading.Tasks;
using Borzoo.Data.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Borzoo.Web.Tests.Integ.Framework
{
    public class DataInitializer
    {
        private static bool _isMongoSeeded;

        public static async Task InitMongoDb(string connectionString)
        {
            if (_isMongoSeeded) return;
            _isMongoSeeded = true;

            string dbName = new ConnectionString(connectionString).DatabaseName;
            
            var mongoClient = new MongoClient(connectionString);
            await mongoClient.DropDatabaseAsync(dbName);
            await Initializer.CreateSchemaAsync(mongoClient.GetDatabase(dbName));
        }
    }
}