using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class TourImgProfile : Profile
    {
        public TourImgProfile(IConfiguration configuration)
        {
            CreateMap<TourImg, TourImgDto>()
                                //.ForMember(d => d.TourName, options => options.MapFrom(s => s.Tour.Title))
                                .ForMember(d => d.ImageCarouselUrl, options => options.MapFrom(s => $"{configuration["BaseUrl"]}{s.ImageCarouselUrl}"))
          // 🌍 العنوان (Title)
          .ForMember(d => d.Titles,
              opt => opt.MapFrom((src, dest, _, ctx) =>
              {
                  var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString()?.ToLower() : "en";
                  return src.Translations?
                      .FirstOrDefault(t => t.Language.ToLower() == lang)?.Title
                      ?? src.Translations?.FirstOrDefault()?.Title
                      ?? string.Empty;
              }))



          .ForMember(dest => dest.TourName,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                        src.Tour?.Translations
                            ?.FirstOrDefault(t => t.Language.ToLower() == (ctx.Items["Lang"]?.ToString().ToLower() ?? "en"))
                            ?.Title
                        ?? src.Tour?.Translations?.FirstOrDefault()?.Title
                        ?? string.Empty
                    ))
                .ReverseMap();

            CreateMap<TourImgCreateDto, TourImg>()
.ForMember(dest => dest.ImageCarouselUrl, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
.ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));


            CreateMap<TourImgTranslationDto, TourImgTranslation>();

        }
    }
}
