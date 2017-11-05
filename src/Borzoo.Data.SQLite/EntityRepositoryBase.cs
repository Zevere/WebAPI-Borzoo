using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public abstract class EntityRepositoryBase
    {
        private readonly string _connectionString;

        private readonly string _creationScriptFile;

        protected EntityRepositoryBase(string connectionString, string creationScriptFile)
        {
            _connectionString = connectionString;
            _creationScriptFile = creationScriptFile;
        }

        public async Task InitializeDatabaseAsync(CancellationToken cancellationToken = default)
        {
            string sql = File.ReadAllText(_creationScriptFile);

            using (var conn = CreateConnection())
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = sql;

                await conn.OpenAsync(cancellationToken);
                await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        protected SqliteConnection CreateConnection() => new SqliteConnection(
            "" + new SqliteConnectionStringBuilder {DataSource = _connectionString}
        );
    }
}