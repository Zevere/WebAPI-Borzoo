using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserLoginDto
    {
        [Required]
        [MinLength(1)]
        public string Username { get; set; }

        [Required]
        [MinLength(8)]
        public string Passphrase { get; set; }
    }
}