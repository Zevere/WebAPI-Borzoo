using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite.Tests.Framework
{
    public static class Helpers
    {
        public static SqliteConnection CreateInMemoryDatabase(string uniqueDbName)
        {
            string migrationsSql;
            string envVar = Environment.GetEnvironmentVariable("SQLite_Migrations_Script");
            if (string.IsNullOrWhiteSpace(envVar))
            {
                migrationsSql = Path.Combine(
                    AppContext.BaseDirectory, "..", "..", "..", "..", "..",
                    "src", "Borzoo.Data.SQLite", "scripts", "migrations.sql"
                ); 
            }
            else
            {
                migrationsSql = Path.GetFullPath(envVar);
            }

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