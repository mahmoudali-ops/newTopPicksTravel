using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class TourHighlight
    {
        public int Id { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }

        public string Language { get; set; }
        public string Text { get; set; }
    }
}
