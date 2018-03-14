using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(MemberSerialization.Fields, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskListDto
    {
        public string Id;

        public string Owner;
        
        public string Title;

        public DateTime CreatedAt;

        public static explicit operator TaskListDto(TaskList tl) =>
            tl is null
                ? null
                : new TaskListDto
                {
                    Id = tl.DisplayId,
                    Owner = tl.OwnerId,
                    Title = tl.Title,
                    CreatedAt = tl.CreatedAt
                };
    }
}