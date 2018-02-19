using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Web.Models.Login;
using Borzoo.Web.Models.User;
using Borzoo.Web.Tests.Integ.Framework;
using Newtonsoft.Json;
using Xunit;

namespace Borzoo.Web.Tests.Integ
{
    public class UsersQueryTests : IClassFixture<TestHostFixture<Startup>>
    {
        private readonly HttpClient _client;

        public UsersQueryTests(TestHostFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [OrderedFact]
        public async Task Should_Get_User_By_Id()
        {
            string userQuery = @"{ user(id: \""bobby\"") { id firstName lastName joinedAt } }";

            var resp = await _client.PostAsync("/graphql", new StringContent(
                $@"{{ ""query"": ""{userQuery}"" }}", Encoding.UTF8, "application/json"
            ));

            string respContent = await resp.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
            Assert.Equal("bobby", (string) result.data.user.id);
            Assert.Equal("Bob", (string) result.data.user.firstName);
            Assert.Equal("Boo", (string) result.data.user.lastName);
            Assert.NotEmpty((string) result.data.user.joinedAt);
        }
    }
}