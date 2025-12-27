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
    public class CategoryTourConfiguration : IEntityTypeConfiguration<CategoryTour>
    {
        public void Configure(EntityTypeBuilder<CategoryTour> builder)
        {
            builder.ToTable("CategoryTours");


            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.HasMany(c => c.Tours)
                .WithOne(t => t.Category)
                .HasForeignKey(t => t.FK_CategoryID)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(c => c.Translations)
                .WithOne(t => t.CategoryTour)
                .HasForeignKey(t => t.CategoryTourId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(u => u.ReferneceName)
               .HasDefaultValue("");


            builder.Property(u => u.Slug)
         .HasDefaultValue(""); // القيمة الافتراضية

            builder.HasIndex(u => u.Slug)
                   .IsUnique(); // index فريد





        }
    }
}
