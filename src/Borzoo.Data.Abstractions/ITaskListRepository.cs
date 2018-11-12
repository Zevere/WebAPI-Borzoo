using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface ITaskListRepository : IEntityRepository<TaskList>
    {
        Task SetUsernameAsync(
            string username,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets a single task list by its name and owner id
        /// </summary>
        /// <param name="name">Display ID of the task list</param>
        /// <param name="ownerId">User ID of the list owner</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        Task<TaskList> GetByNameAsync(
            string name,
            string ownerId,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets all the task lists for a user
        /// </summary>
        /// <param name="ownerId">User ID of the list owner</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation.</param>
        Task<TaskList[]> GetUserTaskListsAsync(
            string ownerId,
            CancellationToken cancellationToken = default
        );
    }
}
