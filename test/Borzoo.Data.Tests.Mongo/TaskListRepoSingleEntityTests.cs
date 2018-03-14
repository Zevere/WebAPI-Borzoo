using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Borzoo.Data.Mongo.Entities;
using Borzoo.Data.Tests.Common;
using Borzoo.Data.Tests.Mongo.Framework;
using Xunit;

namespace Borzoo.Data.Tests.Mongo
{
    public class TaskListRepoSingleEntityTests :
        TaskListRepoSingleEntityTestsBase,
        IClassFixture<TaskListRepoSingleEntityTests.Fixture>
    {
        private readonly Fixture _fixture;

        public TaskListRepoSingleEntityTests(Fixture fixture)
            : base(() => new TaskListRepository(fixture.Collection, fixture.UserRepo))
        {
            _fixture = fixture;
        }

        public class Fixture : FixtureBase<TaskListMongo>
        {
            public IUserRepository UserRepo { get; }

            public Fixture()
                : base(MongoConstants.Collections.TaskLists.Name)
            {
                UserRepo = new UserRepository(
                    Collection.Database.GetCollection<User>(MongoConstants.Collections.Users.Name)
                );
                
                SeedUserDataAsync().GetAwaiter().GetResult();
            }
        }
    }
}