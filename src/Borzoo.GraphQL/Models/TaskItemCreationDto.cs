using System;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskItemCreationDto
    {
        [Required] [MinLength(1)] [JsonProperty(Required = Required.Always)]
        public string Name;

        [Required] [MinLength(1)] [JsonProperty(Required = Required.Always)]
        public string Title;

        public string Description;

        public DateTime? Due;

        public string[] Tags;

        public static explicit operator TaskItem(TaskItemCreationDto dto) =>
            dto is null
                ? null
                : new TaskItem
                {
                    DisplayId = dto.Name,
                    Title = dto.Title,
                    Description = dto.Description,
                    Due = dto.Due,
                    Tags = dto.Tags
                };
    }
}