using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Destnation;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class DestnationALlProfile:Profile
    {
        public DestnationALlProfile(IConfiguration configuration)
        {
            CreateMap<Destination, DestnationAllDto>()
           // 🖼️ نضيف الـ BaseUrl على الصورة
           .ForMember(d => d.ImageCover,
               opt => opt.MapFrom((src, dest, _, ctx) =>
                   $"{configuration["BaseUrl"]}{src.ImageCover}"))

           // 🌍 العنوان (Title)
           .ForMember(d => d.Names,
               opt => opt.MapFrom((src, dest, _, ctx) =>
               {
                   var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString()?.ToLower() : "en";
                   return src.Translations?
                       .FirstOrDefault(t => t.Language.ToLower() == lang)?.Name
                       ?? src.Translations?.FirstOrDefault()?.Name
                       ?? string.Empty;
               }))

           // 📜 الوصف (Description)
           .ForMember(d => d.Descriptions,
               opt => opt.MapFrom((src, dest, _, ctx) =>
               {
                   var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString()?.ToLower() : "en";
                   return src.Translations?
                       .FirstOrDefault(t => t.Language.ToLower() == lang)?.Description
                       ?? src.Translations?.FirstOrDefault()?.Description
                       ?? string.Empty;
               }));

    

            CreateMap<DestnationToutCreateDto, Destination>()
    .ForMember(dest => dest.ImageCover, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
    .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<DestnaionTranslationDto, DestinationTranslation>();

        }

    }
}
