// ReSharper disable once CheckNamespace

namespace Borzoo.Data.Abstractions
{
    public class DuplicateKeyException : RepositoryException
    {
        public string Key { get; }

        public DuplicateKeyException(string key)
            : base($@"Duplicate key: ""{key}""")
        {
            Key = key;
        }
    }
}