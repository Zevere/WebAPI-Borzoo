using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface IEntityRepository<TEntity>
        where TEntity : IEntity
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    }
}