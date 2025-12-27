using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.Transfer;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class TransferAllProfile : Profile
    {
        public TransferAllProfile(IConfiguration configuration)
        {
            CreateMap<Transfer, TransferAllDto>()
                                .ForMember(d => d.ImageCover, options => options.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCover}"))


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



          .ForMember(dest => dest.DestinationName,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                        src.Destination?.Translations
                            ?.FirstOrDefault(t => t.Language.ToLower() == (ctx.Items["Lang"]?.ToString().ToLower() ?? "en"))
                            ?.Name
                        ?? src.Destination?.Translations?.FirstOrDefault()?.Name
                        ?? string.Empty
                    ))
                .ReverseMap();

            CreateMap<TransferCreateDto, Transfer>()
.ForMember(dest => dest.ImageCover, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
.ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));


            CreateMap<TransferTranslationDto, TransferTranslation>();
            CreateMap<TransferPricesDTO, TrasnferPrices>();

        }
    }
}
