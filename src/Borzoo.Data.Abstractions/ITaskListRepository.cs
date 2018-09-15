using System.Threading;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;

namespace Borzoo.Data.Abstractions
{
    public interface ITaskListRepository : IEntityRepository<TaskList>
    {
        string UserName { get; }

        string UserId { get; }

        Task SetUsernameAsync(string username, CancellationToken cancellationToken = default);
        
        Task<TaskList> GetByNameAsync(string name, bool includeDeletedRecords = false,
            CancellationToken cancellationToken = default);
        
        Task<TaskList[]> GetUserTaskListsAsync(CancellationToken cancellationToken = default);
    }
}