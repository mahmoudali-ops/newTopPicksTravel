using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class CategoryTourTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Language { get; set; } // "en", "ar", etc.

        [Required, MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeyWords { get; set; }

        // FK
        public int CategoryTourId { get; set; }

        [ForeignKey(nameof(CategoryTourId))]
        public CategoryTour CategoryTour { get; set; }
    }
}
