using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.GraphQL.Models
{
    /// <summary>
    /// DTO object for creating a new task list
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
    public class TaskListCreationDto
    {
        /// <summary>
        /// Unique identifier or list name
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Id { get; set; }

        /// <summary>
        /// Title or summary
        /// </summary>
        [Required]
        [MinLength(1)]
        public string Title { get; set; }

        /// <summary>
        /// Optional description of the task
        /// </summary>
        [MinLength(1)]
        public string Description { get; set; }

        /// <summary>
        /// Optional list of collaborator usernames
        /// </summary>
        [MinLength(1)]
        public string[] Collaborators { get; set; }

        /// <summary>
        /// Optional tags
        /// </summary>
        [MinLength(1)]
        public string[] Tags { get; set; }

        /// <summary>
        /// Converts the DTO into a <see cref="TaskList"/> entity
        /// </summary>
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
