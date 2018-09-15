using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Borzoo.Data.Mongo.Entities;
using Borzoo.Data.Tests.Common;
using Borzoo.Data.Tests.Mongo.Framework;
using Xunit;

namespace Borzoo.Data.Tests.Mongo
{
    public class TaskItemRepoTests :
        TaskItemRepoTestsBase,
        IClassFixture<TaskItemRepoTests.Fixture>
    {
        public TaskItemRepoTests(Fixture fixture)
            : base(() => new TaskItemRepository(fixture.Collection, fixture.TaskListRepo))
        {
        }

        public class Fixture : FixtureBase<TaskItemMongo>
        {
            public ITaskListRepository TaskListRepo { get; }

            public Fixture()
                : base(MongoConstants.Collections.TaskItems.Name)
            {
                var userRepo = new UserRepository(
                    Collection.Database.GetCollection<User>(MongoConstants.Collections.Users.Name)
                );

                TaskListRepo = new TaskListRepository(
                    Collection.Database.GetCollection<TaskListMongo>(MongoConstants.Collections.TaskLists.Name),
                    userRepo
                );

                TestDataSeeder.SeedUsersAsync(userRepo).GetAwaiter().GetResult();

                TaskListRepo.SetUsernameAsync("bobby").GetAwaiter().GetResult();
                TestDataSeeder.SeedTaskListAsync(TaskListRepo).GetAwaiter().GetResult();
            }
        }
    }
}