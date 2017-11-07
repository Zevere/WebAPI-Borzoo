using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public abstract class EntityRepositoryBase
    {
        protected SqliteConnection CreateConnection() => DatabaseInitializer.CreateConnection();
    }
}