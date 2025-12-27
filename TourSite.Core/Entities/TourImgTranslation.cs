using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class TourImgTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Language { get; set; } // "en", "ar", "de" ...

        [Required, MaxLength(250)]
        public string Title { get; set; }

        [MaxLength(250)]
        public string? TourName { get; set; }

        public int FK_TourImgId { get; set; }

        [ForeignKey(nameof(FK_TourImgId))]
        public TourImg ParentTourImg { get; set; }
    }
}
