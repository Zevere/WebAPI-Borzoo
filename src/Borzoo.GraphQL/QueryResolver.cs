using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.GraphQL
{
    /// <inheritdoc />
    public class QueryResolver : IQueryResolver
    {
        private readonly IUserRepository _userRepo;

        private readonly ITaskListRepository _taskListRepo;

        private readonly ITaskItemRepository _taskItemRepo;

        /// <inheritdoc />
        public QueryResolver(
            IUserRepository userRepo,
            ITaskListRepository taskListRepo,
            ITaskItemRepository taskItemRepo
        )
        {
            _userRepo = userRepo;
            _taskListRepo = taskListRepo;
            _taskItemRepo = taskItemRepo;
        }

        /// <inheritdoc />
        public async Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context)
        {
            var dto = context.GetArgument<UserCreationDto>("user");

            var entity = (User) dto;
            try
            {
                await _userRepo.AddAsync(entity, context.CancellationToken)
                    .ConfigureAwait(false);

                string token = GenerateAlphaNumericString(100);
                await _userRepo.SetTokenForUserAsync(entity.Id, token, context.CancellationToken)
                    .ConfigureAwait(false);
                string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                entity.Token = encodedToken;
            }
            catch (DuplicateKeyException)
            {
                var err = new Error("duplicate key")
                {
                    Path = new[] { "user" }
                };
                context.Errors.Add(err);
                return default;
            }

            return (UserDto) entity;
        }

        /// <inheritdoc />
        public async Task<UserDto> LoginAsync(ResolveFieldContext<object> context)
        {
            var login = context.GetArgument<UserLoginDto>("login");

            User entity;
            try
            {
                entity = await _userRepo.GetByNameAsync(login.Username, cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (EntityNotFoundException)
            {
                var err = new Error("authentication failed")
                {
                    Path = new[] { "login" }
                };
                context.Errors.Add(err);
                return null;
            }

            if (login.Passphrase != entity.PassphraseHash)
            {
                var err = new Error("authentication failed")
                {
                    Path = new[] { "login" }
                };
                context.Errors.Add(err);
                return null;
            }

            return (UserDto) entity;
        }

        /// <inheritdoc />
        public async Task<UserDto> GetUserAsync(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("userId");
            User entity;
            try
            {
                entity = await _userRepo.GetByNameAsync(username, cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (EntityNotFoundException)
            {
                var err = new Error("not found")
                {
                    Path = new[] { "user" }
                };
                context.Errors.Add(err);
                return null;
            }

            return (UserDto) entity;
        }

        /// <inheritdoc />
        public async Task<TaskList> CreateTaskListAsync(ResolveFieldContext<object> context)
        {
            string ownerId = context.GetArgument<string>("owner");
            {
                try
                {
                    await _userRepo.GetByNameAsync(ownerId, context.CancellationToken)
                        .ConfigureAwait(false);
                }
                catch (EntityNotFoundException)
                {
                    var err = new Error("User ID not found.");
                    context.Errors.Add(err);
                    return default;
                }
            }

            var dto = context.GetArgument<TaskListCreationDto>("list");

            var entity = (TaskList) dto;
            entity.OwnerId = ownerId;

            if (entity.DisplayId == null)
            {
                entity.DisplayId = IdGenerator.GetIdFromTitle(entity.Title);
            }
            else
            {
                bool isIdValid = IdGenerator.IsValid(entity.DisplayId);
                if (!isIdValid)
                {
                    var err = new Error("Invalid List ID.")
                    {
                        Path = new[] { "list" }
                    };
                    context.Errors.Add(err);
                    return default;
                }
            }

            try
            {
                await _taskListRepo.AddAsync(entity, context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (DuplicateKeyException)
            {
                var err = new Error("duplicate key")
                {
                    Path = new[] { "list" }
                };
                context.Errors.Add(err);
                return default;
            }

            return entity;
        }

        /// <inheritdoc />
        public async Task<bool> DeleteTaskListAsync(ResolveFieldContext<object> context)
        {
            string ownerId = context.GetArgument<string>("owner");
            string listId = context.GetArgument<string>("list");

            TaskList taskList;
            try
            {
                taskList = await _taskListRepo.GetByNameAsync(listId, ownerId, context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (EntityNotFoundException)
            {
                var err = new Error("Task list not found.");
                context.Errors.Add(err);
                return false;
            }

            await _taskListRepo.DeleteAsync(taskList.Id, context.CancellationToken)
                .ConfigureAwait(false);

            return true;
        }

        /// <inheritdoc />
        public async Task<TaskList> GetTaskListForUserAsync(ResolveFieldContext<UserDto> context)
        {
            string username = context.Source.Id;
            string listName = context.Arguments["listId"].ToString();

            TaskList taskList;
            try
            {
                taskList = await _taskListRepo.GetByNameAsync(listName, username, context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (EntityNotFoundException)
            {
                var err = new Error("Task List not found.")
                {
                    Path = new[] { "user.list" }
                };
                context.Errors.Add(err);
                return default;
            }

            return taskList;
        }

        /// <inheritdoc />
        public async Task<TaskList[]> GetAllTaskListsForUserAsync(ResolveFieldContext<UserDto> context)
        {
            string username = context.Source.Id;

            var taskLists = await _taskListRepo.GetUserTaskListsAsync(username, context.CancellationToken)
                .ConfigureAwait(false);

            return taskLists;
        }

        /// <inheritdoc />
        public async Task<TaskItem> CreateTaskItemAsync(ResolveFieldContext<object> context)
        {
            string ownerId = context.GetArgument<string>("ownerId");
            string listId = context.GetArgument<string>("listId");

            if (!IdGenerator.IsValid(ownerId))
            {
                var err = new Error("Invalid owner ID.")
                {
                    Path = new[] { "createTask" }
                };
                context.Errors.Add(err);
                return default;
            }

            if (IdGenerator.IsValid(listId))
            {
                try
                {
                    await _taskListRepo.GetByNameAsync(listId, ownerId, context.CancellationToken)
                        .ConfigureAwait(false);
                }
                catch (EntityNotFoundException)
                {
                    var err = new Error("Task list not found.")
                    {
                        Path = new[] { "createTask" }
                    };
                    context.Errors.Add(err);
                    return default;
                }
            }
            else
            {
                var err = new Error("Invalid list ID.")
                {
                    Path = new[] { "createTask" }
                };
                context.Errors.Add(err);
                return default;
            }

            var dto = context.GetArgument<TaskItemCreationDto>("task");
            var entity = (TaskItem) dto;
            entity.OwnerId = ownerId;
            entity.ListId = listId;

            if (entity.DisplayId == null)
            {
                entity.DisplayId = IdGenerator.GetIdFromTitle(entity.Title);
            }
            else
            {
                bool isIdValid = IdGenerator.IsValid(entity.DisplayId);
                if (!isIdValid)
                {
                    var err = new Error("Invalid Task ID.")
                    {
                        Path = new[] { "createTask" }
                    };
                    context.Errors.Add(err);
                    return default;
                }
            }

            try
            {
                await _taskItemRepo.AddAsync(entity, context.CancellationToken)
                    .ConfigureAwait(false);
            }
            catch (DuplicateKeyException)
            {
                var err = new Error("duplicate key")
                {
                    Path = new[] { "createTask" }
                };
                context.Errors.Add(err);
                return default;
            }

            return entity;
        }

        /// <inheritdoc />
        public Task<TaskItem[]> GetTaskItemsForListAsync(ResolveFieldContext<TaskList> context)
        {
            return Task.FromResult(new TaskItem[0]);
//            string taskListName = context.Source.DisplayId;
//            string ownerId = context.Source.OwnerId;
//
//            await _taskItemRepo.SetTaskListAsync(ownerId, taskListName, context.CancellationToken)
//                .ConfigureAwait(false);
//
//            var tasks = await _taskItemRepo.GetTaskItemsAsync(cancellationToken: context.CancellationToken)
//                .ConfigureAwait(false);
//
//            var taskDtos = tasks
//                .Select(tl => (TaskItemDto) tl)
//                .ToArray();
//
//            return taskDtos;
        }

        private string GenerateAlphaNumericString(int charCount)
        {
            var rnd = new Random(DateTime.UtcNow.Millisecond);
            var chars = Enumerable.Range(0, charCount)
                .Select(_ =>
                {
                    char c = default;
                    int charType = rnd.Next() % 3;
                    switch (charType)
                    {
                        case 0: // Number
                            c = (char) rnd.Next(48, 50);
                            break;
                        case 1: // Upper-Case Letter
                            c = (char) rnd.Next(65, 91);
                            break;
                        case 2: // Lower-Case Letter
                            c = (char) rnd.Next(97, 123);
                            break;
                    }

                    return c;
                });
            return string.Join(string.Empty, chars);
        }
    }
}
