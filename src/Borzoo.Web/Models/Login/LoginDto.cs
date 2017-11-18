using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models.Login
{
    [JsonObject(
        MemberSerialization.OptOut,
        NamingStrategyType = typeof(SnakeCaseNamingStrategy)
    )]
    public class LoginDto
    {
        public LoginDto()
        {
        }

        public LoginDto(string token)
        {
            Token = token;
        }
        
        [Required]
        public string Token { get; set; }
    }
}