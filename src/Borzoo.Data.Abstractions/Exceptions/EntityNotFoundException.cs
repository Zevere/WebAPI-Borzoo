// ReSharper disable once CheckNamespace

namespace Borzoo.Data.Abstractions
{
    /// <summary>
    /// Indicates an error in finding a unique entity
    /// </summary>
    public class EntityNotFoundException : RepositoryException
    {
        /// <inheritdoc />
        public EntityNotFoundException(string id)
            : base($"Entity with id of \"{id}\" does not exist.") { }

        /// <inheritdoc />
        public EntityNotFoundException(string field, string value)
            : base($"Entity with \"{field}\" of \"{value}\" does not exist.") { }
    }
}
