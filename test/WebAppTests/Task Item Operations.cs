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
                    ownerId: ""Poulad1024"",
                    listId: ""a_test_list"",
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

        [OrderedFact("Should query task items in a list")]
        public async Task Should_Query_TaskItems()
        {
            string query = @"
            query {
                user(userId: ""Poulad1024"") {
                    list(listId: ""a_test_list"") {
                        tasks { id title description due tags createdAt }
                    }
                }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(query);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        user: {{
                            list: {{
                                tasks: [
                                    {{
                                        id: ""do-something"",
                                        title: ""Do Something!"",
                                        description: null,
                                        due: null,
                                        tags: null,
                                        createdAt: ""{DateTime.UtcNow:yyyy-MM-dd}""
                                    }}
                                ]
                            }}
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
                    ownerId: ""Poulad1024"",
                    listId: ""a_test_list"",
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
                    ownerId: ""Poulad1024"",
                    listId: ""a_test_list"",
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

        [OrderedFact("Should fail when creating task item with invalid owner ID")]
        public async Task Should_Fail_Create_TaskItem_Invalid_OwnerID()
        {
            string mutation = @"
            mutation {
                createTask(
                    ownerId: ""_invalid"",
                    listId: ""whatever"",
                    task: { title: ""foo"" }
                ) { id }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                @"{
                    data: { createTask: null },
                    errors: [ { message: ""Invalid owner ID."", path: [ ""createTask"" ] } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should fail when creating task item with invalid list ID")]
        public async Task Should_Fail_Create_TaskItem_Invalid_ListID()
        {
            string mutation = @"
            mutation {
                createTask(
                    ownerId: ""franky"",
                    listId: ""...wrong..."",
                    task: { title: ""title"" }
                ) { id }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                @"{
                    data: { createTask: null },
                    errors: [ { message: ""Invalid list ID."", path: [ ""createTask"" ] } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should fail when creating task item for a non-existing list")]
        public async Task Should_Fail_Create_TaskItem_NonExisting_List()
        {
            string mutation = @"
            mutation {
                createTask(
                    ownerId: ""franky"",
                    listId: ""not.found"",
                    task: { title: ""title"" }
                ) { id }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                @"{
                    data: { createTask: null },
                    errors: [ { message: ""Task list not found."", path: [ ""createTask"" ] } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should fail when deleting a non-existing task item")]
        public async Task Should_Fail_Delete_NonExisting_TaskItem()
        {
            string mutation = @"
            mutation {
                deleteTask(
                    ownerId: ""franky"",
                    listId: ""a_test_list"",
                    taskId: ""NON.existing""
                )
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(
                @"{
                    data: { deleteTask: false },
                    errors: [ { message: ""Task item not found."" } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should delete a task item")]
        public async Task Should_Delete_TaskItem()
        {
            string mutation = @"
               mutation {
                deleteTask(
                    ownerId: ""poulad1024"",
                    listId: ""a_test_list"",
                    taskId: ""do-something""
                )
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(@"{ data: { deleteTask: true } }", responseContent);
        }
    }
}
