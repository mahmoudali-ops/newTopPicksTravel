using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.DTOs.TourImg
{
    public class TourImgUpdateDto
    {
        public int Id { get; set; }

        public string ImageCarouselUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public string? TranslationsJson { get; set; }
        public int? FK_TourId { get; set; }

        public List<TourImgTranslationDto> Translations { get; set; } = new();
    }
}
