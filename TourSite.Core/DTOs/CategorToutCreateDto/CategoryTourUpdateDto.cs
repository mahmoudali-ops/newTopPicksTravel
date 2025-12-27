using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;

namespace TourSite.Core.DTOs.CategorToutCreateDto
{
    public class CategoryTourUpdateDto
    {
        public int Id { get; set; }

        public string ImageCover { get; set; }
        public string ReferneceName { get; set; }
        public bool IsActive { get; set; } = true;

        // Relations
        public ICollection<CategoryTourTranslationDto> Translations { get; set; }
    }
}
