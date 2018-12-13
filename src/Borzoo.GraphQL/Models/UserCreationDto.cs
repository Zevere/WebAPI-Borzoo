using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    /// <summary>
    /// DTO object for creating a new user
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserCreationDto
    {
        /// <summary>
        /// Unique username
        /// </summary>
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Account passphrase in clear text
        /// </summary>
        [Required]
        [MinLength(8)]
        [JsonProperty(Required = Required.Always)]
        public string Passphrase { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string FirstName { get; set; }

        /// <summary>
        /// Optional last name
        /// </summary>
        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LastName { get; set; }

        /// <summary>
        /// If an organization account, usernames of the organization members
        /// </summary>
        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] Members { get; set; }

        /// <summary>
        /// Converts the DTO into a <see cref="User"/> entity
        /// </summary>
        public static explicit operator User(UserCreationDto dtoModel) =>
            dtoModel is null
                ? null
                : new User
                {
                    DisplayId = dtoModel.Name,
                    FirstName = dtoModel.FirstName,
                    LastName = dtoModel.LastName,
                    PassphraseHash = dtoModel.Passphrase
                };
    }
}
