using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserDto
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Token { get; set; }

        public int DaysJoined { get; set; }

        public DateTime JoinedAt { get; set; }

        public static explicit operator UserDto(User entity) =>
            entity is null
                ? null
                : new UserDto
                {
                    Id = entity.DisplayId?.ToLower(),
                    FirstName = entity.FirstName,
                    LastName = entity.LastName,
                    Token = entity.Token,
                    DaysJoined = (DateTime.UtcNow - entity.JoinedAt).Days,
                    JoinedAt = entity.JoinedAt
                };
    }
}