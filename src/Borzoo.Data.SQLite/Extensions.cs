using System;

namespace Borzoo.Data.SQLite
{
    public static class Extensions
    {
        private static readonly DateTime UnixEpochBase = new DateTime(1970, 1, 1);

        public static long ToUnixTime(this DateTime dateTime)
        {
            var diff = dateTime - UnixEpochBase;
            if (diff.Milliseconds < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            return (long) diff.TotalMilliseconds;
        }

        public static long ToUnixTime(this DateTimeOffset dateTimeOffset)
            => dateTimeOffset.UtcDateTime.ToUnixTime();

        public static DateTime FromUnixTime(this long epochTime)
            => new DateTime(UnixEpochBase.Ticks, DateTimeKind.Utc).AddMilliseconds(epochTime);
    }
}