using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskItemCreationDto
    {
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? Due { get; set; }

        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        public static explicit operator TaskItem(TaskItemCreationDto dto) =>
            dto is null
                ? null
                : new TaskItem
                {
                    DisplayId = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    Due = dto.Due,
                    Tags = dto.Tags?.ToArray()
                };
    }
}