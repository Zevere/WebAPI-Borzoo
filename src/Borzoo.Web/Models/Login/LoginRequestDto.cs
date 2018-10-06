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
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Passphrase is required")]
        [MinLength(8, ErrorMessage = "Passphrase should be at least 8 characters long")]
        public string Passphrase { get; set; }
    }
}