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
    public class UsersMutationTests : IClassFixture<UsersMutationTests.Fixture>
    {
        private readonly Fixture _fixture;

        public UsersMutationTests(Fixture fixture)
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
                list = new {name = "GROCERIES", title = "ToDo Groceries"}
            };
            var resp = await _fixture.SendGraphQLQuery(mutation, variables);

            string respContent = await resp.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("groceries", (string) result.data.createList.id);
            Assert.Equal("ToDo Groceries", (string) result.data.createList.title);
            Assert.Null(((JValue)result.data.createList.tasks).Value);
            Assert.True(
                DateTime.TryParse((string) result.data.createList.createdAt, out var creationDate), "createdAt is date"
            );
            Assert.InRange(creationDate, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow);
            
            _fixture.TaskList = JObject.FromObject(result.data.createList).ToObject<TaskListDto>();
        }

        public class Fixture : TestHostFixture<Startup>
        {
            public UserDto User;

            public TaskListDto TaskList;
        }
    }
}