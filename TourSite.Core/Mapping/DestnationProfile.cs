using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.CategorToutCreateDto;
using TourSite.Core.DTOs.Destnation;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.DTOs.Transfer;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class DestnationProfile : Profile
    {
        public DestnationProfile(IConfiguration configuration)
        {
            CreateMap<Destination, DestnationDto>()
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
               }))


           // 🚗 التحويلات
           .ForMember(d => d.Transfers,
               opt => opt.MapFrom((src, dest, _, ctx) =>
               {
                   var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString()?.ToLower() : "en";

                   return src.Transfers.Select(tr => new TransferDto
                   {
                       Id = tr.Id,
                       ImageCover = $"{configuration["BaseUrl"]}{tr.ImageCover}",
                       Names = tr.Translations?
                           .FirstOrDefault(t => t.Language.ToLower() == lang)?.Name
                           ?? tr.Translations?.FirstOrDefault()?.Name ?? string.Empty,
                       Descriptions = tr.Translations?
                           .FirstOrDefault(t => t.Language.ToLower() == lang)?.Description
                           ?? tr.Translations?.FirstOrDefault()?.Description ?? string.Empty
                   }).ToList();
               }))

           // 🧭 الرحلات
           .ForMember(d => d.Tours,
               opt => opt.MapFrom((src, dest, _, ctx) =>
               {
                   var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString()?.ToLower() : "en";

                   return src.Tours.Select(t => new TourDto
                   {
                       Id = t.Id,
                       ImageCover = $"{configuration["BaseUrl"]}{t.ImageCover}",
                       Titles = t.Translations?
                           .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Title
                           ?? t.Translations?.FirstOrDefault()?.Title ?? string.Empty,
                       Descriptions = t.Translations?
                           .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Description
                           ?? t.Translations?.FirstOrDefault()?.Description ?? string.Empty,
                       Duration = t.Duration,
                       Price = t.Price
                   }).ToList();
               }));


            CreateMap<DestnationToutCreateDto, Destination>()
    .ForMember(dest => dest.ImageCover, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
    .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<DestnaionTranslationDto, DestinationTranslation>();

        }
    }
}
