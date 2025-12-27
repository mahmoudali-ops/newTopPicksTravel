using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.DTOs.Includes
{
    public class TransferIncludedDto
    {
        public int Id { get; set; }
        public string Language { get; set; } // en, ar, de, nl ...
        public string Text { get; set; }
    }
}
