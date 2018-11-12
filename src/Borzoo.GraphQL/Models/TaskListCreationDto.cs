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
        public string Id { get; set; }

        [Required]
        [MinLength(1)]
        public string Title { get; set; }

        [MinLength(1)]
        public string Description { get; set; }

        [MinLength(1)]
        public string[] Collaborators { get; set; }

        [MinLength(1)]
        public string[] Tags { get; set; }

        public static explicit operator TaskList(TaskListCreationDto dto) =>
            dto == null
                ? null
                : new TaskList
                {
                    DisplayId = dto.Id,
                    Title = dto.Title,
                    Description = dto.Description,
                    Collaborators = dto.Collaborators,
                    Tags = dto.Tags,
                };
    }
}
