using System;
using Microsoft.Data.Sqlite;

namespace Borzoo.Data.SQLite
{
    public static class Extensions
    {
        private static readonly DateTime UnixEpochBase = new DateTime(1970, 1, 1);

        public static long ToUnixEpoch(this DateTime dateTime)
        {
            var diff = dateTime - UnixEpochBase;
            if (diff.Milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (long) diff.TotalMilliseconds;
        }

        public static string GetLastInsertedRowId(this SqliteConnection connection,
            SqliteTransaction transaction = default)
        {
            var cmd = connection.CreateCommand();
            if (transaction != null)
            {
                cmd.Transaction = transaction;
            }
            cmd.CommandText = "SELECT last_insert_rowid() AS id";
            return cmd.ExecuteScalar().ToString();
        }
    }
}