﻿using System;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models.Task
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class TaskCreationDto : TaskDtoBase
    {
        [Required]
        [MaxLength(140)]
        [JsonProperty(Required = Required.Always)]
        public string Title { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Description { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public DateTime DueBy { get; set; }

        public static explicit operator UserTask(TaskCreationDto dto)
        {
            if (dto is default)
                return default;

            var task = new UserTask
            {
                Title = dto.Title,
                Description = dto.Description,
            };
            if (dto.DueBy == DateTime.MinValue)
                task.Due = null;
            else
                task.Due = dto.DueBy;
            return task;
        }
    }
}