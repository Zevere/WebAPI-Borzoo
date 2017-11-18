using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models.Login
{
    [JsonObject(
        MemberSerialization.OptOut,
        NamingStrategyType = typeof(SnakeCaseNamingStrategy)
    )]
    public class LoginRequestDto
    {
        [Required]
        [MinLength(1)]
        public string UserName { get; set; }

        [Required]
        [MinLength(8)]
        public string Passphrase { get; set; }
    }
}