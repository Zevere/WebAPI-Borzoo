﻿using System.Threading.Tasks;
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

        Task<TaskList> CreateTaskListAsync(ResolveFieldContext<object> context);

        Task<bool> DeleteTaskListAsync(ResolveFieldContext<object> context);

        /// <summary>
        /// Resolve a single task list for user
        /// </summary>
        Task<TaskList> GetTaskListForUserAsync(ResolveFieldContext<UserDto> context);

        Task<TaskList[]> GetAllTaskListsForUserAsync(ResolveFieldContext<UserDto> context);

        Task<TaskItemDto> CreateTaskItemAsync(ResolveFieldContext<object> context);

        Task<TaskItemDto[]> GetTaskItemsForListAsync(ResolveFieldContext<TaskList> context);
    }
}
