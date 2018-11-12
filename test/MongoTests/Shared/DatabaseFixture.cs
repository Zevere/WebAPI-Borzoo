using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Borzoo.Data.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace MongoTests.Shared
{
    public class DatabaseFixture
    {
        public IMongoDatabase Database { get; }

        /// <summary>
        /// Registers mapping from entity class types to MongoDB schema
        /// </summary>
        static DatabaseFixture()
        {
            MongoInitializer.RegisterClassMaps();
        }

        public DatabaseFixture()
        {
            Database = InitializeDatabase().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Initializes the MongoDB database. Recreates the database, if exists, and creates its schema
        /// </summary>
        private static async Task<IMongoDatabase> InitializeDatabase()
        {
            var settings = new Settings();
            var connectionString = new ConnectionString(settings.ConnectionString);

            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            clientSettings.ClusterConfigurator = ClientSettingsClusterConfigurator;

            var client = new MongoClient(clientSettings);

            await client.DropDatabaseAsync(connectionString.DatabaseName);
            var db = client.GetDatabase(connectionString.DatabaseName);

            await MongoInitializer.CreateSchemaAsync(db);

            return db;
        }

        /// <summary>
        /// Configures a MongoDB client to write its logs to the standard output
        /// </summary>
        private static void ClientSettingsClusterConfigurator(ClusterBuilder cb)
        {
            var traceSource = new TraceSource("~~ MongoDB ~~", SourceLevels.Warning);
            traceSource.Listeners.Clear();
            var listener = new TextWriterTraceListener(Console.Out) {TraceOutputOptions = TraceOptions.DateTime,};
            traceSource.Listeners.Add(listener);
            cb.TraceWith(traceSource);
        }
    }
}
