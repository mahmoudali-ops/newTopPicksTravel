using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.CategorToutCreateDto;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.Entities;

namespace TourSite.Core.DTOs.Transfer
{
    public class TransferCreateDto
    {
        public IFormFile? ImageFile { get; set; }
        public bool IsActive { get; set; } = true;
        public string? TranslationsJson { get; set; }
        public string? PriecesListJson { get; set; }
        public string? hightlightJson { get; set; }
        public string? NonIncludesJson { get; set; }
        public string? IncludesJson { get; set; }

        public string ReferneceName { get; set; }

        public int? FK_DestinationID { get; set; }

        public List<TransferTranslationDto> Translations { get; set; } = new();
        public List<TransferPricesDTO> PricesList { get; set; } = new();

        public List<TransferIncludedDto> Includeds { get; set; } = new();
        public List<TransferNotIncludedDto> NotIncludeds { get; set; } = new();
        public List<TransferHighlightDto> Highlights { get; set; } = new();


    }
}
