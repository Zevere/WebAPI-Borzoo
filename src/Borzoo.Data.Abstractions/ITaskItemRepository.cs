using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface ITaskItemRepository : IEntityRepository<TaskItem>
    {
        Task<TaskItem> GetByNameAsync(
            string name,
            string username,
            string taskListName,
            CancellationToken cancellationToken = default
        );

        Task<TaskItem[]> GetTaskItemsForListAsync(
            string ownerName,
            string listName,
            CancellationToken cancellationToken = default
        );
    }
}
