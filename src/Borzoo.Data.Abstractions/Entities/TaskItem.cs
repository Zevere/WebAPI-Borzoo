using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class TaskItem : IEntity
    {
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Unique identifier of the containing list
        /// </summary>
        [Required]
        public string ListId { get; set; }

        /// <summary>
        /// Username of the containing list's owner
        /// </summary>
        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string DisplayId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        public DateTime? Due { get; set; }

        public string[] Tags { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
