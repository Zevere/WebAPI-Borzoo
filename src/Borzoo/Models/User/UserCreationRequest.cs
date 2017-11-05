using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Models.User
{
    [JsonObject(
        MemberSerialization.OptOut,
        NamingStrategyType = typeof(SnakeCaseNamingStrategy)
    )]
    public class UserCreationRequest : UserDtoBase
    {
        public string Name { get; set; }

        public string Passphrase { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}