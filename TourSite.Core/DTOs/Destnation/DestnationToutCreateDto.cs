using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.CategorToutCreateDto;

namespace TourSite.Core.DTOs.Destnation
{
    public class DestnationToutCreateDto
    {
        public IFormFile? ImageFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? TranslationsJson { get; set; }

        public string ReferneceName { get; set; }


        public List<DestnaionTranslationDto> Translations { get; set; } = new();
    }
}
