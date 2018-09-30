//using Borzoo.Data.Abstractions;
//using Borzoo.Data.Abstractions.Entities;
//using Borzoo.Data.SQLite;
//using Borzoo.Data.Tests.Common;
//using Borzoo.Data.Tests.SQLite.Framework;
//using Xunit;
//
//namespace Borzoo.Data.Tests.SQLite
//{
//    public class TaskRepoTests :
//        TaskItemRepoTestsBase,
//        IClassFixture<TaskRepoTests.Fixture>
//    {
//        private readonly Fixture _fixture;
//
//        public TaskRepoTests(Fixture fixture)
//            : base(() => new TaskItemRepository(fixture.Connection, fixture.TaskListRepo))
//        {
//            _fixture = fixture;
//        }
//
//        /*
//        [OrderedFact]
//        public async Task Should_Find_UserId_From_UserName()
//        {
//            const string userName = "BoBby";
//
//            ITaskItemRepository sut = CreateTaskItemRepo();
//            await sut.SetTaskListAsync("BoBby", "list1");
//
//            Assert.Equal("2", sut.TaskListId);
//            Assert.Equal(userName, sut.TaskListName);
//        }
//
//        [OrderedFact]
//        public async Task Should_Add_New_Task()
//        {
//            const string taskTitle = "Task 1";
//            const string taskName = "test_name";
//
//            ITaskItemRepository sut = CreateTaskItemRepo();
//            await sut.SetTaskListAsync("bobby", "list1");
//
//            var task = await sut.AddAsync(new TaskItem
//            {
//                DisplayId = taskName,
//                Title = taskTitle,
//            });
//
//            Assert.Equal("1", task.Id);
//            Assert.Equal(taskName, task.DisplayId);
//            Assert.Equal(taskTitle, task.Title);
//            Assert.InRange(task.CreatedAt, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow.AddSeconds(10));
//            Assert.False(task.IsDeleted);
//            Assert.Null(task.Description);
//            Assert.Null(task.Due);
//            Assert.Null(task.ModifiedAt);
//
//            _fixture.TaskItem = task;
//        }
//
//        [OrderedFact]
//        public async Task Should_Throw_Adding_Same_Task_Name()
//        {
//            var task = new TaskItem
//            {
//                DisplayId = "test_name",
//                Title = "title",
//            };
//            ITaskItemRepository sut = CreateTaskItemRepo();
//            await sut.SetTaskListAsync("bobby", "list1");
//
//            var exception = await Assert.ThrowsAsync<SqliteException>(() => sut.AddAsync(task));
//
//            Assert.Contains("UNIQUE constraint failed", exception.Message);
//        }
//
//        [OrderedFact]
//        public async Task Should_Get_Task_Name()
//        {
//            ITaskItemRepository sut = CreateTaskItemRepo();
//            await sut.SetTaskListAsync("bobby", "list1");
//
//            var task = await sut.GetByNameAsync(_fixture.TaskItem.DisplayId);
//
//            Assert.Equal(_fixture.TaskItem.Id, task.Id);
//            Assert.Equal(_fixture.TaskItem.DisplayId, task.DisplayId);
//            Assert.Equal(_fixture.TaskItem.Title, task.Title);
//            Assert.InRange(task.CreatedAt,
//                _fixture.TaskItem.CreatedAt.AddSeconds(-1),
//                _fixture.TaskItem.CreatedAt.AddSeconds(1)
//            );
//            Assert.False(task.IsDeleted);
//            Assert.Null(task.Description);
//            Assert.Null(task.Due);
//            Assert.Null(task.ModifiedAt);
//        }
//
//        */
//        public class Fixture : FixtureBase
//        {
//            public ITaskListRepository TaskListRepo { get; }
//
//            public TaskItem TaskItem { get; set; }
//
//            public Fixture()
//                : base(nameof(TaskRepoTests))
//            {
//                var userRepo = new UserRepository(Connection);
//                TaskListRepo = new TaskListRepository(Connection, userRepo);
//
//                TestDataSeeder.SeedUsersAsync(userRepo).GetAwaiter().GetResult();
//                
//                TaskListRepo.SetUsernameAsync("bobby").GetAwaiter().GetResult();
//                TestDataSeeder.SeedTaskListAsync(TaskListRepo).GetAwaiter().GetResult();
//            }
//        }
//    }
//}