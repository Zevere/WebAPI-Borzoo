//using System;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using Borzoo.Tests.Framework;
//using Borzoo.Web.Models.Task;
//using Borzoo.Web.Tests.Integ.Framework;
//using Newtonsoft.Json;
//using Xunit;
//
//namespace Borzoo.Web.Tests.Integ
//{
//    public class TasksControllerTests : IClassFixture<TasksControllerTests.Fixture>
//    {
//        private HttpClient Client => _fixture.Client;
//
//        private readonly Fixture _fixture;
//
//        public TasksControllerTests(Fixture fixture)
//        {
//            _fixture = fixture;
//        }
//
//        [OrderedFact]
//        public async Task Should_Create_Task_PrettyDto()
//        {
//            const string title = "test title";
//            string json = $@"{{
//                ""title"": ""{title}""
//            }}";
//
//            var request = new HttpRequestMessage(HttpMethod.Post, $"/zv/users/{_fixture.UserName}/tasks")
//            {
//                Headers =
//                {
//                    {"Authorization", "Basic " + _fixture.AuthToken},
//                    {"Accept", "application/vnd.zv.task.pretty+json"}
//                },
//                Content = new StringContent(json, Encoding.UTF8, "application/vnd.zv.task.creation+json"),
//            };
//            var response = await Client.SendAsync(request);
//
//            var payload = await response.Content.ReadAsStringAsync();
//            var taskPrettyDto = JsonConvert.DeserializeObject<TaskPrettyDto>(payload);
//
//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//            Assert.True(Guid.TryParse(taskPrettyDto.Id, out var _));
//            Assert.Equal(title, taskPrettyDto.Title);
//            Assert.Null(taskPrettyDto.Description);
//            Assert.Equal(default, taskPrettyDto.DueIn);
//        }
//
//        [OrderedFact]
//        public async Task Should_Create_Task_FullDto()
//        {
//            const string id = "my.test_task-2";
//            const string title = "test title2";
//            const string description = "test description";
//            string due = DateTime.Now.AddSeconds(70).ToString("O");
//            string json = $@"{{
//                ""id"": ""{id}"",
//                ""title"": ""{title}"",
//                ""description"": ""{description}"",
//                ""due_by"": ""{due}""
//            }}";
//
//            var request = new HttpRequestMessage(HttpMethod.Post, $"/zv/users/{_fixture.UserName}/tasks")
//            {
//                Headers =
//                {
//                    {"Authorization", "Basic " + _fixture.AuthToken},
//                    {"Accept", "application/vnd.zv.task.full+json"}
//                },
//                Content = new StringContent(json, Encoding.UTF8, "application/vnd.zv.task.creation+json"),
//            };
//            var response = await Client.SendAsync(request);
//
//            var payload = await response.Content.ReadAsStringAsync();
//            dynamic fullDto = JsonConvert.DeserializeObject(payload);
//
//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//            Assert.Equal(id, (string) fullDto.id);
//            Assert.Equal(title, (string) fullDto.title);
//            Assert.Equal(description, (string) fullDto.description);
//            Assert.InRange(
//                DateTime.Parse((string) fullDto.created_at),
//                DateTime.UtcNow.AddSeconds(-10), DateTime.UtcNow.AddSeconds(10)
//            );
//            Assert.InRange(
//                DateTime.Parse((string) fullDto.due_by),
//                DateTime.UtcNow, DateTime.UtcNow.AddSeconds(71)
//            );
//
//            _fixture.Task = JsonConvert.DeserializeObject<TaskFullDto>(payload);
//        }
//
//        [OrderedFact]
//        public async Task Should_Respond400_Creating_Invalid_Task_Id()
//        {
//            const string title = "test-title";
//            string json = $@"{{
//                ""id"": ""a@b"",
//                ""title"": ""{title}""
//            }}";
//
//            var request = new HttpRequestMessage(HttpMethod.Post, $"/zv/users/{_fixture.UserName}/tasks")
//            {
//                Headers =
//                {
//                    {"Authorization", "Basic " + _fixture.AuthToken},
//                    {"Accept", "application/vnd.zv.task.pretty+json"}
//                },
//                Content = new StringContent(json, Encoding.UTF8, "application/vnd.zv.task.creation+json"),
//            };
//            var response = await Client.SendAsync(request);
//
//            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
//        }
//
//        [OrderedFact]
//        public async Task Should_Respond409_Creating_Same_Task_Id()
//        {
//            string id = _fixture.Task.Id;
//            string json = $@"{{
//                ""id"": ""{id}"",
//                ""title"": ""test title""
//            }}";
//
//            var request = new HttpRequestMessage(HttpMethod.Post, $"/zv/users/{_fixture.UserName}/tasks")
//            {
//                Headers =
//                {
//                    {"Authorization", "Basic " + _fixture.AuthToken},
//                    {"Accept", "application/vnd.zv.task.pretty+json"}
//                },
//                Content = new StringContent(json, Encoding.UTF8, "application/vnd.zv.task.creation+json"),
//            };
//            var response = await Client.SendAsync(request);
//
//            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
//        }
//
//        [OrderedFact]
//        public async Task Should_Get_Task_Existing_Id()
//        {
//            string id = _fixture.Task.Id;
//
//            var request = new HttpRequestMessage(HttpMethod.Head, $"/zv/users/{_fixture.UserName}/tasks/{id}")
//            {
//                Headers = {{"Authorization", "Basic " + _fixture.AuthToken}}
//            };
//            var response = await Client.SendAsync(request);
//            string content = await response.Content.ReadAsStringAsync();
//
//            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
//            Assert.Empty(content);
//        }
//
//        [OrderedFact]
//        public async Task Should_Respond404_Checking_NonExisting_Task_Id()
//        {
//            var request = new HttpRequestMessage(HttpMethod.Head, $"/zv/users/{_fixture.UserName}/tasks/task404")
//            {
//                Headers = {{"Authorization", "Basic " + _fixture.AuthToken}}
//            };
//            var response = await Client.SendAsync(request);
//            string content = await response.Content.ReadAsStringAsync();
//
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//            Assert.Empty(content);
//        }
//
//        [OrderedFact]
//        public async Task Should_Get_Tasks_FullDto()
//        {
//            var request = new HttpRequestMessage(HttpMethod.Get, $"/zv/users/{_fixture.UserName}/tasks")
//            {
//                Headers =
//                {
//                    {"Authorization", "Basic " + _fixture.AuthToken},
//                    {"Accept", "application/vnd.zv.task.full+json"}
//                }
//            };
//            var response = await Client.SendAsync(request);
//
//            var payload = await response.Content.ReadAsStringAsync();
//            var tasks = JsonConvert.DeserializeObject<TaskFullDto[]>(payload);
//
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            Assert.NotEmpty(tasks);
//            var _ = tasks.Single(t => t.Id == _fixture.Task.Id);
//            Assert.Collection(tasks,
//                dto => Assert.NotEmpty(dto.Id),
//                dto => Assert.NotEmpty(dto.Title)
//            );
//        }
//
//        [OrderedFact]
//        public async Task Should_Get_Task_PrettyDto()
//        {
//            var request =
//                new HttpRequestMessage(HttpMethod.Get, $"/zv/users/{_fixture.UserName}/tasks/{_fixture.Task.Id}")
//                {
//                    Headers =
//                    {
//                        {"Authorization", "Basic " + _fixture.AuthToken},
//                        {"Accept", "application/vnd.zv.task.pretty+json"}
//                    }
//                };
//            var response = await Client.SendAsync(request);
//
//            var payload = await response.Content.ReadAsStringAsync();
//            dynamic taskPretyDto = JsonConvert.DeserializeObject(payload);
//
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            Assert.Equal(_fixture.Task.Id, (string) taskPretyDto.id);
//            Assert.Equal(_fixture.Task.Title, (string) taskPretyDto.title);
//            Assert.Equal(_fixture.Task.Description, (string) taskPretyDto.description);
//            Assert.False((bool) taskPretyDto.is_due);
//            Assert.NotEmpty((string) taskPretyDto.due_in);
//        }
//
//        [OrderedFact]
//        public async Task Should_Get_Task_FullDto()
//        {
//            var request =
//                new HttpRequestMessage(HttpMethod.Get, $"/zv/users/{_fixture.UserName}/tasks/{_fixture.Task.Id}")
//                {
//                    Headers =
//                    {
//                        {"Authorization", "Basic " + _fixture.AuthToken},
//                        {"Accept", "application/vnd.zv.task.full+json"}
//                    }
//                };
//            var response = await Client.SendAsync(request);
//
//            var payload = await response.Content.ReadAsStringAsync();
//            dynamic fullDto = JsonConvert.DeserializeObject(payload);
//
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            Assert.Equal(_fixture.Task.Id, (string) fullDto.id);
//            Assert.Equal(_fixture.Task.Title, (string) fullDto.title);
//            Assert.Equal(_fixture.Task.Description, (string) fullDto.description);
//            Assert.InRange(
//                DateTime.Parse((string) fullDto.created_at),
//                DateTime.UtcNow.AddSeconds(-20), DateTime.UtcNow
//            );
//            Assert.InRange(
//                DateTime.Parse((string) fullDto.due_by),
//                DateTime.UtcNow, DateTime.UtcNow.AddSeconds(71)
//            );
//        }
//
//        [OrderedFact]
//        public async Task Should_Respond404_Getting_NonExisting_Task_Id()
//        {
//            var request = new HttpRequestMessage(HttpMethod.Get, $"/zv/users/{_fixture.UserName}/tasks/task404")
//            {
//                Headers = {{"Authorization", "Basic " + _fixture.AuthToken}}
//            };
//            var response = await Client.SendAsync(request);
//            string content = await response.Content.ReadAsStringAsync();
//
//            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
//            Assert.Empty(content);
//        }
//
//        [OrderedFact(Skip = "Delete all tasks, and then run this")]
//        public async Task Should_Get_Empty_Tasks()
//        {
//            var request = new HttpRequestMessage(HttpMethod.Get, $"/zv/users/{_fixture.UserName}/tasks")
//            {
//                Headers =
//                {
//                    {"Authorization", "Basic " + _fixture.AuthToken},
//                    {"Accept", "application/vnd.zv.task.full+json"}
//                }
//            };
//            var response = await Client.SendAsync(request);
//
//            var payload = await response.Content.ReadAsStringAsync();
//            var tasks = JsonConvert.DeserializeObject<TaskFullDto[]>(payload);
//
//            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
//            Assert.Empty(tasks);
//        }
//
//        // ReSharper disable once ClassNeverInstantiated.Global
//        public class Fixture : AuthorizedClientFixtureBase
//        {
//            public TaskFullDto Task { get; set; }
//        }
//    }
//}