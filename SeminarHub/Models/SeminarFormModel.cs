using System.ComponentModel.DataAnnotations;
using static Common.ValidationConstants.SeminarConstants;

namespace SeminarHub.Models
{
    public class SeminarFormModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(SeminarTopicMaxLength, MinimumLength = SeminarTopicMinLength)]
        public string Topic { get; set; } = string.Empty;

        [Required]
        [StringLength(SeminarLecturerMaxLength, MinimumLength = SeminarLecturerMinLength)]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [StringLength(SeminarDetailsMaxLength, MinimumLength = SeminarDetailsMinLength)]
        public string Details { get; set; } = string.Empty;

        [Required]
        public string DateAndTime { get; set; } = string.Empty;

        [Range(SeminarDurationMinValue,SeminarDurationMaxValue)]
        public int Duration { get; set; }

        public string OrganizerId { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public ICollection<CategoryViewModel> Categories = new List<CategoryViewModel>();

    }
}
