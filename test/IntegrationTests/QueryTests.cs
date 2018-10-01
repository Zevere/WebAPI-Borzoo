using System;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Tests.Framework;
using Borzoo.Web;
using IntegrationTests.Framework;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests
{
    public class QueryTests : IClassFixture<TestHostFixture<Startup>>
    {
        private readonly TestHostFixture<Startup> _fixture;

        private readonly ITestOutputHelper _output;

        public QueryTests(TestHostFixture<Startup> fixture, ITestOutputHelper output)
        {
            _fixture = fixture;
            _output = output;
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Id()
        {
            const string userQuery = @"{ 
                user(userId: ""bobby"") { 
                    id firstName lastName token daysJoined joinedAt 
                } 
            }";

            var resp = await _fixture.SendGraphQLRequest(userQuery);

            string respContent = await resp.Content.ReadAsStringAsync();
            _output.WriteLine(respContent);
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Null(result.errors);
            Assert.Equal("bobby", (string) result.data.user.id);
            Assert.Equal("Bob", (string) result.data.user.firstName);
            Assert.Equal("Boo", (string) result.data.user.lastName);
            Assert.True(int.TryParse((string) result.data.user.daysJoined, out int daysJoined), "daysJoined is number");
            Assert.Equal(0, daysJoined);
            Assert.True(DateTime.TryParse((string) result.data.user.joinedAt, out var joinedAt), "joinedAt is date");
            Assert.True(joinedAt < DateTime.UtcNow, "Joined before today");
            Assert.Null((string) result.data.user.token);
        }
    }
}