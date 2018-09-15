using System;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Web.Models.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models.Task
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class TaskFullDto
    {
        [Required]
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [Required]
        [MaxLength(140)]
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        [Required]
        [JsonProperty(Required = Required.Always)]
        public DateTime CreatedAt { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime DueBy { get; set; }

        public static explicit operator TaskFullDto(TaskItem entity) =>
            entity is null
                ? null
                : new TaskFullDto
                {
                    Id = entity.DisplayId,
                    Title = entity.Title,
                    Description = entity.Description,
                    CreatedAt = entity.CreatedAt,
                    DueBy = entity.Due?.ToUniversalTime() ?? default
                };
    }
}