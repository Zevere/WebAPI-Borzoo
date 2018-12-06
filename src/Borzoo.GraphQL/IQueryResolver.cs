using System.Threading.Tasks;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    public interface IQueryResolver
    {
        Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context);

        Task<UserDto> LoginAsync(ResolveFieldContext<object> context);

        Task<UserDto> GetUserAsync(ResolveFieldContext<object> context);

        /// <summary>
        /// Creates a new task list
        /// </summary>
        Task<TaskList> CreateTaskListAsync(ResolveFieldContext<object> context);

        Task<bool> DeleteTaskListAsync(ResolveFieldContext<object> context);

        /// <summary>
        /// Resolves a single task list for user
        /// </summary>
        Task<TaskList> GetTaskListForUserAsync(ResolveFieldContext<UserDto> context);

        Task<TaskList[]> GetAllTaskListsForUserAsync(ResolveFieldContext<UserDto> context);

        /// <summary>
        /// Creates a new task item
        /// </summary>
        Task<TaskItem> CreateTaskItemAsync(ResolveFieldContext<object> context);

        Task<TaskItem[]> GetTaskItemsForListAsync(ResolveFieldContext<TaskList> context);

        /// <summary>
        /// Deletes a task item from the list
        /// </summary>
        Task<bool> DeleteTaskItemAsync(ResolveFieldContext<object> context);
    }
}
