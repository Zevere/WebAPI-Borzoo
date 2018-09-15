using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class Comment : EntityBase
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public CommentEntity For { get; set; }

        [Required]
        public string EntityId { get; set; }
        
        [Required]
        public string CommenterId { get; set; }
        
        [Required]
        public string DisplayId { get; set; }

        [Required]
        public string Text { get; set; }
        
        [Required]
        public DateTime PostedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? ModifiedAt { get; set; }
    }
}