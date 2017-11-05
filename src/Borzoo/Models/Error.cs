using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Models
{
    [JsonObject(
        MemberSerialization.OptOut,
        NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemRequired = Required.DisallowNull
    )]
    public class Error
    {
        public string Code { get; set; }
        
        public string Message { get; set; }

        public ValidationError[] ValidationErrors { get; set; }
    }
}