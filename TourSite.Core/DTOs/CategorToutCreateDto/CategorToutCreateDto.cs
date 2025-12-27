using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.DTOs.CategorToutCreateDto
{
    public class CategorToutCreateDto
    {
        public IFormFile? ImageFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? TranslationsJson { get; set; }

        public string ReferneceName { get; set; }


        public List<CategoryTourTranslationDto> Translations { get; set; } = new();
    }
}
