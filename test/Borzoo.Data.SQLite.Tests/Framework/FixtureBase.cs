using System;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite.Tests.Framework
{
    public abstract class FixtureBase : IDisposable
    {
        public SqliteConnection Connection { get; }

        protected FixtureBase(string collectionName)
        {
            Connection = Helpers.CreateInMemoryDatabase(collectionName);
        }

        public void Dispose()
        {
            Connection.Dispose();
        }

        protected void SeedUserData()
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

            var userRepo = new UserRepository(Connection);
            foreach (var user in testUsers)
                userRepo.AddAsync(user).GetAwaiter().GetResult();
        }
    }
}