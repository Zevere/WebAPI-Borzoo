using System.Collections.Generic;

// ReSharper disable once CheckNamespace
namespace Borzoo.Data.Abstractions
{
    /// <summary>
    /// Indicates an unique index error
    /// </summary>
    public class DuplicateKeyException : RepositoryException
    {
        /// <summary>
        /// Name of the fields violating the index
        /// </summary>
        public IEnumerable<string> Keys { get; }

        /// <inheritdoc />
        public DuplicateKeyException(params string[] keys)
            : base(string.Format(@"Duplicate key{0}: ""{1}""",
                keys.Length > 1 ? "s" : string.Empty,
                string.Join(", ", keys)
            ))
        {
            Keys = keys;
        }
    }
}
