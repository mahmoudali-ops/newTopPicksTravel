using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourSite.Core.Entities
{
    public class CategoryTour
    {
    [Key]
    public int Id { get; set; }
    public string? Slug { get; set; }   // 👈 جديد


    public string ImageCover { get; set; }
    public string ReferneceName { get; set; }
    public bool IsActive { get; set; } = true;

    // Relations
    public ICollection<CategoryTourTranslation> Translations { get; set; }
    public ICollection<Tour> Tours { get; set; }
    }
}
