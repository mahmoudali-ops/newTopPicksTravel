using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class Transfer
    {
        [Key]
        public int Id { get; set; }

        public string? Slug { get; set; }   // 👈 جديد

        public string ReferneceName { get; set; } = "";

        public string ImageCover { get; set; }
        public bool IsActive { get; set; } = true;

        // FK
        public int? FK_DestinationID { get; set; }

        [ForeignKey(nameof(FK_DestinationID))]
        public Destination Destination { get; set; }

        // Relations
        public ICollection<TransferIncludes> Includeds { get; set; }
        public ICollection<TransferNotIncludes> NotIncludeds { get; set; }
        public ICollection<TransferIHighlights> Highlights { get; set; }

        public ICollection<TransferTranslation> Translations { get; set; }


        public ICollection<TrasnferPrices> PricesList { get; set; }
    }
}
