using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Models.User
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserCreationDto
    {
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [Required]
        [MinLength(8)]
        [JsonProperty(Required = Required.Always)]
        public string Passphrase { get; set; }

        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string FirstName { get; set; }

        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LastName { get; set; }

        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string[] Members { get; set; }

        public static explicit operator UserEntity(UserCreationDto dtoModel) =>
            dtoModel is null
                ? null
                : new UserEntity
                {
                    DisplayId = dtoModel.Name,
                    FirstName = dtoModel.FirstName,
                    LastName = dtoModel.LastName,
                    PassphraseHash = dtoModel.Passphrase
                };
    }
}