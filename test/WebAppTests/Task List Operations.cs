using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Framework;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using WebAppTests.Shared;
using Xunit;

namespace WebAppTests
{
    public class TaskListOpsTests : IClassFixture<TaskListOpsTests.Fixture>
    {
        private readonly TestsFixture _fxt;

        public TaskListOpsTests(Fixture fxt)
        {
            _fxt = fxt;
        }

        public class Fixture : TestsFixture
        {
            public Fixture()
            {
                TestData.SeedUsers(Services.GetRequiredService<IMongoDatabase>());
            }
        }

        [OrderedFact("Should create a new task list with the required fields only")]
        public async Task Should_Create_New_TaskList()
        {
            string mutation = @"mutation {
                createList(
                    owner: ""Poulad1024"",
                    list: {
                        title: ""ToDo List""
                    }
                )
                { id owner title description collaborators tags createdAt updatedAt tasks { id } }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        createList: {{
                            id: ""todo-list"",
                            owner: ""poulad1024"",
                            title: ""ToDo List"",
                            description: null,
                            collaborators: null,
                            tags: null,
                            createdAt: ""{DateTime.UtcNow:yyyy-MM-dd}"",
                            updatedAt: null,
                            tasks: [ ]
                        }}
                    }}
                }}",
                responseContent
            );
        }

        [OrderedTheory("Should create a new task list with valid ID")]
        [InlineData("foo.BAR")]
        [InlineData("A1")]
        [InlineData("z_")]
        [InlineData("z-d")]
        [InlineData("f-3")]
        public async Task Should_Create_New_TaskList_ID(string id)
        {
            string mutation = $@"
            mutation {{
                createList(
                    owner: ""Poulad1024"",
                    list: {{
                        id: ""{id}"",
                        title: ""Test List Title""
                    }}
                )
                {{ id owner title description collaborators tags createdAt updatedAt tasks {{ id }} }}
            }}";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        createList: {{
                            id: ""{id.ToLower()}"",
                            owner: ""poulad1024"",
                            title: ""Test List Title"",
                            description: null,
                            collaborators: null,
                            tags: null,
                            createdAt: ""{DateTime.UtcNow:yyyy-MM-dd}"",
                            updatedAt: null,
                            tasks: [ ]
                        }}
                    }}
                }}",
                responseContent
            );
        }

        [OrderedFact("Should query a task list by its ID")]
        public async Task Should_Query_Single_TaskList()
        {
            string query = @"
            query {
                user(userId: ""Poulad1024"") {
                    list(listId: ""todo-list"") {
                        id owner title description collaborators tags createdAt updatedAt tasks { id }
                    }
                }
            }
            ";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(query);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        user: {{
                            list: {{
                                id: ""todo-list"",
                                owner: ""poulad1024"",
                                title: ""ToDo List"",
                                description: null,
                                collaborators: null,
                                tags: null,
                                createdAt: ""{DateTime.UtcNow:yyyy-MM-dd}"",
                                updatedAt: null,
                                tasks: [ ]
                            }}
                        }}
                    }}
                }}",
                responseContent
            );
        }

        [OrderedFact("Should fail when querying a non-existing task list")]
        public async Task Should_Fail_Query_NonExisting_TaskList()
        {
            string query = @"
            query {
                user(userId: ""Poulad1024"") {
                    list(listId: ""NOT.a.List"") {
                        id owner title description collaborators tags createdAt updatedAt tasks { id }
                    }
                }
            }
            ";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(query);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(
                @"{
                    data: { user: { list: null } },
                    errors: [ { message: ""Task List not found."", path: [ ""user.list"" ] } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should fail when creating task list for a non-existing user")]
        public async Task Should_Fail_Create_TaskList_For_NonExisting_User()
        {
            string mutation = @"mutation {
                createList(
                    owner: ""NOT.FOUND_user"",
                    list: { id: ""some.list"", title: ""TITLE HERE"" }
                )
                { id owner title createdAt }
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(
                @"{
                    data: { createList: null },
                    errors: [ { message: ""User ID not found."" } ]
                }",
                responseContent
            );
        }

        [OrderedTheory("Should fail when creating task list with invalid ID")]
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
        public async Task Should_Fail_Create_TaskList_Invalid_ID(string id)
        {
            string mutation = $@"
            mutation {{
                createList(
                    owner: ""Poulad1024"",
                    list: {{
                        id: ""{id}"",
                        title: ""Won't Happen!""
                    }}
                )
                {{ id }}
            }}";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            Asserts.JsonEqual(
                @"{
                    data: { createList: null },
                    errors: [ { message: ""Invalid List ID."", path: [ ""list"" ] } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should fail when deleting a non-existing task list")]
        public async Task Should_Fail_Delete_NonExisting_TaskList()
        {
            string mutation = @"mutation {
                deleteList(
                    owner: ""poulad1024"",
                    list: ""NOT.FOUND_list""
                )
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(
                @"{
                    data: { deleteList: false },
                    errors: [ { message: ""Task list not found."" } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should fail when deleting a task list for a non-existing user")]
        public async Task Should_Fail_Delete_TaskList_For_NonExisting_User()
        {
            string mutation = @"mutation {
                deleteList(
                    owner: ""Random-user1"",
                    list: ""my_list""
                )
            }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(
                @"{
                    data: { deleteList: false },
                    errors: [ { message: ""Task list not found."" } ]
                }",
                responseContent
            );
        }

        [OrderedFact("Should delete a task list")]
        public async Task Should_Delete_TaskList()
        {
            string mutation = @"mutation { deleteList( owner: ""poulad1024"", list: ""todo-list"" ) }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(@"{ data: { deleteList: true } }", responseContent);
        }
    }
}
