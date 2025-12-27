using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class TransferTranslation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Language { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string? MetaDescription { get; set; }
        public string? MetaKeyWords { get; set; }

        public int TransferId { get; set; }

        [ForeignKey(nameof(TransferId))]
        public Transfer Transfer { get; set; }
    }
}
