using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.DTOs.Transfer
{
    public class TransferTranslationDto
    {
        public string Language { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? MetaDescription { get; set; }
        public string? MetaKeyWords { get; set; }
    }
}
