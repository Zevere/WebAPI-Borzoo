using System;
using System.Threading.Tasks;
using Borzoo.Data.Abstractions;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Data.Mongo;
using Framework;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoTests.Shared;
using Xunit;

namespace MongoTests
{
    [Collection("task list repository")]
    public class TaskListRepoTests : IClassFixture<DatabaseFixture>
    {
        private readonly DatabaseFixture _fxt;

        public TaskListRepoTests(DatabaseFixture fixture)
        {
            _fxt = fixture;
        }

        [OrderedFact("Should create a new task list")]
        public async Task Should_Add_New_TaskList()
        {
            // insert a new task list into the collection
            TaskList taskList;
            {
                ITaskListRepository taskListRepo = new TaskListRepository(
                    _fxt.Database.GetCollection<TaskList>("task-lists")
                );
                taskList = new TaskList
                {
                    DisplayId = "GROCERIes",
                    Title = "My Groceries List",
                    OwnerId = "bobby",
                };
                await taskListRepo.AddAsync(taskList);
            }

            // ensure task list object is updated
            {
                Assert.Equal("My Groceries List", taskList.Title);
                Assert.Equal("groceries", taskList.DisplayId);
                Assert.Equal("bobby", taskList.OwnerId);
                Assert.NotNull(taskList.Id);
                Assert.True(ObjectId.TryParse(taskList.Id, out _), "Entity's ID should be a Mongo ObjectID.");
                Assert.InRange(taskList.CreatedAt, DateTime.UtcNow.AddSeconds(-5), DateTime.UtcNow);

                Assert.Null(taskList.Description);
                Assert.Null(taskList.Tags);
                Assert.Null(taskList.Collaborators);
                Assert.Null(taskList.ModifiedAt);
            }

            // ensure task list document is created in the collection
            {
                BsonDocument taskListDocument = _fxt.Database
                    .GetCollection<BsonDocument>("task-lists")
                    .FindSync(FilterDefinition<BsonDocument>.Empty)
                    .Single();

                Assert.Equal(
                    BsonDocument.Parse($@"{{
                        _id: ObjectId(""{taskList.Id}""),
                        name: ""groceries"",
                        owner: ""bobby"",
                        title: ""My Groceries List"",
                        created: ISODate(""{taskList.CreatedAt:O}"")
                    }}"),
                    taskListDocument
                );
            }
        }

        [OrderedFact("Should get task list 'groceries' by name")]
        public async Task Should_Get_TaskList_By_Name()
        {
            ITaskListRepository taskListRepo = new TaskListRepository(
                _fxt.Database.GetCollection<TaskList>("task-lists")
            );

            // get task list by unique name and owner
            TaskList taskList = await taskListRepo.GetByNameAsync("grocerIES", "bobby");

            Assert.Equal("My Groceries List", taskList.Title);
            Assert.Equal("groceries", taskList.DisplayId);
            Assert.Equal("bobby", taskList.OwnerId);
            Assert.NotNull(taskList.Id);
            Assert.True(ObjectId.TryParse(taskList.Id, out _), "Entity ID should be a Mongo ObjectID.");
            Assert.InRange(taskList.CreatedAt, DateTime.UtcNow.AddSeconds(-30), DateTime.UtcNow);

            Assert.Null(taskList.Description);
            Assert.Null(taskList.Tags);
            Assert.Null(taskList.Collaborators);
            Assert.Null(taskList.ModifiedAt);
        }

        [OrderedFact("Should throw when adding a duplicate task list")]
        public async Task Should_Throw_When_Add_Duplicate_TaskList()
        {
            ITaskListRepository taskListRepo = new TaskListRepository(
                _fxt.Database.GetCollection<TaskList>("task-lists")
            );

            await Assert.ThrowsAsync<DuplicateKeyException>(() =>
                taskListRepo.AddAsync(new TaskList { DisplayId = "groceries", OwnerId = "bobby" })
            );
        }

        [OrderedFact("Should throw when getting non-existing task list by name")]
        public async Task Should_Throw_When_Get_NonExisting_TaskList()
        {
            ITaskListRepository taskListRepo = new TaskListRepository(
                _fxt.Database.GetCollection<TaskList>("task-lists")
            );

            await Assert.ThrowsAsync<EntityNotFoundException>(() =>
                taskListRepo.GetByNameAsync("task name ಠ_ಠ", "owner id (ツ)")
            );
        }
    }
}
