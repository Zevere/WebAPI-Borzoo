using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Models.User
{
    [JsonObject(MemberSerialization.OptOut, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class UserPrettyDto : UserDtoBase
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
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