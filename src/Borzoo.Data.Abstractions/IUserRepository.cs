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
        /// Get a user by his unique username.
        /// </summary>
        /// <param name="name">Username to search for.</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        Task<User> GetByNameAsync(
            string name,
            CancellationToken cancellationToken = default
        );

        Task<User> GetByTokenAsync(
            string token,
            CancellationToken cancellationToken = default
        );

        Task<User> GetByPassphraseLoginAsync(
            string userName,
            string passphrase,
            CancellationToken cancellationToken = default
        );

        Task SetTokenForUserAsync(
            string userId,
            string token,
            CancellationToken cancellationToken = default
        );

        Task<bool> RevokeTokenAsync(
            string token,
            CancellationToken cancellationToken = default
        );
    }
}
