using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Tours;

namespace TourSite.Core.DTOs.CategoryTour
{
    public class CategorToutAllDto
    {
        public int Id { get; set; }
        public string? Slug { get; set; }   // 👈 جديد

        public string ImageCover { get; set; }
        public bool IsActive { get; set; }

        // key: "en", "ar"  |  value: text
        public string Titles { get; set; }
        public string Descriptions { get; set; }

        public string ReferneceName { get; set; }


    }
}
