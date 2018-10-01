using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskItemDto
    {
        public string Id { get; set; }

        public string List { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime? Due { get; set; }

        public string[] Tags { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public static explicit operator TaskItemDto(TaskItem ti) =>
            ti is null
                ? null
                : new TaskItemDto
                {
                    Id = ti.DisplayId,
                    List = ti.ListId,
                    Title = ti.Title,
                    Description = ti.Description,
                    Due = ti.Due,
                    Tags = ti.Tags,
                    CreatedAt = ti.CreatedAt
                };
    }
}