using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.DTOs.Transfer
{
    public class TransferAllDto
    {
        public int Id { get; set; }
        public string? Slug { get; set; }   // 👈 جديد

        public string ImageCover { get; set; }
        public bool IsActive { get; set; }

        public int? FK_DestinationID { get; set; }
        public string DestinationName { get; set; }

        public string Names { get; set; }
        public string Descriptions { get; set; }


    }
}
