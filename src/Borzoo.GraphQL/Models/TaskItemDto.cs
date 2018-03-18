using System;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(MemberSerialization.Fields, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskItemDto
    {
        public string Id;

        public string List;

        public string Title;

        public string Description;

        public DateTime? Due;

        public DateTime CreatedAt;

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
                    CreatedAt = ti.CreatedAt
                };
    }
}