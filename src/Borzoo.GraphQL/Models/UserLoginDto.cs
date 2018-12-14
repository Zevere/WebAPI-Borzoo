using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    /// <summary>
    /// DTO object for user login
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserLoginDto
    {
        /// <summary>
        /// Username
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Username { get; set; }

        /// <summary>
        /// Passphrase
        /// </summary>
        [Required]
        [MinLength(8)]
        public string Passphrase { get; set; }
    }
}
