using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite.Tests.Framework
{
    public static class Helpers
    {
        public static SqliteConnection CreateInMemoryDatabase(string uniqueDbName)
        {
            string envVar = Environment.GetEnvironmentVariable("SQLite_Migrations_Script");
            string migrationsSql = Path.GetFullPath(
                envVar ?? "../../../../../src/Borzoo.Data.SQLite/scripts/migrations.sql"
            );

            return DatabaseInitializer.ConnectAndCreateDatabase(
                GetInMemoryConnectionString(uniqueDbName),
                migrationsSql
            );
        }

        private static string GetInMemoryConnectionString(string uniqueDbName) =>
            "" + new SqliteConnectionStringBuilder
            {
                DataSource = uniqueDbName,
                Mode = SqliteOpenMode.Memory,
                Cache = SqliteCacheMode.Default
            };
    }
}