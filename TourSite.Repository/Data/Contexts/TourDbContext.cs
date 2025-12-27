using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.Entities;

namespace TourSite.Repository.Data.Contexts
{
    public class TourDbContext : IdentityDbContext<User>
    {
        public TourDbContext(DbContextOptions<TourDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Email> Emails { get; set; }
        public DbSet<Destination> Destinations { get; set; }
        public DbSet<Transfer> Transfers { get; set; }
        public DbSet<CategoryTour> CategoryTours { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<TourImg> TourImgs { get; set; }



        public DbSet<CategoryTourTranslation> CategoryTourTranslations { get; set; }
        public DbSet<DestinationTranslation> DestinationTranslations { get; set; }
        public DbSet<TourTranslation> TourTranslations { get; set; }
        public DbSet<TourImgTranslation> TourImgTranslations { get; set; }
        public DbSet<TransferTranslation> TransferTranslations { get; set; }


        public DbSet<TourIncluded> TourIncludeds { get; set; }
        public DbSet<TourNotIncluded> TourNotIncludeds { get; set; }
        public DbSet<TourHighlight> TourHighLight { get; set; }

        public DbSet<TransferIncludes> TransferIncludeds { get; set; }
        public DbSet<TransferNotIncludes> TransferNotIncludeds { get; set; }
        public DbSet<TransferIHighlights> TransferHighLight { get; set; }


        public DbSet<TrasnferPrices> PricesList { get; set; }
        
    }
}
