using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Framework;
using Framework.Extensions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebAppTests.Shared;
using Xunit;

namespace WebAppTests
{
    public class TaskItemOpsTests : IClassFixture<TaskItemOpsTests.Fixture>
    {
        private readonly TestsFixture _fxt;

        public TaskItemOpsTests(Fixture fxt)
        {
            _fxt = fxt;
        }

        public class Fixture : TestsFixture
        {
            public Fixture()
            {
                IMongoDatabase db = Services.GetRequiredService<IMongoDatabase>();
                TestData.SeedUsers(db);
                TestData.SeedTaskLists(db);
            }
        }

        [OrderedFact("Should create a new task item with the required fields only")]
        public async Task Should_Create_New_TaskItem()
        {
            string mutation = @"
            mutation TaskCreation($t: TaskInput!) {
                createTask(
                    owner: ""Poulad1024"",
                    list: ""a_test_list"",
                    task: $t
                )
                { id title description due tags createdAt }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostJsonGraphqlAsync($@"{{
                ""query"": ""{mutation.Stringify()}"",
                ""variables"": {{
                    ""t"": {{
                        ""title"": ""Do Something!""
                    }}
                }}
            }}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        createTask: {{
                            id: ""do-something"",
                            title: ""Do Something!"",
                            description: null,
                            due: null,
                            tags: null,
                            createdAt: ""{DateTime.UtcNow:yyyy-MM-dd}"",
                        }}
                    }}
                }}",
                responseContent
            );
        }

        [OrderedTheory("Should create a new task item with valid ID")]
        [InlineData("foo.BAR")]
        [InlineData("A1")]
        [InlineData("z_")]
        [InlineData("z-d")]
        [InlineData("f-3")]
        public async Task Should_Create_New_TaskItem_ID(string id)
        {
            string mutation = $@"
            mutation {{
                createTask(
                    owner: ""Poulad1024"",
                    list: ""a_test_list"",
                    task: {{
                        id: ""{id}"",
                        title: ""A Task of Testing""
                    }}
                )
                {{ id title description due tags createdAt }}
            }}";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        createTask: {{
                            id: ""{id.ToLower()}"",
                            title: ""A Task of Testing"",
                            description: null,
                            due: null,
                            tags: null,
                            createdAt: ""{DateTime.UtcNow:yyyy-MM-dd}"",
                        }}
                    }}
                }}",
                responseContent
            );
        }

        [OrderedTheory("Should fail when creating task item with invalid ID")]
        [InlineData("")]
        [InlineData("p")]
        [InlineData("3")]
        [InlineData(".")]
        [InlineData("-")]
        [InlineData("_")]
        [InlineData("_invalid")]
        [InlineData("2_0")]
        [InlineData("#")]
        [InlineData("val!d")]
        public async Task Should_Fail_Create_TaskItem_Invalid_ID(string id)
        {
            string mutation = $@"
            mutation {{
                createTask(
                    owner: ""Poulad1024"",
                    list: ""a_test_list"",
                    task: {{
                        id: ""{id}"",
                        title: ""THIS IS INVALID!""
                    }}
                )
                {{ id title description due tags createdAt }}
            }}";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                @"{
                    data: { createTask: null },
                    errors: [ { message: ""Invalid Task ID."", path: [ ""createTask"" ] } ]
                }",
                responseContent
            );
        }
    }
}