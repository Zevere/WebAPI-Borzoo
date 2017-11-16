// ReSharper disable once CheckNamespace
namespace Borzoo.Data.Abstractions
{
    public class EntityNotFoundException : RepositoryException
    {
        public EntityNotFoundException(string id)
            : base($"Entity with id of \"{id}\" does not exist.")
        {
        }
    }
}