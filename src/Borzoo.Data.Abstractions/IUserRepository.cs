using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface IUserRepository : IEntityRepository<User>
    {
        Task<User> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default);

        Task<User> GetByTokenAsync(string token, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default);

        Task<User> GetByPassphraseLoginAsync(string userName, string passphrase, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default);

        Task SetTokenForUserAsync(string userId, string token, CancellationToken cancellationToken = default);

        Task<bool> RevokeTokenAsync(string token, CancellationToken cancellationToken = default);
    }
}