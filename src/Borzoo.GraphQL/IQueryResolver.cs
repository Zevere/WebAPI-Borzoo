using System.Threading.Tasks;
using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public interface IQueryResolver
    {
        Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context);
        
        Task<UserDto> GetUserAsync(ResolveFieldContext<object> context);
        
        Task<TaskListDto> CreateTaskListAsync(ResolveFieldContext<object> context);
        
        Task<TaskListDto[]> GetTaskListsForUserAsync(ResolveFieldContext<UserDto> context);
        
        Task<TaskItemDto> CreateTaskItemAsync(ResolveFieldContext<object> context);
        
        Task<TaskItemDto[]> GetTaskItemsForListAsync(ResolveFieldContext<TaskListDto> context);
    }
}