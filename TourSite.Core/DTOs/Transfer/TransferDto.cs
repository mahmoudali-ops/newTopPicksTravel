using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Destnation;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.Entities;

namespace TourSite.Core.DTOs.Transfer
{
    public class TransferDto
    {
        public int Id { get; set; }
        public string? Slug { get; set; }   // 👈 جديد

        public string ImageCover { get; set; }
        public bool IsActive { get; set; }

        public int? FK_DestinationID { get; set; }
        public string DestinationName { get; set; }

        public string Names { get; set; }
        public string Descriptions { get; set; }

        public string ReferneceName { get; set; }


        public List<TransferPricesDTO> PricesList { get; set; }

        public List<TransferIncludedDto> Includeds { get; set; } = new();
        public List<TransferNotIncludedDto> NotIncludeds { get; set; } = new();
        public List<TransferHighlightDto> Highlights { get; set; } = new();

    }
}
