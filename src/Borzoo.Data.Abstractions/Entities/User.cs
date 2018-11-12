using System;
using System.ComponentModel.DataAnnotations;

namespace Borzoo.Data.Abstractions.Entities
{
    /// <summary>
    /// Represents a Zevere user
    /// </summary>
    public class User : IEntity
    {
        /// <summary>
        /// Unique identifier in the data store
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Unique display ID (username)
        /// </summary>
        [Required]
        public string DisplayId { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Passphrase hash
        /// </summary>
        [Required]
        public string PassphraseHash { get; set; }

        /// <summary>
        /// Unique authentication token
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// The time that this entity was created at in UTC format
        /// </summary>
        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// The time that this entity was last modified at in UTC format
        /// </summary>
        public DateTime? ModifiedAt { get; set; }
    }
}
