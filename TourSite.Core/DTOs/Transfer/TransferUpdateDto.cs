using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;

namespace TourSite.Core.DTOs.Transfer
{
    public class TransferUpdateDto
    {
        public int Id { get; set; }

        public string ImageCover { get; set; }
        public bool IsActive { get; set; }

        public int? FK_DestinationID { get; set; }
        public string DestinationName { get; set; }

        public string ReferneceName { get; set; }

        // 🔴 كل اللغات
        public List<TransferTranslationDto> Translations { get; set; } = new();

        public List<TransferPricesDTO> PricesList { get; set; } = new();

        public List<TransferIncludedDto> Includeds { get; set; } = new();
        public List<TransferNotIncludedDto> NotIncludeds { get; set; } = new();
        public List<TransferHighlightDto> Highlights { get; set; } = new();
    }
}
