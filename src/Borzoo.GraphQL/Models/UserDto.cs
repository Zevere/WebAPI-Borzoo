using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(MemberSerialization.Fields, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserDto
    {
        public string Id;

        public string FirstName;

        public string LastName;

        public string Token;

        public int DaysJoined;

        public DateTime JoinedAt;

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