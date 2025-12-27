using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.Entities;

namespace TourSite.Core.DTOs.TourImg
{
    public class TourImgDto
    {
        public int Id { get; set; }

        public string ImageCarouselUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public int? FK_TourId { get; set; }

        public string TourName { get; set; }

        // en : .. fr : .. ar : ..  
        public string Titles { get; set; }
    }
}
