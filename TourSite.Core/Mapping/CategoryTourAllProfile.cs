using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.CategorToutCreateDto;
using TourSite.Core.DTOs.CategoryTour;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class CategoryTourAllProfile:Profile
    {
        public CategoryTourAllProfile(IConfiguration configuration)
        {
            CreateMap<CategoryTour, CategorToutAllDto>()

               .ForMember(d => d.ImageCover,
                   opt => opt.MapFrom((src, dest, _, ctx) =>
                       $"{configuration["BaseUrl"]}{src.ImageCover}"))
               .ForMember(d => d.Titles,
                   opt => opt.MapFrom((src, dest, _, ctx) =>
                   {
                       var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                       return src.Translations?
                                  .FirstOrDefault(t => t.Language.ToLower() == lang)?.Title
                              ?? src.Translations?.FirstOrDefault()?.Title
                              ?? string.Empty;
                   }))
               .ForMember(d => d.Descriptions,
                   opt => opt.MapFrom((src, dest, _, ctx) =>
                   {
                       var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                       return src.Translations?
                                  .FirstOrDefault(t => t.Language.ToLower() == lang)?.Description
                              ?? src.Translations?.FirstOrDefault()?.Description
                              ?? string.Empty;
                   }));
           
        }
    }
}
