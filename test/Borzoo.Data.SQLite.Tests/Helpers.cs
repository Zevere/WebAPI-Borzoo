﻿using System;
using System.IO;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite.Tests
{
    public static class Helpers
    {
        public static SqliteConnection CreateInMemoryDatabase(string uniqueDbName)
        {
            string migrationsSql = Path.Combine(
                AppContext.BaseDirectory, "..", "..", "..", "..", "..",
                "src", "Borzoo.Data.SQLite", "scripts", "migrations.sql"
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