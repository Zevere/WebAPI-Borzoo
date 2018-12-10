using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    /// <summary>
    /// Contains operations to work with a task item collection
    /// </summary>
    public interface ITaskItemRepository : IEntityRepository<TaskItem>
    {
        /// <summary>
        /// Gets a single task list by its name and owner id
        /// </summary>
        /// <param name="name">Display ID of the task</param>
        /// <param name="ownerName">User ID of the list owner</param>
        /// <param name="listName">Display ID of the list</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        Task<TaskItem> GetByNameAsync(
            string name,
            string ownerName,
            string listName,
            CancellationToken cancellationToken = default
        );

        /// <summary>
        /// Gets all the task items in a list
        /// </summary>
        /// <param name="ownerName">User ID of the list owner</param>
        /// <param name="listName">Display ID of the list</param>
        /// <param name="cancellationToken">The cancellation token to cancel operation</param>
        Task<TaskItem[]> GetAllTaskItemsForListAsync(
            string ownerName,
            string listName,
            CancellationToken cancellationToken = default
        );
    }
}
