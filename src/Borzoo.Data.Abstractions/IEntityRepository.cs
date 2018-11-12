using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    /// <summary>
    /// A repository for the entities of type <typeparamref name="TEntity"/>
    /// </summary>
    /// <typeparam name="TEntity">Type of entity in the repository</typeparam>
    public interface IEntityRepository<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Adds an entity of type <see cref="TEntity"/> to the repository.
        /// </summary>
        /// <param name="entity">Entity to be added.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task<TEntity> GetByIdAsync(
            string id,
            CancellationToken cancellationToken = default
        );

        Task<TEntity> UpdateAsync(
            TEntity entity,
            CancellationToken cancellationToken = default
        );

        Task DeleteAsync(
            string id,
            CancellationToken cancellationToken = default
        );
    }
}
