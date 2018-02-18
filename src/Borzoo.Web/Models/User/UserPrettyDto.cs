using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Models.User
{
    [Obsolete]
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class UserPrettyDto : UserDtoBase
    {
        [Required]
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [Required]
        [JsonProperty(Required = Required.Always)]
        public string DisplayName { get; set; }

        [Required]
        [JsonProperty(Required = Required.Always)]
        public int DaysJoined { get; set; }

        public static explicit operator UserPrettyDto(UserEntity entity) =>
            entity is default
                ? default
                : new UserPrettyDto
                {
                    Id = entity.DisplayId,
                    DisplayName = $"{entity.FirstName} {entity.LastName}".Trim(),
                    DaysJoined = (DateTime.UtcNow - entity.JoinedAt).Days
                };
    }
}