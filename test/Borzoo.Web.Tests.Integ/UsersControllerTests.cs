using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Tests.Framework;
using Borzoo.Web.Models.Login;
using Borzoo.Web.Models.User;
using Borzoo.Web.Tests.Integ.Framework;
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

        [OrderedFact]
        public async Task Should_Create_User()
        {
            var userCreationDto = new UserCreationDto
            {
                Name = "username",
                FirstName = "1st name",
                Passphrase = "secretpassphrase",
            };

            var request = new HttpRequestMessage(HttpMethod.Post, Constants.ZevereRoutes.Users)
            {
                Headers = {{"Accept", Constants.ZevereContentTypes.User.Full}},
                Content = new StringContent(
                    JsonConvert.SerializeObject(userCreationDto),
                    Encoding.UTF8, "application/vnd.zv.user.creation+json"
                ),
            };
            var response = await _client.SendAsync(request);

            var responsePayloadText = await response.Content.ReadAsStringAsync();
            var userFullRepresentation = JsonConvert.DeserializeObject<UserFullDto>(responsePayloadText);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(userCreationDto.FirstName, userFullRepresentation.FirstName);
            Assert.Equal(userCreationDto.LastName, userFullRepresentation.LastName);
            Assert.NotEmpty(userFullRepresentation.Id);
        }

        [OrderedFact]
        public async Task Should_Reject_Unsupported_Media_Types()
        {
            var userCreationDto = new UserCreationDto
            {
                Name = "username",
                FirstName = "1st name",
                Passphrase = "secretpassphrase",
            };

            var result = await _client.PostAsync(
                Constants.ZevereRoutes.Users,
                new StringContent(JsonConvert.SerializeObject(userCreationDto),
                    Encoding.UTF8, "application/json")
            );

            Assert.Equal(HttpStatusCode.UnsupportedMediaType, result.StatusCode);
        }

        [OrderedFact]
        public async Task Should_Respond204_For_Existing_User()
        {
            var req = new HttpRequestMessage(
                HttpMethod.Head,
                Constants.ZevereRoutes.User.Replace(Constants.ZevereRoutes.PathParameters.UserId, "aliCE0")
            );

            var result = await _client.SendAsync(req);
            string respContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
            Assert.Empty(respContent);
        }

        [OrderedFact]
        public async Task Should_Respond404_For_NonExisting_User()
        {
            var req = new HttpRequestMessage(
                HttpMethod.Head,
                Constants.ZevereRoutes.User.Replace(Constants.ZevereRoutes.PathParameters.UserId, "abc")
            );

            var result = await _client.SendAsync(req);
            string respContent = await result.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Empty(respContent);
        }

        [OrderedFact]
        public async Task Should_Get_User_Pretty()
        {
            var login = await _client.PostAsync("/zv/login", new StringContent(
                @"{""user_name"":""BObby"",""passphrase"":""secret_passphrase2""}", Encoding.UTF8,
                "application/vnd.zv.login.creation+json"
            ));
            string loginResp = await login.Content.ReadAsStringAsync();
            string token = JsonConvert.DeserializeObject<LoginDto>(loginResp).Token;

            var req = new HttpRequestMessage(HttpMethod.Get, "zv/users/bobby")
            {
                Headers =
                {
                    {"Accept", "application/vnd.zv.user.pretty+json"},
                    {"Authorization", $"Basic {token}"}
                }
            };

            var result = await _client.SendAsync(req);
            string respContent = await result.Content.ReadAsStringAsync();
            dynamic userPretty = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("bobby", (string) userPretty.id);
            Assert.Equal("Bob Boo", (string) userPretty.display_name);
            Assert.True(0 <= (int) userPretty.days_joined);
        }

        [OrderedFact]
        public async Task Should_Get_User_Full()
        {
            var login = await _client.PostAsync("/zv/login", new StringContent(
                @"{""user_name"":""BObby"",""passphrase"":""secret_passphrase2""}", Encoding.UTF8,
                "application/vnd.zv.login.creation+json"
            ));
            string loginResp = await login.Content.ReadAsStringAsync();
            string token = JsonConvert.DeserializeObject<LoginDto>(loginResp).Token;

            var req = new HttpRequestMessage(HttpMethod.Get, "zv/users/bobby")
            {
                Headers =
                {
                    {"Authorization", $"Basic {token}"},
                    {"Accept", "application/vnd.zv.user.full+json"},
                }
            };

            var result = await _client.SendAsync(req);
            string respContent = await result.Content.ReadAsStringAsync();
            dynamic dto = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Equal("bobby", (string) dto.id);
            Assert.Equal("Bob", (string) dto.first_name);
            Assert.Equal("Boo", (string) dto.last_name);
            Assert.NotEmpty((string) dto.joined_at);
        }

        [OrderedFact]
        public async Task Should_Update_User_FirstName()
        {
            var login = await _client.PostAsync("/zv/login", new StringContent(
                @"{""user_name"":""BObby"",""passphrase"":""secret_passphrase2""}", Encoding.UTF8,
                "application/vnd.zv.login.creation+json"
            ));
            string loginResp = await login.Content.ReadAsStringAsync();
            string token = JsonConvert.DeserializeObject<LoginDto>(loginResp).Token;

            var req = new HttpRequestMessage(new HttpMethod("PATCH"), "zv/users/bobby")
            {
                Content = new StringContent(@"{""first_name"":""bbb""}", Encoding.UTF8, "application/json"),
                Headers =
                {
                    {"Authorization", $"Basic {token}"},
                    {"Accept", "application/vnd.zv.user.pretty+json"},
                }
            };

            var result = await _client.SendAsync(req);
            string respContent = await result.Content.ReadAsStringAsync();
            dynamic dto = JsonConvert.DeserializeObject(respContent);

            Assert.Equal(HttpStatusCode.Accepted, result.StatusCode);
            Assert.Equal("bobby", (string) dto.id);
            Assert.Equal("bbb Boo", (string) dto.display_name);
            Assert.NotEmpty((string) dto.days_joined);
        }

        [OrderedFact]
        public async Task Should_Delete_User()
        {
            var login = await _client.PostAsync("/zv/login", new StringContent(
                @"{""user_name"":""BObby"",""passphrase"":""secret_passphrase2""}", Encoding.UTF8,
                "application/vnd.zv.login.creation+json"
            ));
            string loginResp = await login.Content.ReadAsStringAsync();
            string token = JsonConvert.DeserializeObject<LoginDto>(loginResp).Token;

            var req = new HttpRequestMessage(HttpMethod.Delete, "zv/users/bobby")
            {
                Headers = {{"Authorization", $"Basic {token}"}}
            };

            var result = await _client.SendAsync(req);

            Assert.Equal(HttpStatusCode.NoContent, result.StatusCode);
        }
    }
}