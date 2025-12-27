using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.DTOs.Transfer;

namespace TourSite.Core.DTOs.Destnation
{
    public class DestnationAllDto
    {
        public int Id { get; set; }

        public string ImageCover { get; set; }
        public bool IsActive { get; set; }

        public string Names { get; set; }
        public string Descriptions { get; set; }

        public string ReferneceName { get; set; }


    }
}
