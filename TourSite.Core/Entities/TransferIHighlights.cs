using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class TransferIHighlights
    {
        public int Id { get; set; }
        public int TransferId { get; set; }

        [ForeignKey(nameof(TransferId))]
        public Transfer Transfer { get; set; }

        public string Language { get; set; } // en, ar, de, nl ...
        public string Text { get; set; }
    }
}
