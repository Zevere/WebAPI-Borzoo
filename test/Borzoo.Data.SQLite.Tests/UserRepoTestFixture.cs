using System;
using Borzoo.Data.Abstractions.Entities;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite.Tests
{
    public class UserRepoFixture : IDisposable
    {
        public SqliteConnection Connection { get; }

        public User NewUser { get; set; }

        public UserRepoFixture()
        {
            Connection = Helpers.CreateInMemoryDatabase(nameof(UserRepoFixture));
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}