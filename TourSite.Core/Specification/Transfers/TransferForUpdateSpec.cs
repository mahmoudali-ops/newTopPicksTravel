using Store.Core.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.Entities;

namespace TourSite.Core.Specification.Transfers
{
    public class TransferForUpdateSpec : BaseSpecifications<Transfer>
    {
        public TransferForUpdateSpec(int id) : base(t => t.Id == id)
        {
            // Include الترجمات فقط، لأننا محتاجين نحدثها
            Includes.Add(c => c.Translations);
            Includes.Add(c => c.PricesList);
            Includes.Add(c => c.Includeds);
            Includes.Add(c => c.NotIncludeds);
            Includes.Add(c => c.Highlights);


            // ممكن نضيف includes إضافية لو في حاجة تانية محتاجين نحدثها
            // مثلاً الصور أو tours لو المستقبل هيحتاج
            // Includes.Add(c => c.Tours);
        }
    }
}
