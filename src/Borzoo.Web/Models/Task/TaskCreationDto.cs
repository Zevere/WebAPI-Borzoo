using System;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models.Task
{
    [JsonObject(MemberSerialization.OptOut, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class TaskCreationDto : TaskDtoBase
    {
        [Required]
        [MaxLength(140)]
        public string Title { get; set; }

        public string Description { get; set; }

        public DateTime DueBy { get; set; }

        public static explicit operator UserTask(TaskCreationDto dto) =>
            dto is default
                ? default
                : new UserTask
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    Due = dto.DueBy == DateTime.MinValue ? default : dto.DueBy
                };
    }
}