using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Web.Models.User;
using Newtonsoft.Json;
using Xunit;

namespace Borzoo.Web.Tests.Integ
{
    public class UsersControllerTests : IClassFixture<TestHostFixture<Startup>>
    {
        private readonly HttpClient _client;

        public UsersControllerTests(TestHostFixture<Startup> fixture)
        {
            _client = fixture.Client;
        }

        [Fact]
        public async Task Should_Create_User()
        {
            var userCreationDto = new UserCreationRequest
            {
                Name = "username",
                FirstName = "1st name",
                Passphrase = "secretpassphrase",
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Constants.ZVeerRoutes.Users)
            {
                Headers = {Accept = {new MediaTypeWithQualityHeaderValue(Constants.ZVeerContentTypes.User.Full)}},
                Content = new StringContent(
                    JsonConvert.SerializeObject(userCreationDto),
                    Encoding.UTF8, "application/vnd.zv.user.creation+json"
                ),
            };
            var response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responsePayloadText = await response.Content.ReadAsStringAsync();
            var userFullRepresentation = JsonConvert.DeserializeObject<UserFullDto>(responsePayloadText);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(userCreationDto.FirstName, userFullRepresentation.FirstName);
            Assert.Equal(userCreationDto.LastName, userFullRepresentation.LastName);
            Assert.NotEmpty(userFullRepresentation.Id);
        }

        [Fact]
        public async Task Should_Reject_Unsupported_Media_Types()
        {
            var userCreationDto = new UserCreationRequest
            {
                Name = "username",
                FirstName = "1st name",
                Passphrase = "secretpassphrase",
            };

            var result = await _client.PostAsync(
                Constants.ZVeerRoutes.Users,
                new StringContent(JsonConvert.SerializeObject(userCreationDto),
                    Encoding.UTF8, "application/json")
            );

            Assert.Equal(HttpStatusCode.UnsupportedMediaType, result.StatusCode);
        }
    }
}