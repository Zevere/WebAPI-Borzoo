using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models
{
    [JsonObject(
        MemberSerialization.OptOut,
        NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemRequired = Required.DisallowNull
    )]
    public class ValidationError
    {
        public string Field { get; set; }

        public string Message { get; set; }

        public string Hint { get; set; }
    }
}