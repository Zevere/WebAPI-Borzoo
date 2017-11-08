using System.ComponentModel.DataAnnotations;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;
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
        [Required]
        [MinLength(1)]
        public string Name { get; set; }

        [Required]
        [MinLength(8)]
        public string Passphrase { get; set; }

        [Required]
        [MinLength(1)]
        public string FirstName { get; set; }

        [MinLength(1)]
        public string LastName { get; set; }

        public static explicit operator UserEntity(UserCreationRequest requestModel)
        {
            return new UserEntity
            {
                DisplayId = requestModel.Name,
                FirstName = requestModel.FirstName,
                LastName = requestModel.LastName,
                PassphraseHash = requestModel.Passphrase
            };
        }
    }
}