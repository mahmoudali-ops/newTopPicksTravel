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
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class TourProfile : Profile
    {
        public TourProfile(IConfiguration configuration)
        {
            CreateMap<Tour, TourDto>()
                // ✅ الصورة الأساسية
                .ForMember(dest => dest.ImageCover,
                    opt => opt.MapFrom(src => $"{configuration["BaseUrl"]}{src.ImageCover}"))

                // ✅ عنوان الرحلة
                .ForMember(dest => dest.Titles,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.Translations?
                            .FirstOrDefault(t => t.Language.ToLower() == lang)?.Title
                            ?? src.Translations?.FirstOrDefault()?.Title
                            ?? string.Empty;
                    }))
                  .ForMember(dest => dest.DestinationName,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.Destination.Translations?
                            .FirstOrDefault(t => t.Language.ToLower() == lang)?.Name;
                    }))
                    .ForMember(dest => dest.CategoryName,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.Category.Translations?
                            .FirstOrDefault(t => t.Language.ToLower() == lang)?.Title;
                    }))

                // ✅ وصف الرحلة
                .ForMember(dest => dest.Descriptions,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.Translations?
                            .FirstOrDefault(t => t.Language.ToLower() == lang)?.Description
                            ?? src.Translations?.FirstOrDefault()?.Description
                            ?? string.Empty;
                    }))

                // ✅ الصور
                .ForMember(dest => dest.TourImgs,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.TourImgs.Select(img => new TourImgDto
                        {
                            Id = img.Id,
                            ImageCarouselUrl = $"{configuration["BaseUrl"]}{img.ImageCarouselUrl}",
                            Titles = img.Translations?
                                .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Title
                                ?? img.Translations?.FirstOrDefault()?.Title ?? string.Empty,
                            TourName = img.Translations?
                                .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.TourName
                                ?? img.Translations?.FirstOrDefault()?.TourName ?? string.Empty
                        }).ToList();
                    }))

                // ✅ Included Items
                .ForMember(dest => dest.IncludedItems,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.Included?
                            .Where(i => i.Language.ToLower() == lang)
                            .Select(i => new TourIncludedDto
                            {
                          //      TourName = dest.Titles, // أو ممكن src.Translations?.FirstOrDefault()?.Title
                                Language = i.Language,
                                Text = i.Text
                            }).ToList();
                    }))

                // ✅ NotIncluded Items
                .ForMember(dest => dest.NotIncludedItems,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.NotIncluded?
                            .Where(i => i.Language.ToLower() == lang)
                            .Select(i => new TourNotIncludedDto
                            {
                                //TourName = dest.Titles,
                                Language = i.Language,
                                Text = i.Text
                            }).ToList();
                    }))

                // ✅ Highlights Items
                .ForMember(dest => dest.Highlights,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        var tourName = src.Translations.FirstOrDefault(t => t.Language.ToLower() == lang)?.Title;

                        return src.Highlights?
                            .Where(i => i.Language.ToLower() == lang)
                            .Select(i => new TourHighlightDto
                            {
                              ///  Id = i.Id,
                               // TourId = i.TourId,
                               // TourName = tourName,
                                Language = i.Language,
                                Text = i.Text
                            }).ToList();
                    }));
            CreateMap<TourCreateDto, Tour>()
.ForMember(dest => dest.ImageCover, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
.ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations))
.ForMember(dest => dest.Included, opt => opt.MapFrom(src => src.IncludesPoints))
.ForMember(dest => dest.NotIncluded, opt => opt.MapFrom(src => src.NonIncludesPoints))
.ForMember(dest => dest.Highlights, opt => opt.MapFrom(src => src.hightlightPoints));


            CreateMap<TourTranslationDto, TourTranslation>();
            CreateMap<TourIncludedDto, TourIncluded>();
            CreateMap<TourNotIncludedDto, TourNotIncluded>();
            CreateMap<TourHighlightDto, TourHighlight>();
            CreateMap<TourImgTranslationDto, TourImgTranslation>();

        }
    }
}
