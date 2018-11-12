using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    /// <summary>
    /// A repository for the <see cref="User"/> entities
    /// </summary>
    public interface IUserRepository : IEntityRepository<User>
    {
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
