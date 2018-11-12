using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebAppTests.Shared
{
    public static class Extensions
    {
        public static Task<HttpResponseMessage> PostJsonGraphqlAsync(
            this HttpClient client,
            string json
        ) =>
            client.PostAsync(
                "/zv/graphql",
                new StringContent(json, Encoding.UTF8, "application/json")
            );
    }
}
