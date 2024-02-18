using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Common.ValidationConstants.SeminarConstants;

namespace SeminarHub.Data.Models
{
    public class Seminar
    {
        /// <summary>
        /// Seminar identifier
        /// </summary>
        [Key]
        [Comment("Seminar identifier")]
        public int Id { get; set; }

        /// <summary>
        /// Seminar topic's
        /// </summary>
        [Required]
        [MaxLength(SeminarTopicMaxLength)]
        [Comment("Seminar's topic")]
        public string Topic { get; set; } = string.Empty;

        /// <summary>
        /// Seminar's lecturer
        /// </summary>
        [Required]
        [MaxLength(SeminarLecturerMaxLength)]
        [Comment("Seminar's lecturer")]
        public string Lecturer { get; set; } = string.Empty;

        /// <summary>
        /// Seminar's details
        /// </summary>
        [Required]
        [MaxLength(SeminarDetailsMaxLength)]
        [Comment("Seminar's details")]
        public string Details { get; set; } = string.Empty;

        /// <summary>
        /// Seminar's organizer identifier
        /// </summary>
        [Required]
        [Comment("Seminar's organizer identifier")]
        public string OrganizerId { get; set; } = string.Empty;

        /// <summary>
        /// Seminar's organiser
        /// </summary>
        [Required]
        [Comment("Seminar's organiser")]
        public IdentityUser Organizer { get; set; } = null!;

        /// <summary>
        /// Seminar's date and time
        /// </summary>
        [Required]
        [Comment("Seminar's date and time")]
        public DateTime DateAndTime { get; set; }

        /// <summary>
        /// Duration of the seminar
        /// </summary>
        [Range(SeminarDurationMinValue, SeminarDurationMaxValue)]
        [Comment("Duration of the seminar")]
        public int Duration { get; set; }

        /// <summary>
        /// Category identifier
        /// </summary>
        [Required]
        [Comment("Seminar category identifier")]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        /// <summary>
        /// Category of the seminar
        /// </summary>
        [Required]
        [Comment("Category of the seminar")]
        public Category Category { get; set; } = null!;

        /// <summary>
        /// Seminar's Participants
        /// </summary>
        [Comment("Seminar's Participants")]
        public ICollection<SeminarParticipant> SeminarsParticipants { get; set; } = new List<SeminarParticipant>();
    }
}
