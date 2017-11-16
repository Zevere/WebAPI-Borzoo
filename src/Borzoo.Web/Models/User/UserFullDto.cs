using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Models.User
{
    [JsonObject(
        MemberSerialization.OptOut,
        NamingStrategyType = typeof(SnakeCaseNamingStrategy)
    )]
    public class UserFullDto : UserDtoBase
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; } // ToDo Define convertor to proper ISO datetiem JSON representation

        public static explicit operator UserFullDto(UserEntity entity) =>
            new UserFullDto
            {
                Id = entity.DisplayId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                JoinedAt = entity.JoinedAt
            };
    }
}