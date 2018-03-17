using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.GraphQL;
using Borzoo.GraphQL.Models;
using GraphQL.Types;

namespace Borzoo.Web.GraphQL
{
    public class QueryResolver : IQueryResolver
    {
        private readonly IUserRepository _userRepo;
        
        private readonly ITaskListRepository _taskListRepo;

        public QueryResolver(IUserRepository userRepo, ITaskListRepository taskListRepo)
        {
            _userRepo = userRepo;
            _taskListRepo = taskListRepo;
        }

        public async Task<UserDto> CreateUserAsync(ResolveFieldContext<object> context)
        {
            var dto = context.GetArgument<UserCreationDto>("user");

            var entity = (User) dto;
            try
            {
                await _userRepo.AddAsync(entity, context.CancellationToken);

                string token = GenerateAlphaNumericString(100);
                await _userRepo.SetTokenForUserAsync(entity.Id, token, context.CancellationToken);
                string encodedToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
                entity.Token = encodedToken;
            }
            catch (DuplicateKeyException)
            {
                var err = new Error("duplicate key")
                {
                    Path = new[] {"user"}
                };
                context.Errors.Add(err);
                return default;
            }

            return (UserDto) entity;
        }
        
        public async Task<UserDto> GetUserAsync(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("id");
            User entity;
            try
            {
                entity = await _userRepo.GetByNameAsync(username, cancellationToken: context.CancellationToken);
            }
            catch (EntityNotFoundException)
            {
                var err = new Error("not found")
                {
                    Path = new[] {"user"}
                };
                context.Errors.Add(err);
                return default;
            }

            return (UserDto) entity;
        }

        public async Task<TaskListDto> CreateTaskListAsync(ResolveFieldContext<object> context)
        {
            string username = context.GetArgument<string>("owner");
            var dto = context.GetArgument<TaskListCreationDto>("list");

            var entity = (TaskList) dto;
            await _taskListRepo.SetUsernameAsync(username);
            try
            {
                await _taskListRepo.AddAsync(entity, context.CancellationToken);
            }
            catch (DuplicateKeyException)
            {
                var err = new Error("duplicate key")
                {
                    Path = new[] {"list"}
                };
                context.Errors.Add(err);
                return default;
            }

            return (TaskListDto) entity;            
        }
        
        public async Task<TaskListDto[]> GetTaskListsForUserAsync(ResolveFieldContext<UserDto> context)
        {
            string username = context.Source.Id;
            await _taskListRepo.SetUsernameAsync(username, context.CancellationToken);

            var taskLists = await _taskListRepo.GetUserTaskListsAsync(context.CancellationToken);
            var taskListDtos = taskLists
                .Select(tl => (TaskListDto)tl)
                .ToArray();
            
            return taskListDtos;
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