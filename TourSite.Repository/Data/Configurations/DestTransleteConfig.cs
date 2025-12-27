using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Configurations
{
    public class DestTransleteConfig : IEntityTypeConfiguration<DestinationTranslation>
    {
        public void Configure(EntityTypeBuilder<DestinationTranslation> builder)
        {
            builder.Property(u => u.MetaDescription)
                                       .HasDefaultValue("");

            builder.Property(u => u.MetaKeyWords)
               .HasDefaultValue("");
        }
    }
}
