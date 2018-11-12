using System;
using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Framework
{
    public class NullableDateTimeEqualityComparer : IEqualityComparer<DateTime?>
    {
        private readonly int _range;

        public NullableDateTimeEqualityComparer(int range = 100_000)
        {
            _range = range;
        }

        public bool Equals(DateTime? x, DateTime? y) =>
            (!x.HasValue && !y.HasValue) || Math.Abs(x.Value.Ticks - y.Value.Ticks) <= _range;

        public int GetHashCode(DateTime? obj) => obj.GetHashCode();
    }
}
