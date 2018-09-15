using System;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.Tests.SQLite.Framework
{
    public abstract class FixtureBase : IDisposable
    {
        public SqliteConnection Connection { get; }

        protected FixtureBase(string databaseName)
        {
            Connection = Helpers.CreateInMemoryDatabase(databaseName);
        }

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}