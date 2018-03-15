using Borzoo.Data.Abstractions;
using Borzoo.Data.SQLite;
using Borzoo.Data.Tests.Common;
using Borzoo.Data.Tests.SQLite.Framework;
using Xunit;

namespace Borzoo.Data.Tests.SQLite
{
    public class TaskListRepoSingleEntityTests :
        TaskListRepoSingleEntityTestsBase,
        IClassFixture<TaskListRepoSingleEntityTests.Fixture>
    {
        private readonly Fixture _fixture;

        public TaskListRepoSingleEntityTests(Fixture fixture)
            : base(() => new TaskListRepository(fixture.Connection, fixture.UserRepo))
        {
            _fixture = fixture;
        }

        public class Fixture : FixtureBase
        {
            public IUserRepository UserRepo { get; }

            public Fixture()
                : base(nameof(TaskListRepoSingleEntityTests))
            {
                UserRepo = new UserRepository(Connection);

                SeedUserDataAsync().GetAwaiter().GetResult();
            }
        }
    }
}