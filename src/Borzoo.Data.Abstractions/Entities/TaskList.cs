using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class TaskList : EntityBase
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string OwnerId { get; set; }
        
        [Required]
        public string DisplayId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}