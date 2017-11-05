using System;
using System.Threading;
using System.Threading.Tasks;
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

        public static async Task<string> GetLastInsertedRowIdAsync(this SqliteConnection connection,
            CancellationToken cancellationToken = default)
        {
            var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT last_insert_rowid() AS id";
            return (await cmd.ExecuteScalarAsync(cancellationToken)).ToString();
        }
    }
}