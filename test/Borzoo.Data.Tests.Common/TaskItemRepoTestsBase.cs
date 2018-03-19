using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Tests.Framework;
using Xunit;

namespace Borzoo.Data.Tests.Common
{
    public abstract class TaskItemRepoTestsBase
    {
        protected Func<ITaskItemRepository> CreateTaskItemRepo { get; }

        protected TaskItemRepoTestsBase(Func<ITaskItemRepository> repoResolver)
        {
            CreateTaskItemRepo = repoResolver;
        }

        [OrderedFact]
        public async Task Should_Add_TaskItem()
        {
            ITaskItemRepository repo = CreateTaskItemRepo();
            await repo.SetTaskListAsync("bobby", "list");

            var task = new TaskItem
            {
                DisplayId = "task1",
                Title = "Do something",
            };

            TaskItem entity = await repo.AddAsync(task);

            Assert.Same(task, entity);
            Assert.NotEmpty(entity.Id);
            Assert.NotEmpty(entity.ListId);
            Assert.NotEmpty(entity.Title);
            Assert.NotEmpty(entity.DisplayId);
            Assert.InRange(entity.CreatedAt, DateTime.UtcNow.AddSeconds(-30), DateTime.UtcNow);
        }

        [OrderedFact]
        public async Task Should_Throw_If_Duplicate_Name()
        {
            ITaskItemRepository repo = CreateTaskItemRepo();
            await repo.SetTaskListAsync("bobby", "list");

            var task = new TaskItem
            {
                DisplayId = "task1",
                Title = "ToDo",
            };

            var e = await Assert.ThrowsAsync<DuplicateKeyException>(() =>
                repo.AddAsync(task)
            );

            Assert.Contains("ListId", e.Keys);
            Assert.Contains("DisplayId", e.Keys);
        }
    }
}