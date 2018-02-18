using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.Web.Models.User
{
    [Obsolete]
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class UserFullDto : UserDtoBase
    {
        [Required]
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [Required]
        [JsonProperty(Required = Required.Always)]
        public string FirstName { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string LastName { get; set; }

        [Required]
        [JsonProperty(Required = Required.Always)]
        public DateTime JoinedAt { get; set; }

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