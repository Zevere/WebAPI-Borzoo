using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class UserTask : EntityBase
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [StringLength(14)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime? Due { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}