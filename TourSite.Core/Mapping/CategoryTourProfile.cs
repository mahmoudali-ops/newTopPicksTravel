using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TourSite.Core.DTOs.CategorToutCreateDto;
using TourSite.Core.DTOs.CategoryTour;
using TourSite.Core.DTOs.Email;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;

namespace TourSite.Core.Mapping
{
    public class CategoryTourProfile : Profile
    {
        public CategoryTourProfile(IConfiguration configuration)
        {
            CreateMap<CategoryTour, CategorToutDto>()

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
                    }))
                .ForMember(d => d.Tours,
                    opt => opt.MapFrom((src, dest, _, ctx) =>
                    {
                        var lang = ctx.Items.ContainsKey("Lang") ? ctx.Items["Lang"]?.ToString() : "en";
                        return src.Tours.Select(t => new TourDto
                        {
                            Id = t.Id,
                            ImageCover = $"{configuration["BaseUrl"]}{t.ImageCover}",
                            Duration = t.Duration,
                            Price = t.Price,
                            StartLocation = t.StartLocation,
                            EndLocation = t.EndLocation,
                            LanguageOptions = t.LanguageOptions,
                            CreatedAt = t.CreatedAt,
                            Titles = t.Translations?
                                         .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Title
                                     ?? t.Translations?.FirstOrDefault()?.Title
                                     ?? string.Empty,
                            Descriptions = t.Translations?
                                             .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Description
                                         ?? t.Translations?.FirstOrDefault()?.Description
                                         ?? string.Empty,
                            DestinationName = t.Destination?.Translations?
                                                 .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Name
                                             ?? t.Destination?.Translations?.FirstOrDefault()?.Name ?? string.Empty,
                            TourImgs = t.TourImgs.Select(img => new TourImgDto
                            {
                                Id = img.Id,
                                ImageCarouselUrl = $"{configuration["BaseUrl"]}{img.ImageCarouselUrl}",
                                Titles = img.Translations?
                                             .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.Title
                                         ?? img.Translations?.FirstOrDefault()?.Title ?? string.Empty,
                                TourName = img.Translations?
                                               .FirstOrDefault(tr => tr.Language.ToLower() == lang)?.TourName
                                           ?? img.Translations?.FirstOrDefault()?.TourName ?? string.Empty
                            }).ToList()
                        }).ToList();
                    }));

            CreateMap<CategorToutCreateDto, CategoryTour>()
    .ForMember(dest => dest.ImageCover, opt => opt.Ignore()) // لأننا هنرفع الصورة يدوي
    .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<CategoryTourTranslationDto, CategoryTourTranslation>();
        }
    }

}
