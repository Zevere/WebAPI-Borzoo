using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Tests.Framework;
using Xunit;

namespace Borzoo.Data.Tests.Common
{
    public abstract class TaskListRepoSingleEntityTestsBase
    {
        protected Func<ITaskListRepository> CreateTaskListRepo { get; }

        protected TaskListRepoSingleEntityTestsBase(Func<ITaskListRepository> repoResolver)
        {
            CreateTaskListRepo = repoResolver;
        }

        [OrderedFact]
        public async Task Should_Add_TaskList()
        {
            ITaskListRepository repo = CreateTaskListRepo();
            await repo.SetUsernameAsync("bobby");

            var taskList = new TaskList
            {
                DisplayId = "my_tasks",
                Title = "My Tasks",
            };

            TaskList entity = await repo.AddAsync(taskList);

            Assert.Same(taskList, entity);
            Assert.NotEmpty(entity.OwnerId);
            Assert.NotEmpty(entity.Title);
            Assert.NotEmpty(entity.DisplayId);
            Assert.InRange(entity.CreatedAt, DateTime.UtcNow.AddSeconds(-30), DateTime.UtcNow);
        }

        [OrderedFact]
        public async Task Should_Throw_If_Duplicate_Name()
        {
            ITaskListRepository repo = CreateTaskListRepo();
            await repo.SetUsernameAsync("bobby");

            var taskList = new TaskList
            {
                DisplayId = "my_tasks",
                Title = "My Tasks 2",
            };

            var e = await Assert.ThrowsAsync<DuplicateKeyException>(() =>
                repo.AddAsync(taskList)
            );

            Assert.Contains("OwnerId", e.Keys);
            Assert.Contains("DisplayId", e.Keys);
        }

        [OrderedFact]
        public async Task Should_Throw_If_Username_Not_Set()
        {
            ITaskListRepository repo = CreateTaskListRepo();

            var e = await Assert.ThrowsAsync<ArgumentNullException>(() =>
                repo.AddAsync(new TaskList())
            );

            Assert.Equal("UserId", e.ParamName);
        }

        [OrderedFact]
        public async Task Should_Get_All_TaskLists()
        {
            ITaskListRepository repo = CreateTaskListRepo();
            await repo.SetUsernameAsync("bobby");

            TaskList[] taskLists = await repo.GetUserTaskListsAsync();

            Assert.NotEmpty(taskLists);
        }
    }
}