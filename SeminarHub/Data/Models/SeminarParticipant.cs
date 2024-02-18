using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeminarHub.Data.Models
{
    /// <summary>
    /// Mapping entity that maps seminars and participants
    /// </summary>
    [Comment("Mapping entity that maps seminars and participants")]
    public class SeminarParticipant
    {
        /// <summary>
        /// Seminar identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(Seminar))]
        [Comment("Seminar identifier")]
        public int SeminarId { get; set; }

        /// <summary>
        /// Seminar entity
        /// </summary>
        [Comment("Seminar entity")]
        public Seminar Seminar { get; set; } = null!;

        /// <summary>
        /// Participant identifier
        /// </summary>
        [Required]
        [ForeignKey(nameof(Participant))]
        [Comment("Participant identifier")]
        public string ParticipantId { get; set; } = string.Empty;

        /// <summary>
        /// Participant entity
        /// </summary>
        [Comment("Participant entity")]
        public IdentityUser Participant { get; set; } = null!;
    }
}
