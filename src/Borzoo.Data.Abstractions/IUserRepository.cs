using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    /// <summary>
    /// A repository for the user entities
    /// </summary>
    public interface IUserRepository : IEntityRepository<User>
    {
        /// <summary>
        /// Gets a user by his unique username
        /// </summary>
        /// <param name="name">Username to search for</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        Task<User> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets a user by his unique authentication token
        /// </summary>
        /// <param name="token">Unique authentication token</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        Task<User> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Sets authentication token for a user
        /// </summary>
        /// <param name="userId">Unique identifier of the user</param>
        /// <param name="token">User's authentication token</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        Task SetTokenForUserAsync(
            string userId,
            string token,
            CancellationToken cancellationToken = default
        );
    }
}
