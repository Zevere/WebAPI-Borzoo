using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Models.User
{
    [JsonObject(MemberSerialization.OptOut, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class UserCreationDto : UserDtoBase
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

        public static explicit operator UserEntity(UserCreationDto dtoModel) =>
            dtoModel is default
                ? default
                : new UserEntity
                {
                    DisplayId = dtoModel.Name,
                    FirstName = dtoModel.FirstName,
                    LastName = dtoModel.LastName,
                    PassphraseHash = dtoModel.Passphrase
                };
    }
}