using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Framework;
using Framework.Extensions;
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
