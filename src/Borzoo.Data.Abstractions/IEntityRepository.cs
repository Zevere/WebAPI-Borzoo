using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface IEntityRepository<TEntity>
        where TEntity : IEntity
    {
        Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task<TEntity> GetByIdAsync(string id, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default);

        Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);

        Task DeleteAsync(string id, bool hardDelete = false, CancellationToken cancellationToken = default);
    }
}