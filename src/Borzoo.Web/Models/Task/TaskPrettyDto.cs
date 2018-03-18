using System;
using System.ComponentModel.DataAnnotations;
using Borzoo.Data.Abstractions.Entities;
using Borzoo.Web.Models.User;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Borzoo.Web.Models.Task
{
    [JsonObject(MemberSerialization.OptIn, NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class TaskPrettyDto
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
        public bool IsDue { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string DueIn { get; set; }

        public static explicit operator TaskPrettyDto(TaskItem entity)
        {
            if (entity is null)
                return null;
            var now = DateTime.UtcNow;
            var due = entity.Due?.ToUniversalTime();
            bool isDue = due.HasValue && now >= due;
            string dueIn = default;
            if (due.HasValue && !isDue)
            {
                var timeLeft = due.Value - now;
                if (timeLeft.Days > 0)
                {
                    dueIn = $"{timeLeft.Days} day" + (timeLeft.Days > 1 ? "s" : string.Empty);
                    dueIn += timeLeft.Hours > 1 ? $", {timeLeft.Hours} hours" : string.Empty;
                }
                else if (timeLeft.Hours > 0)
                {
                    dueIn = $"{timeLeft.Hours} hour" + (timeLeft.Hours > 1 ? "s" : string.Empty);
                    dueIn += timeLeft.Minutes > 1 ? $", {timeLeft.Minutes} minutes" : string.Empty;
                }
                else if (timeLeft.Minutes > 0)
                {
                    dueIn = $"{timeLeft.Minutes} minute" + (timeLeft.Minutes > 1 ? "s" : string.Empty);
                }
                else if (timeLeft.Seconds > 1)
                {
                    dueIn = "a few seconds";
                }
                else
                {
                    dueIn = "now";
                }
            }

            return new TaskPrettyDto
            {
                Id = entity.DisplayId,
                Title = entity.Title,
                Description = entity.Description,
                IsDue = isDue,
                DueIn = dueIn
            };
        }
    }
}