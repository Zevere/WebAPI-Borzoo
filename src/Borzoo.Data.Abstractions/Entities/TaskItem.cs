using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    /// <summary>
    /// Represents a task item entity
    /// </summary>
    public class TaskItem : IEntity
    {
        /// <summary>
        /// Unique identifier in the data store
        /// </summary>
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

        /// <summary>
        /// Unique display ID. The task name
        /// </summary>
        [Required]
        public string DisplayId { get; set; }

        /// <summary>
        /// Title or summary of the task
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Optional due date
        /// </summary>
        public DateTime? Due { get; set; }

        /// <summary>
        /// Optional tags
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// Creation time
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The time this task was last modified at, if available.
        /// </summary>
        public DateTime? ModifiedAt { get; set; }
    }
}
