using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.DTOs.CategorToutCreateDto
{
    public class CategoryTourTranslationDto
    {
        public string Language { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeyWords { get; set; }

    }
}
