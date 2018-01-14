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
            Assert.True(Guid.TryParse(taskFullDto.Id, out var _));
            Assert.Equal(title, taskFullDto.Title);
            Assert.InRange(taskFullDto.CreatedAt, DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow.AddSeconds(10));
            Assert.Null(taskFullDto.Description);
            Assert.Equal(default, taskFullDto.DueBy);
        }

        [OrderedFact]
        public async Task Should_Create_Task_PrettyDto()
        {
            const string id = "my.test_task-2";
            const string title = "test title2";
            const string description = "test description";
            string due = DateTime.Now.AddSeconds(70).ToString("O");
            string json = $@"{{
                ""id"": ""{id}"",
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
            Assert.Equal(id, taskPrettyDto.Id);
            Assert.Equal(title, taskPrettyDto.Title);
            Assert.Equal(description, taskPrettyDto.Description);
            Assert.Equal(false, taskPrettyDto.IsDue);
            Assert.Equal("1 minute", taskPrettyDto.DueIn);

            _fixture.TaskId = taskPrettyDto.Id;
        }

        [OrderedFact]
        public async Task Should_Respond400_Creating_Invalid_Task_Id()
        {
            const string title = "test-title";
            string json = $@"{{
                ""id"": ""a@b"",
                ""title"": ""{title}""
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

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [OrderedFact]
        public async Task Should_Respond409_Creating_Same_Task_Id()
        {
            string id = _fixture.TaskId;
            string json = $@"{{
                ""id"": ""{id}"",
                ""title"": ""test title""
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

            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [OrderedFact]
        public async Task Should_Get_Task_Existing_Id()
        {
            string id = _fixture.TaskId;

            var request = new HttpRequestMessage(HttpMethod.Head, $"/zv/users/{_fixture.UserName}/tasks/{id}")
            {
                Headers = {{"Authorization", "Basic " + _fixture.AuthToken}}
            };
            var response = await Client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Empty(content);
        }

        [OrderedFact]
        public async Task Should_Respond404_Getting_NonExisting_Task_Id()
        {
            var request = new HttpRequestMessage(HttpMethod.Head, $"/zv/users/{_fixture.UserName}/tasks/task404")
            {
                Headers = {{"Authorization", "Basic " + _fixture.AuthToken}}
            };
            var response = await Client.SendAsync(request);
            string content = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Empty(content);
        }

        public class Fixture : AuthorizedClientFixtureBase
        {
            public string TaskId { get; set; }
        }
    }
}