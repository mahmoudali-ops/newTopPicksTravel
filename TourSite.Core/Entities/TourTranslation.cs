using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class TourTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Language { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeyWords { get; set; }

        [Required, MaxLength(250)]
        public string Title { get; set; }
        public string Description { get; set; }

        public int TourId { get; set; }

        [ForeignKey(nameof(TourId))]
        public Tour Tour { get; set; }
    }
}
