using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using static Common.ValidationConstants.CategoryConstants;

namespace SeminarHub.Data.Models
{
    /// <summary>
    /// Category database entity
    /// </summary>
    [Comment("Category database entity")]
    public class Category
    {
        /// <summary>
        /// Category's identifier
        /// </summary>
        [Key]
        [Comment("Category's identifier")]
        public int Id { get; set; }

        /// <summary>
        /// "Category name"
        /// </summary>
        [Required]
        [StringLength(CategoryNameMaxLength)]
        [Comment("Category name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Seminars in certain category
        /// </summary>
        [Comment("Seminars in certain category")]
        public ICollection<Seminar> Seminars { get; set; } = new List<Seminar>();
    }
}
