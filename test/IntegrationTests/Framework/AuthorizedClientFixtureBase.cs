using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Borzoo.Web;
using Borzoo.Web.Models.Login;
using Newtonsoft.Json;

namespace IntegrationTests.Framework
{
    public class AuthorizedClientFixtureBase : TestHostFixture<Startup>
    {
        public readonly string UserName;

        public string AuthToken { get; }

        public AuthorizedClientFixtureBase()
        {
            UserName = "bobby";
            AuthToken = GetAuthTokenAsync(UserName, "secret_passphrase2").GetAwaiter().GetResult();
        }

        private async Task<string> GetAuthTokenAsync(string user, string pass)
        {
            var login = await Client.PostAsync("/zv/login", new StringContent(
                $@"{{""user_name"":""{user}"",""passphrase"":""{pass}""}}", Encoding.UTF8,
                "application/vnd.zv.login.creation+json"
            ));
            string loginResp = await login.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<LoginDto>(loginResp).Token;
        }
    }
}