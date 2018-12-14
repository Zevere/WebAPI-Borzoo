using System;
using GraphQL;

namespace Borzoo.GraphQL
{
    /// <summary>
    /// An execution error in GraphQL operations
    /// </summary>
    public class Error : ExecutionError
    {
        /// <inheritdoc />
        public Error(string message)
            : base(message) { }

        /// <inheritdoc />
        public Error(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
