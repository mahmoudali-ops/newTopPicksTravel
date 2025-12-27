using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Specification.Transfers
{
    public class TrasferSpecParam
    {
        private string? lang = "en";
        public string? Lang
        {
            get => lang;
            set => lang = value.ToLower();
        }
        public int pageSize { get; set; } = 10;
        public int pageIndex { get; set; } = 1; 
    }
}
