using System;
using System.Net.Http;
using Borzoo.Data.Mongo;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace WebAppTests.Shared
{
    public class TestsFixture
    {
        public HttpClient HttpClient { get; }

        public IServiceProvider Services => WebAppFactory.Server.Host.Services;

        public WebAppFactory WebAppFactory { get; }

        public TestsFixture()
        {
            WebAppFactory = new WebAppFactory();
            HttpClient = WebAppFactory.CreateClient();

            EnsureEmptyDatabase();
            InitializeDatabase();
        }

        private void EnsureEmptyDatabase()
        {
            var db = Services.GetRequiredService<IMongoDatabase>();

            var collections = db.ListCollectionNames().ToList();

            foreach (var collection in collections)
            {
                db.DropCollection(collection);
            }
        }

        private void InitializeDatabase()
        {
            var db = Services.GetRequiredService<IMongoDatabase>();

            MongoInitializer.RegisterClassMaps();
            MongoInitializer.CreateSchemaAsync(db).GetAwaiter().GetResult();
        }
    }
}
