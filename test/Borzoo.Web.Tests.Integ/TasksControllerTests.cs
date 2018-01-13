using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Web.Models.Task;
using Borzoo.Web.Tests.Integ.Framework;
using Newtonsoft.Json;
using Xunit;

namespace Borzoo.Web.Tests.Integ
{
    public class TasksControllerTests : IClassFixture<TasksControllerTests.Fixture>
    {
        private HttpClient Client => _fixture.Client;

        private readonly Fixture _fixture;

        public TasksControllerTests(Fixture fixture)
        {
            _fixture = fixture;
        }

        [OrderedFact]
        public async Task Should_Create_Task()
        {
            const string title = "test title";
            string json = $@"{{
                ""title"": ""{title}""
            }}";

            var request = new HttpRequestMessage(HttpMethod.Post, $"/zv/users/{_fixture.UserName}/tasks")
            {
                Headers =
                {
                    {"Authorization", "Basic " + _fixture.AuthToken},
                    {"Accept", "application/vnd.zv.task.full+json"}
                },
                Content = new StringContent(json, Encoding.UTF8, "application/vnd.zv.task.creation+json"),
            };
            var response = await Client.SendAsync(request);

            var payload = await response.Content.ReadAsStringAsync();
            var taskFullDto = JsonConvert.DeserializeObject<TaskFullDto>(payload);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotEmpty(taskFullDto.Id);
            Assert.Equal(title, taskFullDto.Title);
            Assert.InRange(taskFullDto.CreatedAt, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow.AddSeconds(10));
            Assert.Null(taskFullDto.Description);
            Assert.Equal(default, taskFullDto.DueBy);
        }

        [OrderedFact]
        public async Task Should_Create_Task_PrettyDto()
        {
            const string title = "test title2";
            const string description = "test description";
            string due = DateTime.Now.AddSeconds(70).ToString("O");
            string json = $@"{{
                ""title"": ""{title}"",
                ""description"": ""{description}"",
                ""due_by"": ""{due}""
            }}";

            var request = new HttpRequestMessage(HttpMethod.Post, $"/zv/users/{_fixture.UserName}/tasks")
            {
                Headers =
                {
                    {"Authorization", "Basic " + _fixture.AuthToken},
                    {"Accept", "application/vnd.zv.task.pretty+json"}
                },
                Content = new StringContent(json, Encoding.UTF8, "application/vnd.zv.task.creation+json"),
            };
            var response = await Client.SendAsync(request);

            var payload = await response.Content.ReadAsStringAsync();
            var taskPrettyDto = JsonConvert.DeserializeObject<TaskPrettyDto>(payload);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.NotEmpty(taskPrettyDto.Id);
            Assert.Equal(title, taskPrettyDto.Title);
            Assert.Equal(description, taskPrettyDto.Description);
            Assert.Equal(false, taskPrettyDto.IsDue);
            Assert.Equal("1 minute", taskPrettyDto.DueIn);
        }

        public class Fixture : AuthorizedClientFixtureBase
        {
        }
    }
}