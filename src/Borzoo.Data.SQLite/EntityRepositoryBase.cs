using System;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public abstract class EntityRepositoryBase : IDisposable
    {
        protected SqliteConnection Connection => _connection ??
                                                 (_connection = new SqliteConnection(_connectionString));

        private readonly string _connectionString;

        private SqliteConnection _connection;

        protected EntityRepositoryBase()
        {
        }

        protected EntityRepositoryBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected EntityRepositoryBase(SqliteConnection connection)
        {
            _connection = connection;
        }

        public void EnsureConnectinoOpened()
        {
            Connection.Open();
        }
        
        public void Dispose() => _connection?.Dispose();
    }
}