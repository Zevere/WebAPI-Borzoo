using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using MongoDB.Driver;

namespace Borzoo.Data.Tests.Mongo.Framework
{
    public abstract class FixtureBase<TEntity>
    {
        public IMongoCollection<TEntity> Collection { get; }

        protected readonly IMongoDatabase Database;

        protected FixtureBase(string collectionName, MongoCollectionSettings settings = default)
        {
            Database = Helpers.GetTestDatabase().GetAwaiter().GetResult();
            Initializer.CreateSchemaAsync(Database).GetAwaiter().GetResult();

            Collection = Database.GetCollection<TEntity>(collectionName, settings);
        }

        protected async Task SeedUserDataAsync()
        {
            User[] testUsers =
            {
                new User
                {
                    DisplayId = "alice0",
                    FirstName = "Alice",
                    PassphraseHash = "secret_passphrase"
                },
                new User
                {
                    DisplayId = "bobby",
                    FirstName = "Bob",
                    LastName = "Boo",
                    PassphraseHash = "secret_passphrase2"
                },
            };

            var usersCollection = Database.GetCollection<User>(MongoConstants.Collections.Users.Name);
            var userRepo = new UserRepository(usersCollection);
            foreach (var user in testUsers)
                await userRepo.AddAsync(user);
        }
    }
}