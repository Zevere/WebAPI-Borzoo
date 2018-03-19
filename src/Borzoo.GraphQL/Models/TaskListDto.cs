using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskListDto
    {
        public string Id { get; set; }

        public string Owner { get; set; }

        public string Title { get; set; }

        public DateTime CreatedAt { get; set; }

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