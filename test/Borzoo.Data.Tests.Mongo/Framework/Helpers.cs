using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Borzoo.Data.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;

namespace Borzoo.Data.Tests.Mongo.Framework
{
    public static class Helpers
    {
        static Helpers()
        {
            Initializer.RegisterClassMaps();
        }

        public static async Task<IMongoDatabase> GetTestDatabase()
        {
            MongoClient client;
            string connStr = Environment.GetEnvironmentVariable("MONGO_CONNECTION");
            if (connStr != null)
            {
                client = new MongoClient(connStr);
            }
            else
            {
                client = new MongoClient(new MongoClientSettings
                {
                    ClusterConfigurator = cb =>
                    {
                        var traceSource = new TraceSource("mongodb-tests", SourceLevels.Warning);
                        traceSource.Listeners.Clear();
                        var listener = new TextWriterTraceListener(Console.Out)
                        {
                            TraceOutputOptions = TraceOptions.DateTime,
                        };
                        traceSource.Listeners.Add(listener);
                        cb.TraceWith(traceSource);
                    }
                });
            }

            await client.DropDatabaseAsync(MongoConstants.Database.Test);
            var db = client.GetDatabase(MongoConstants.Database.Test);
            return db;
        }
    }
}