//using Borzoo.Data.Abstractions;
//using Borzoo.Data.SQLite;
//using Borzoo.Data.Tests.Common;
//using Borzoo.Data.Tests.SQLite.Framework;
//using Xunit;
//
//namespace Borzoo.Data.Tests.SQLite
//{
//    public class TaskListRepoSingleEntityTests :
//        TaskListRepoSingleEntityTestsBase,
//        IClassFixture<TaskListRepoSingleEntityTests.Fixture>
//    {
//        public TaskListRepoSingleEntityTests(Fixture fixture)
//            : base(() => new TaskListRepository(fixture.Connection, fixture.UserRepo))
//        {
//        }
//
//        public class Fixture : FixtureBase
//        {
//            public IUserRepository UserRepo { get; }
//
//            public Fixture()
//                : base(nameof(TaskListRepoSingleEntityTests))
//            {
//                UserRepo = new UserRepository(Connection);
//
//                TestDataSeeder.SeedUsersAsync(UserRepo).GetAwaiter().GetResult();
//            }
//        }
//    }
//}