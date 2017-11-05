using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Models.User;
using Newtonsoft.Json;
using Xunit;

namespace Borzoo.Tests.Integ
{
    public class UsersControllerTests : IClassFixture<TestHostFixture<Startup>>
    {
        private readonly HttpClient _client;

        public UsersControllerTests(TestHostFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task Should_Do()
        {
            var userCreationDto = new UserCreationRequest
            {
                Name = "username",
                FirstName = "1st name",
                Passphrase = "secret",
            };

            var result = await _client.PostAsync(
                Constants.ZVeerRoutes.Users,
                new StringContent(JsonConvert.SerializeObject(userCreationDto),
                    Encoding.UTF8, "application/vnd.zv.user.creation+json")
            );

            Assert.Equal(HttpStatusCode.NotImplemented, result.StatusCode);
        }
    }
}