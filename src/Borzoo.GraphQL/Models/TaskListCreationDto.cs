using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskListCreationDto
    {
        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Id { get; set; }

        [Required]
        [MinLength(1)]
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        public static explicit operator TaskList(TaskListCreationDto dto) =>
            dto is null
                ? null
                : new TaskList
                {
                    DisplayId = dto.Id,
                    Title = dto.Title,
                };
    }
}