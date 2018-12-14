using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    /// <summary>
    /// DTO object for creating a new task item
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskItemCreationDto
    {
        /// <summary>
        /// Unique identifier or task name
        /// </summary>
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        /// <summary>
        /// Title or summary
        /// </summary>
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        /// <summary>
        /// Optional description of the task
        /// </summary>
        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Optional due date
        /// </summary>
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime? Due { get; set; }

        /// <summary>
        /// Optional tags
        /// </summary>
        [MinLength(1)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<string> Tags { get; set; }

        /// <summary>
        /// Converts the DTO into a <see cref="TaskItem"/> entity
        /// </summary>
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
