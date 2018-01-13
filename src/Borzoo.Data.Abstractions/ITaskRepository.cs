using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface ITaskRepository : IEntityRepository<UserTask>
    {
        string UserName { get; set; }

        string UserId { get; }

        Task<UserTask[]> GetUserTasksAsync(bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default);
    }
}