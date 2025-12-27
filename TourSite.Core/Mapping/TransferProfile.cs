using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Transfer;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class TransferProfile : Profile
    {
        public TransferProfile(IConfiguration configuration)
        {
            CreateMap<Transfer, TransferDto>()
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

                // ✅ Included Items
                .ForMember(dest => dest.PricesList,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.PricesList?
                            .Where(i => i.Language.ToLower() == lang)
                            .Select(i => new TransferPricesDTO
                            {
                                Language = i.Language,
                                Title = i.Title,
                                SharedPrice = i.SharedPrice,
                                PrivtePrice=i.PrivtePrice,
                                ReferneceName=i.ReferneceName

                            }).ToList();
                    }))
                .ReverseMap();
            
            CreateMap<TransferCreateDto, Transfer>()
            .ForMember(dest => dest.ImageCover, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
            .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
            .ForMember(dest => dest.Includeds, opt => opt.MapFrom(src => src.Includeds))
            .ForMember(dest => dest.NotIncludeds, opt => opt.MapFrom(src => src.NotIncludeds))
            .ForMember(dest => dest.Highlights, opt => opt.MapFrom(src => src.Highlights));
            

            CreateMap<TransferTranslationDto, TransferTranslation>();
            CreateMap<TransferIncludedDto, TransferIncludes>();
            CreateMap<TransferNotIncludedDto, TransferNotIncludes>();
            CreateMap<TransferHighlightDto, TransferIHighlights>();

            CreateMap<TransferTranslation, TransferTranslationDto>();
            CreateMap<TransferIncludes, TransferIncludedDto>();
            CreateMap<TransferNotIncludes, TransferNotIncludedDto>();
            CreateMap<TransferIHighlights, TransferHighlightDto>();
        }
    }
    
}
