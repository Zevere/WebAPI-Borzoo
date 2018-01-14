using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.SQLite.Tests.Framework;
using Microsoft.Data.Sqlite;
using Xunit;

namespace Borzoo.Data.SQLite.Tests
{
    public class TaskRepoSingleEntityTests : IClassFixture<TaskRepoSingleEntityTests.Fixture>
    {
        private SqliteConnection Connection => _fixture.Connection;

        private readonly Fixture _fixture;

        public TaskRepoSingleEntityTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact]
        public void Should_Find_UserId_From_UserName()
        {
            const string userName = "BoBby";

            ITaskRepository sut = new TaskRepository(Connection);

            sut.UserName = "BoBby";

            Assert.Equal("2", sut.UserId);
            Assert.Equal(userName, sut.UserName);
        }

        [OrderedFact]
        public async Task Should_Add_New_Task()
        {
            const string taskTitle = "Task 1";
            const string taskName = "test_name";

            ITaskRepository sut = new TaskRepository(Connection) {UserName = "bobby"};

            var task = await sut.AddAsync(new UserTask
            {
                Name = taskName,
                Title = taskTitle,
            });

            Assert.Equal("1", task.Id);
            Assert.Equal(taskName, task.Name);
            Assert.Equal(taskTitle, task.Title);
            Assert.InRange(task.CreatedAt, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow.AddSeconds(10));
            Assert.False(task.IsDeleted);
            Assert.Null(task.Description);
            Assert.Null(task.Due);
            Assert.Null(task.ModifiedAt);
        }

        [OrderedFact]
        public async Task Should_Throw_Adding_Same_Task_Name()
        {
            var task = new UserTask
            {
                Name = "test_name",
                Title = "title",
            };
            ITaskRepository sut = new TaskRepository(Connection) {UserName = "bobby"};

            var exception = await Assert.ThrowsAsync<SqliteException>(() => sut.AddAsync(task));

            Assert.Contains("UNIQUE constraint failed", exception.Message);
        }

        public class Fixture : FixtureBase
        {
            public Fixture()
                : base(nameof(TaskRepoSingleEntityTests))
            {
                SeedUserData();
            }
        }
    }
}