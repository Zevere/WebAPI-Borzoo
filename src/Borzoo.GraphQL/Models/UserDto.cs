using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    /// <summary>
    /// DTO object for a user account
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class UserDto
    {
        /// <summary>
        /// Unique username
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Unique authentication token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Number of the days since account creation
        /// </summary>
        public int DaysJoined { get; set; }

        /// <summary>
        /// Time of account creation
        /// </summary>
        public DateTime JoinedAt { get; set; }

        /// <summary>
        /// Converts an <see cref="User"/> into an <see cref="UserDto"/>
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
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
