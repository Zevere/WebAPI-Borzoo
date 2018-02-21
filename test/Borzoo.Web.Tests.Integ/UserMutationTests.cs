using System;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Tests.Framework;
using Borzoo.Web.Tests.Integ.Framework;
using Newtonsoft.Json;
using Xunit;

namespace Borzoo.Web.Tests.Integ
{
    public class UsersMutationTests : IClassFixture<TestHostFixture<Startup>>
    {
        private readonly TestHostFixture<Startup> _fixture;

        public UsersMutationTests(TestHostFixture<Startup> fixture)
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
            Assert.Equal((int) result.data.createUser.daysJoined, 0);
            Assert.True(DateTime.TryParse((string) result.data.createUser.joinedAt, out var join), "joinedAt is date");
            Assert.InRange(join, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow);
            Assert.NotEmpty((string) result.data.createUser.token);
        }
    }
}