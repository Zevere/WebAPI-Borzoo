using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Framework;
using Framework.Extensions;
using Newtonsoft.Json;
using WebAppTests.Shared;
using Xunit;

namespace WebAppTests
{
    [Collection("user operations")]
    public class UserGraphTests : IClassFixture<TestsFixture>
    {
        private readonly TestsFixture _fxt;

        public UserGraphTests(TestsFixture fxt)
        {
            _fxt = fxt;
        }

        [OrderedFact("Should create a new user with the required fields only")]
        public async Task Should_Create_New_User()
        {
            string mutation = @"mutation SomeMutation($u: UserInput!) {
                createUser(user: $u) {
                    id firstName lastName daysJoined joinedAt token
                    lists { id }
                }
            }";
            HttpResponseMessage response = await _fxt.HttpClient.PostJsonGraphqlAsync($@"{{
                ""query"": ""{mutation.Stringify()}"",
                ""variables"": {{
                    ""u"": {{
                        ""name"": ""POULAD1024"",
                        ""firstName"": ""Poulad"",
                        ""passphrase"": ""10-pass_phrase-24""
                    }}
                }}
            }}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.IsJson(responseContent);

            dynamic result = JsonConvert.DeserializeObject(responseContent);

            string token = result.data.createUser.token;
            Assert.NotEmpty(token);

            Asserts.JsonEqual(
                $@"{{
                    data: {{
                        createUser: {{
                            id: ""poulad1024"",
                            firstName: ""Poulad"",
                            lastName: null,
                            daysJoined: 0,
                            joinedAt: ""{DateTime.Today:yyyy-MM-dd}"",
                            token: ""{token}"",
                            lists: [ ]
                        }}
                    }}
                }}",
                responseContent
            );
        }

        [OrderedFact("Should fail when querying non-existing user")]
        public async Task Should_Fail_Query_NonExisting_User()
        {
            string query = @"query { user(userId: ""alice0"") { id } }";
            HttpResponseMessage response = await _fxt.HttpClient.PostJsonGraphqlAsync(
                $@"{{ ""query"": ""{query.Stringify()}"" }}"
            );
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            string responseContent = await response.Content.ReadAsStringAsync();
            Asserts.JsonEqual(
                @"{
                    data: { user: null },
                    errors: [
                        {
                            message: ""not found"",
                            path: [ ""user"" ]
                        }
                    ]
                }",
                responseContent
            );
        }
    }
}
