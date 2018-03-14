using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class TaskList : EntityBase
    {
        public string Id { get; set; }

        public string OwnerId { get; set; }
        
        [Required]
        public string DisplayId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }
    }
}