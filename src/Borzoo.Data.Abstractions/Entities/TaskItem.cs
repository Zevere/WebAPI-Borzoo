using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class TaskItem : EntityBase
    {
        [Required] public string Id { get; set; }

        [Required] public string ListId { get; set; }

        [Required] public string DisplayId { get; set; }

        [Required] [StringLength(14)] public string Title { get; set; }

        [Required] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string Description { get; set; }

        public DateTime? Due { get; set; }

        public string[] Tags { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}