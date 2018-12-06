using System;
using System.Runtime.CompilerServices;
using Xunit;

namespace Framework
{
    public sealed class OrderedTheoryAttribute : TheoryAttribute
    {
        public int LineNumber { get; }

        public OrderedTheoryAttribute(
            [CallerLineNumber] int line = default
        )
        {
            if (line < 1)
                throw new ArgumentOutOfRangeException(nameof(line));

            LineNumber = line;
        }

        public OrderedTheoryAttribute(
            string displayName,
            [CallerLineNumber] int line = default
        )
        {
            if (line < 1)
                throw new ArgumentOutOfRangeException(nameof(line));

            LineNumber = line;
            DisplayName = displayName;
        }
    }
}
