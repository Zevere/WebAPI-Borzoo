using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    public class User : EntityBase
    {
        public string Id { get; set; }

        [Required]
        public string DisplayId { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        public string PassphraseHash { get; set; }

        public string Token { get; set; }
        
        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedAt { get; set; }

        public bool IsDeleted { get; set; }
    }
}