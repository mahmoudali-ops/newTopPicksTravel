using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class Tour
    {
        [Key]
        public int Id { get; set; }
        public string ReferneceName { get; set; }

        public string? Slug { get; set; }   // 👈 جديد

        public string ImageCover { get; set; }
        public bool IsActive { get; set; } = true;

        public int Duration { get; set; } // in days or hours
        public decimal Price { get; set; }

        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string LanguageOptions { get; set; }

        public string? LinkVideo { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // FK
        public int? FK_CategoryID { get; set; }
        public string? FK_UserID { get; set; }
        public int? FK_DestinationID { get; set; }

        // Relations
        [ForeignKey(nameof(FK_CategoryID))]
        public CategoryTour Category { get; set; }

        [ForeignKey(nameof(FK_UserID))]
        public User CreatedBy { get; set; }

        [ForeignKey(nameof(FK_DestinationID))]
        public Destination Destination { get; set; }

        public ICollection<TourImg> TourImgs { get; set; } =new List<TourImg>();
        public ICollection<Email> Emails { get; set; } = new List<Email>();
        public ICollection<TourTranslation> Translations { get; set; } = new List<TourTranslation>();

        public ICollection<TourIncluded> Included { get; set; } = new List<TourIncluded>();
        public ICollection<TourNotIncluded> NotIncluded { get; set; } = new List<TourNotIncluded>();
        public ICollection<TourHighlight> Highlights { get; set; } = new List<TourHighlight>();




    }


}
