using System;
using System.Net;
using System.Threading.Tasks;
using Borzoo.GraphQL.Models;
using Borzoo.Tests.Framework;
using Borzoo.Web.Tests.Integ.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Borzoo.Web.Tests.Integ
{
    public class MutationTests : IClassFixture<MutationTests.Fixture>
    {
        private readonly Fixture _fixture;

        public MutationTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact]
        public async Task Should_Create_User()
        {
            const string mutation = @"
            mutation ZevereMutation($u: UserInput!) { 
                createUser(user: $u) { 
                    id firstName lastName token daysJoined joinedAt
                } 
            }";
            var variables = new
            {
                u = new
                {
                    name = "ELI.1024",
                    firstName = "Eli",
                    passphrase = "my-passphrase9"
                }
            };
            var resp = await _fixture.SendGraphQLQuery(mutation, variables);

            string respContent = await resp.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("eli.1024", (string) result.data.createUser.id);
            Assert.Equal("Eli", (string) result.data.createUser.firstName);
            Assert.Null((string) result.data.createUser.lastName);
            Assert.Equal(0, (int) result.data.createUser.daysJoined);
            Assert.True(DateTime.TryParse((string) result.data.createUser.joinedAt, out var join), "joinedAt is date");
            Assert.InRange(join, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow);
            Assert.NotEmpty((string) result.data.createUser.token);

            _fixture.User = JObject.FromObject(result.data.createUser).ToObject<UserDto>();
        }

        [OrderedFact]
        public async Task Should_Create_TaskList()
        {
            const string mutation = @"
            mutation ZevereMutation($userId: String!, $list: ListInput!) { 
                createList(owner: $userId, list: $list) { 
                    id title createdAt
                    tasks { id }
                }
            }";
            var variables = new
            {
                userId = _fixture.User.Id,
                list = new {id = "GROCERIES", title = "ToDo Groceries"}
            };
            var resp = await _fixture.SendGraphQLQuery(mutation, variables);

            string respContent = await resp.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("groceries", (string) result.data.createList.id);
            Assert.Equal("ToDo Groceries", (string) result.data.createList.title);
            Assert.Null(((JValue) result.data.createList.tasks).Value);
            Assert.True(
                DateTime.TryParse((string) result.data.createList.createdAt, out var creationDate), "createdAt is date"
            );
            Assert.InRange(creationDate, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow);

            _fixture.TaskList = JObject.FromObject(result.data.createList).ToObject<TaskListDto>();
        }

        [OrderedFact]
        public async Task Should_Add_Task_To_List()
        {
            const string mutation = @"
            mutation ZevereMutation($userId: String!, $listId: String!, $task: TaskInput!) { 
                addTask(owner: $userId, list: $listId, task: $task) { 
                    id title description due tags createdAt
                }
            }";
            var variables = new
            {
                userId = _fixture.User.Id,
                listId = _fixture.TaskList.Id,
                task = new
                {
                    id = "fruit",
                    title = "Fruits",
                    description = "Apples",
                    due = DateTime.Today.AddDays(3),
                    tags = new[] {"_priority:5"}
                }
            };
            var resp = await _fixture.SendGraphQLQuery(mutation, variables);

            string respContent = await resp.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("fruit", (string) result.data.addTask.id);
            Assert.Equal("Fruits", (string) result.data.addTask.title);
            Assert.Equal("Apples", (string) result.data.addTask.description);
            Assert.InRange(
                DateTime.Parse((string) result.data.addTask.due),
                DateTime.Today.AddDays(2),
                DateTime.Today.AddDays(4)
            );
            Assert.InRange(
                DateTime.Parse((string) result.data.addTask.createdAt),
                DateTime.UtcNow.AddSeconds(-10),
                DateTime.UtcNow
            );
            string[] tags = JArray.FromObject(result.data.addTask.tags).ToObject<string[]>();
            Assert.NotEmpty(tags);
            Assert.Collection(tags, Assert.NotEmpty);

            _fixture.TaskItem = JObject.FromObject(result.data.addTask).ToObject<TaskItemDto>();
        }

        public class Fixture : TestHostFixture<Startup>
        {
            public UserDto User;

            public TaskListDto TaskList;

            public TaskItemDto TaskItem;
        }
    }
}