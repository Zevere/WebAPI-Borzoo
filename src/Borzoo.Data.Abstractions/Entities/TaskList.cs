using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class TaskList : IEntity
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string OwnerId { get; set; }

        [Required]
        public string DisplayId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public string[] Tags { get; set; }

        public string[] Collaborators { get; set; }
    }
}
