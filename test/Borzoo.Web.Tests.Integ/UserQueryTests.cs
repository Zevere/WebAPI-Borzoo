using System;
using System.Net;
using System.Threading.Tasks;
using Borzoo.Tests.Framework;
using Borzoo.Web.Tests.Integ.Framework;
using Newtonsoft.Json;
using Xunit;

namespace Borzoo.Web.Tests.Integ
{
    public class UsersQueryTests : IClassFixture<TestHostFixture<Startup>>
    {
        private readonly TestHostFixture<Startup> _fixture;

        public UsersQueryTests(TestHostFixture<Startup> fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Id()
        {
            const string userQuery = @"{ 
                user(id: ""bobby"") { 
                    id firstName lastName token daysJoined joinedAt 
                } 
            }";

            var resp = await _fixture.SendGraphQLQuery(userQuery);

            string respContent = await resp.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("bobby", (string) result.data.user.id);
            Assert.Equal("Bob", (string) result.data.user.firstName);
            Assert.Equal("Boo", (string) result.data.user.lastName);
            Assert.True(int.TryParse((string) result.data.user.daysJoined, out int daysJoined), "daysJoined is number");
            Assert.Equal(daysJoined, 0);
            Assert.True(DateTime.TryParse((string) result.data.user.joinedAt, out var joinedAt), "joinedAt is date");
            Assert.True(joinedAt < DateTime.UtcNow, "Joined before today");
            Assert.Null((string) result.data.user.token);
        }
    }
}