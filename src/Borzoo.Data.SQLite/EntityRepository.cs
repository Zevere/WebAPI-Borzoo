using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.SQLite
{
    public class EntityRepository<TEntity> : IEntityRepository<TEntity>
        where TEntity : IEntity
    {
    }
}