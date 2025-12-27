using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Email;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.Entities;

namespace TourSite.Core.DTOs.Tours
{
    public class TourDto
    {
        public int Id { get; set; }

        public string? Slug { get; set; }   // 👈 جديد

        public string ImageCover { get; set; }
        public bool IsActive { get; set; }

        public int Duration { get; set; }
        public decimal Price { get; set; }

        public string StartLocation { get; set; }
        public string EndLocation { get; set; }
        public string LanguageOptions { get; set; }

        public DateTime CreatedAt { get; set; }

        public int? FK_CategoryID { get; set; }
        public string? FK_UserID { get; set; }
        public int? FK_DestinationID { get; set; }

        public string CategoryName { get; set; }

        //  public string CreatedByName { get; set; }
        public string DestinationName { get; set; }

        public string Titles { get; set; }
        public string Descriptions { get; set; }

        public string? LinkVideo { get; set; }

        public string ReferneceName { get; set; }



        public ICollection<TourImgDto> TourImgs { get; set; }

   //     public ICollection<EmailsDto> Emails { get; set; }


        public List<TourHighlightDto> Highlights { get; set; }
        public List<TourIncludedDto> IncludedItems { get; set; }
        public List<TourNotIncludedDto> NotIncludedItems { get; set; }


    }
}
