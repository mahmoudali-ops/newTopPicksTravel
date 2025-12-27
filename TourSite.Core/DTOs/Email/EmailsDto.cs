using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.Entities;

namespace TourSite.Core.DTOs.Email
{
    public class EmailsDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }

        public string EmailAddress { get; set; }

        public string Message { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? BookingDate { get; set; }

        // Foreign Key
        public int FK_TourId { get; set; }

        public string FullTourName { get; set; }

        public int AdultNumber { get; set; }
        public int ChildernNumber { get; set; }


        public int? RoomNumber { get; set; }
        public string? HotelName { get; set; }
    }
}
