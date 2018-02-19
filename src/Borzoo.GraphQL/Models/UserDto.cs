using System;
using UserEntity = Borzoo.Data.Abstractions.Entities.User;

namespace Borzoo.GraphQL.Models
{
    public class UserDto
    {
        public string Id;

        public string FirstName;

        public string LastName;

        public string DisplayName;

        public int DaysJoined;

        public DateTime JoinedAt;

        public static explicit operator UserDto(UserEntity entity) =>
            new UserDto
            {
                Id = entity.DisplayId,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                DisplayName = $"{entity.FirstName} {entity.LastName}".Trim(),
                DaysJoined = (DateTime.UtcNow - entity.JoinedAt).Days,
                JoinedAt = entity.JoinedAt
            };
    }
}