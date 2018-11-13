using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    /// <summary>
    /// Represents a task list entity
    /// </summary>
    public class TaskList : IEntity
    {
        /// <summary>
        /// Unique identifier in the data store
        /// </summary>
        [Required]
        public string Id { get; set; }

        /// <summary>
        /// Unique display ID (task list name)
        /// </summary>
        [Required]
        public string DisplayId { get; set; }

        /// <summary>
        /// Username of the list's owner
        /// </summary>
        [Required]
        public string OwnerId { get; set; }

        /// <summary>
        /// List title
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// List descriptions
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Usernames of the list collaborators
        /// </summary>
        public string[] Collaborators { get; set; }

        /// <summary>
        /// List tags
        /// </summary>
        public string[] Tags { get; set; }

        /// <summary>
        /// List creations time
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The time this list was last modified at
        /// </summary>
        public DateTime? ModifiedAt { get; set; }
    }
}
