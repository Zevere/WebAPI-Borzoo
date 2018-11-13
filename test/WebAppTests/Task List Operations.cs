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
                        id: ""todo_list"",
                        title: ""A ToDo List""
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
                            id: ""todo_list"",
                            owner: ""poulad1024"",
                            title: ""A ToDo List"",
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
            string mutation = @"mutation { deleteList( owner: ""poulad1024"", list: ""todo_list"" ) }";

            HttpResponseMessage response = await _fxt.HttpClient.PostGraphqlAsync(mutation);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(@"{ data: { deleteList: true } }", responseContent);
        }
    }
}
