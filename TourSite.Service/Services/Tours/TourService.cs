using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using Store.Core.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TourSite.Core;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.TourImgs;
using TourSite.Core.Specification.Tours;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TourSite.Service.Services.Tours
{
    public class TourService : IToursService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private IConfiguration configuration;

        public IWebHostEnvironment env { get; }


        public TourService(IUnitOfWork _unitOfWork, IMapper _mapper, IWebHostEnvironment _env, IConfiguration _configuration)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            env = _env;
            configuration = _configuration;
        }


        public async Task<PageinationResponse<TourAllDto>> GetAllToursAsync(TourSpecParams specParams )
        {
            var allowedLangs = new[] { "en", "de" ,"nl"};
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            string lang = specParams.Lang.ToLower();

            var query = unitOfWork.Repository<Tour>().Query()
                .Where(t => t.IsActive) // شرط التفعيل
                .AsNoTracking();

            // تطبيق Pagination
            int skip = specParams.pageSize * (specParams.pageIndex - 1);
            int take = specParams.pageSize;

            // Projection مباشر
            var dataQuery = query
                .OrderBy(t => t.Id) // أو حسب أي OrderBy في specParams
                .Skip(skip)
                .Take(take)
                .Select(t => new TourAllDto
                {
                    Id = t.Id,
                    Slug = t.Slug,
                    ReferneceName = t.ReferneceName,
                    ImageCover = $"{configuration["BaseUrl"]}{t.ImageCover}", // ممكن تضيف BaseUrl
                    Price = t.Price,
                    Duration = t.Duration,
                    StartLocation = t.StartLocation,
                    EndLocation = t.EndLocation,
                    LanguageOptions = t.LanguageOptions,
                    CreatedAt = t.CreatedAt,
                    FK_CategoryID = t.FK_CategoryID,
                    FK_UserID = t.FK_UserID,
                    FK_DestinationID = t.FK_DestinationID,
                    Descriptions = t.Translations.
                                    Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Description)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Description,
                    Titles = t.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Title)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Title,
                    DestinationName = t.Destination.Translations
                                .Where(d => d.Language.ToLower() == lang)
                                .Select(d => d.Name)
                                .FirstOrDefault(),
                    CategoryName = t.Category.Translations
                                .Where(c => c.Language.ToLower() == lang)
                                .Select(c => c.Title)
                                .FirstOrDefault()
                });

            var data = await dataQuery.ToListAsync();

            // Count بدون Includes (أخف بكثير)
            int totalCount = await query.CountAsync();

            return new PageinationResponse<TourAllDto>(specParams.pageIndex, specParams.pageSize, totalCount, data);
        }

        public async Task<PageinationResponse<TourAllDto>> GetAllToursTrueAsync(TourSpecParams tourSpecParams)
        {

            var allowedLangs = new[] { "en", "de", "nl" };
            if (string.IsNullOrEmpty(tourSpecParams.Lang) || !allowedLangs.Contains(tourSpecParams.Lang.ToLower()))
                tourSpecParams.Lang = "en";

            string lang = tourSpecParams.Lang.ToLower();

            var query = unitOfWork.Repository<Tour>().Query()
                .AsNoTracking();

            // تطبيق Pagination
            int skip = tourSpecParams.pageSize * (tourSpecParams.pageIndex - 1);
            int take = tourSpecParams.pageSize;

            // Projection مباشر
            var dataQuery = query
                .OrderBy(t => t.Id) // أو حسب أي OrderBy في specParams
                .Skip(skip)
                .Take(take)
                .Select(t => new TourAllDto
                {
                    Id = t.Id,
                    Slug = t.Slug,
                    ReferneceName = t.ReferneceName,
                    Price = t.Price,
                    CreatedAt = t.CreatedAt,
                    FK_CategoryID = t.FK_CategoryID,
                    FK_UserID = t.FK_UserID,
                    IsActive = t.IsActive,
                    FK_DestinationID = t.FK_DestinationID,
                    Titles = t.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Title)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Title,
                    DestinationName = t.Destination.Translations
                                .Where(d => d.Language.ToLower() == lang)
                                .Select(d => d.Name)
                                .FirstOrDefault(),
                    CategoryName = t.Category.Translations
                                .Where(c => c.Language.ToLower() == lang)
                                .Select(c => c.Title)
                                .FirstOrDefault()
                });

            var data = await dataQuery.ToListAsync();

            // Count بدون Includes (أخف بكثير)
            int totalCount = await query.CountAsync();

            return new PageinationResponse<TourAllDto>(tourSpecParams.pageIndex, tourSpecParams.pageSize, totalCount, data);
        }


        public async Task<TourDto> GetTourByIdAsync(string slug, string? lang = "en")
        {
            if (lang is null) lang = "en";
            lang = lang.ToLower();

            // projection مباشر بدون Includes الثقيلة
            var tourDto = await unitOfWork.Repository<Tour>()
                .Query()
                .Where(t => t.Slug == slug && t.IsActive)
                .Select(t => new TourDto
                {
                    Id = t.Id,
                    Slug = t.Slug,
                    Price= t.Price,
                    IsActive = t.IsActive,
                    Duration = t.Duration,
                    StartLocation = t.StartLocation,
                    EndLocation = t.EndLocation,
                    LanguageOptions = t.LanguageOptions,
                    CreatedAt = t.CreatedAt,
                    FK_CategoryID = t.FK_CategoryID,
                    FK_UserID = t.FK_UserID,
                    FK_DestinationID = t.FK_DestinationID,
                    LinkVideo = t.LinkVideo,

                    ReferneceName = t.ReferneceName,
                    ImageCover = $"{configuration["BaseUrl"]}{t.ImageCover}",

                    // العنوان والوصف حسب اللغة
                    Titles = t.Translations
                                .Where(x => x.Language.ToLower() == lang)
                                .Select(x => x.Title)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Title,
                    Descriptions = t.Translations
                                .Where(x => x.Language.ToLower() == lang)
                                .Select(x => x.Description)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Description,

                    // Destination & Category
                    DestinationName = t.Destination.Translations
                                        .Where(d => d.Language.ToLower() == lang)
                                        .Select(d => d.Name)
                                        .FirstOrDefault(),
                    CategoryName = t.Category.Translations
                                        .Where(c => c.Language.ToLower() == lang)
                                        .Select(c => c.Title)
                                        .FirstOrDefault(),

                    // الصور
                    TourImgs = t.TourImgs.Select(img => new TourImgDto
                    {
                        Id = img.Id,
                        ImageCarouselUrl = $"{configuration["BaseUrl"]}{img.ImageCarouselUrl}",
                        Titles = img.Translations
                                    .Where(tr => tr.Language.ToLower() == lang)
                                    .Select(tr => tr.Title)
                                    .FirstOrDefault() ?? img.Translations.FirstOrDefault().Title,
                        TourName = img.Translations
                                    .Where(tr => tr.Language.ToLower() == lang)
                                    .Select(tr => tr.TourName)
                                    .FirstOrDefault() ?? img.Translations.FirstOrDefault().TourName
                    }).ToList(),

                    // Included / NotIncluded / Highlights
                    IncludedItems = t.Included
                                        .Where(i => i.Language.ToLower() == lang)
                                        .Select(i => new TourIncludedDto { Language = i.Language, Text = i.Text })
                                        .ToList(),
                    NotIncludedItems = t.NotIncluded
                                        .Where(i => i.Language.ToLower() == lang)
                                        .Select(i => new TourNotIncludedDto { Language = i.Language, Text = i.Text })
                                        .ToList(),
                    Highlights = t.Highlights
                                        .Where(h => h.Language.ToLower() == lang)
                                        .Select(h => new TourHighlightDto { Language = h.Language, Text = h.Text })
                                        .ToList()
                })
                .AsNoTracking() // يقلل overhead
                .FirstOrDefaultAsync();

            return tourDto;
        }

        public async Task AddTourAsync(TourCreateDto TourCreateDto)
        {

            if (!string.IsNullOrEmpty(TourCreateDto.TranslationsJson))
            {
                TourCreateDto.Translations = JsonSerializer.Deserialize<List<TourTranslationDto>>(TourCreateDto.TranslationsJson);
            }
            if (!string.IsNullOrEmpty(TourCreateDto.IncludesJson))
            {
                TourCreateDto.IncludesPoints = JsonSerializer.Deserialize<List<TourIncludedDto>>(TourCreateDto.IncludesJson);
            }
            if (!string.IsNullOrEmpty(TourCreateDto.NonIncludesJson))
            {
                TourCreateDto.NonIncludesPoints = JsonSerializer.Deserialize<List<TourNotIncludedDto>>(TourCreateDto.NonIncludesJson);
            }
            if (!string.IsNullOrEmpty(TourCreateDto.hightlightJson))
            {
                TourCreateDto.hightlightPoints = JsonSerializer.Deserialize<List<TourHighlightDto>>(TourCreateDto.hightlightJson);
            }


       

            // 🖼️ حفظ الصورة في wwwroot/images/categoryTours
            string imagePath = string.Empty;
            if (TourCreateDto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/tours");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);

                // فتح الصورة باستخدام ImageSharp
                using (var image = await Image.LoadAsync(TourCreateDto.ImageFile.OpenReadStream()))
                {
                    // تغيير الأبعاد
                    image.Mutate(x => x.Resize(1600, 900));

                    // حفظ WebP بجودة ممتازة وحجم صغير جدًا
                    await image.SaveAsync(fullPath, new WebpEncoder()
                    {
                        Quality = 80
                    });
                }

                imagePath = $"images/tours/{fileName}";
            }
            var baseSlug = SlugHelper.GenerateSlug(TourCreateDto.ReferneceName);

            // تأكد إنه Unique
            var slug = baseSlug;
            int counter = 1;

            while (await unitOfWork.Repository<Tour>()
                   .AnyAsync(t => t.Slug == slug))
            {
                slug = $"{baseSlug}-{counter}";
                counter++;
            }


            // 🧩 إنشاء الكيان
            var tour = new Tour
            {
                Slug = slug,
                ImageCover = imagePath,
                IsActive = TourCreateDto.IsActive,
                FK_DestinationID= TourCreateDto.FK_DestinationID,
                FK_CategoryID= TourCreateDto.FK_CategoryID,
                FK_UserID= TourCreateDto.FK_UserID,
                StartLocation= TourCreateDto.StartLocation,
                EndLocation= TourCreateDto.EndLocation,
                Price= TourCreateDto.Price,
                Duration= TourCreateDto.Duration,
                LanguageOptions= TourCreateDto.LanguageOptions,
                CreatedAt= DateTime.Now,
                TourImgs = new List<TourImg>(),
                Translations = TourCreateDto.Translations.Select(t => new TourTranslation
                {
                    Language = t.Language.ToLower(),
                    Title = t.Title,
                    Description = t.Description
                }).ToList(),
                Included= TourCreateDto.IncludesPoints.Select(t => new TourIncluded
                {
                    Language = t.Language.ToLower(),
                    Text = t.Text,
                }).ToList(),
                NotIncluded= TourCreateDto.NonIncludesPoints.Select(t => new TourNotIncluded
                {
                    Language = t.Language.ToLower(),
                    Text = t.Text,
                }).ToList(),
                Highlights= TourCreateDto.hightlightPoints.Select(t => new TourHighlight
                {
                    Language = t.Language.ToLower(),
                    Text = t.Text,
                }).ToList(),
                ReferneceName=TourCreateDto.ReferneceName,
                LinkVideo=TourCreateDto.LinkVideo
            };

            await unitOfWork.Repository<Tour>().AddAsync(tour);
            await unitOfWork.CompleteAsync();

            // ===============================
            //   رفع صور متعددة ImagesList
            // ===============================
            if (TourCreateDto.ImagesList != null && TourCreateDto.ImagesList.Any())
            {
                foreach (var imgDto in TourCreateDto.ImagesList)
                {
                    if (imgDto.ImageFile == null)
                        continue;

                    // رفع الصورة
                    string uploadDir = Path.Combine(env.WebRootPath, "images/tourImgs");
                    Directory.CreateDirectory(uploadDir);

                    string fileName = Guid.NewGuid() + ".webp";
                    string fullPath = Path.Combine(uploadDir, fileName);

                    // فتح الصورة باستخدام ImageSharp
                    using (var image = await Image.LoadAsync(imgDto.ImageFile.OpenReadStream()))
                    {
                        // تغيير الأبعاد
                        image.Mutate(x => x.Resize(1600, 900));

                        // حفظ WebP بجودة ممتازة وحجم صغير جدًا
                        await image.SaveAsync(fullPath, new WebpEncoder()
                        {
                            Quality = 80
                        });
                    }

                    string imageUrl = $"images/tourImgs/{fileName}";

                    // فك JSON لو موجود
                    if (!string.IsNullOrEmpty(imgDto.TranslationsJson))
                    {
                        imgDto.Translations = JsonSerializer.Deserialize<List<TourImgTranslationDto>>(
                            imgDto.TranslationsJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }

                    // إنشاء كيان الصورة
                    var tourImg = new TourImg
                    {
                        FK_TourId = tour.Id,
                        IsActive = imgDto.IsActive,
                        ImageCarouselUrl = imageUrl,
      
                        Translations = imgDto.Translations.Select(x => new TourImgTranslation
                        {
                            Language = x.Language.ToLower(),
                            Title = x.Title,
                            TourName = x.TourName
                        }).ToList()
                    };

                    await unitOfWork.Repository<TourImg>().AddAsync(tourImg);
                }

                await unitOfWork.CompleteAsync();
            }

        }


        public async Task<bool> UpdateTour(TourCreateDto dto, int id)
        {
            // استخدم الـ spec الجديد
            var spec = new TourForUpdateSpec(id);
            var tour = await unitOfWork.Repository<Tour>().GetByIdSpecTEntityTourAsync(spec);

            if (tour == null)
            {
                return false;
            }


            // ✅ فك JSON الخاص بالترجمات
            if (!string.IsNullOrEmpty(dto.TranslationsJson))
            {
                dto.Translations = JsonSerializer.Deserialize<List<TourTranslationDto>>(
                    dto.TranslationsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            if (!string.IsNullOrEmpty(dto.IncludesJson))
            {
                dto.IncludesPoints = JsonSerializer.Deserialize<List<TourIncludedDto>>(
                    dto.IncludesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            if (!string.IsNullOrEmpty(dto.NonIncludesJson))
            {
                dto.NonIncludesPoints = JsonSerializer.Deserialize<List<TourNotIncludedDto>>(
                    dto.NonIncludesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }
            if (!string.IsNullOrEmpty(dto.hightlightJson))
            {
                dto.hightlightPoints = JsonSerializer.Deserialize<List<TourHighlightDto>>(
                    dto.hightlightJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }


            // ✅ تحديث الصورة (لو تم رفع واحدة جديدة)
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/tour");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);

                // فتح الصورة باستخدام ImageSharp
                using (var image = await Image.LoadAsync(dto.ImageFile.OpenReadStream()))
                {
                    // تغيير الأبعاد
                    image.Mutate(x => x.Resize(1600, 900));

                    // حفظ WebP بجودة ممتازة وحجم صغير جدًا
                    await image.SaveAsync(fullPath, new WebpEncoder()
                    {
                        Quality = 80
                    });
                }

                tour.ImageCover = $"images/tour/{fileName}";
            }

            // ✅ تحديث الحالة
            tour.IsActive = dto.IsActive;
            tour.FK_CategoryID = dto.FK_CategoryID;
            tour.FK_DestinationID = dto.FK_DestinationID;
            tour.Price = dto.Price;
            tour.Duration = dto.Duration;
            tour.StartLocation = dto.StartLocation;
            tour.EndLocation = dto.EndLocation;
            tour.LanguageOptions = dto.LanguageOptions;
            tour.ReferneceName = dto.ReferneceName;
            tour.LinkVideo = dto.LinkVideo;



            // ✅ تحديث الترجمات حسب اللغة
            foreach (var translationDto in dto.Translations)
            {
                var existingTranslation = tour.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == translationDto.Language.ToLower());

                if (existingTranslation != null)
                {
                    existingTranslation.Title = translationDto.Title;
                    existingTranslation.Description = translationDto.Description;
                }
                else
                {
                    tour.Translations.Add(new TourTranslation
                    {
                        Id = tour.Id,
                        Language = translationDto.Language.ToLower(),
                        Title = translationDto.Title,
                        Description = translationDto.Description
                    });
                }
            }

            tour.Included.Clear();

            foreach (var dtoItem in dto.IncludesPoints)
            {
                tour.Included.Add(new TourIncluded
                {
                    TourId = tour.Id,   // ✅ مش Id
                    Language = dtoItem.Language.ToLower(),
                    Text = dtoItem.Text
                });
            }


            tour.NotIncluded.Clear();

            foreach (var dtoItem in dto.NonIncludesPoints)
            {
                tour.NotIncluded.Add(new TourNotIncluded
                {
                    TourId = tour.Id,
                    Language = dtoItem.Language.ToLower(),
                    Text = dtoItem.Text
                });
            }


            tour.Highlights.Clear();

            foreach (var dtoItem in dto.hightlightPoints)
            {
                tour.Highlights.Add(new TourHighlight
                {
                    TourId = tour.Id,
                    Language = dtoItem.Language.ToLower(),
                    Text = dtoItem.Text
                });
            }

            // ================================
            // ✅ Update Tour Gallery Images
            // ================================
            if (dto.ImagesList != null && dto.ImagesList.Any())
            {
                // 🧹 مسح الصور القديمة (Translations هتتمسح Cascade)

                tour.TourImgs.Clear();

                foreach (var imgDto in dto.ImagesList)
                {
                    if (imgDto.ImageFile == null)
                        continue;

                    // 📂 مسار الرفع
                    string uploadDir = Path.Combine(env.WebRootPath, "images/tourImgs");
                    Directory.CreateDirectory(uploadDir);

                    string fileName = Guid.NewGuid() + ".webp";
                    string fullPath = Path.Combine(uploadDir, fileName);

                    // 🖼️ معالجة الصورة
                    using (var image = await Image.LoadAsync(imgDto.ImageFile.OpenReadStream()))
                    {
                        image.Mutate(x => x.Resize(1600, 900));
                        await image.SaveAsync(fullPath, new WebpEncoder
                        {
                            Quality = 80
                        });
                    }

                    string imageUrl = $"images/tourImgs/{fileName}";

                    // 🔓 فك JSON الترجمات
                    if (!string.IsNullOrEmpty(imgDto.TranslationsJson))
                    {
                        imgDto.Translations = JsonSerializer.Deserialize<List<TourImgTranslationDto>>(
                            imgDto.TranslationsJson,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );
                    }

                    // 🧱 إنشاء كيان الصورة
                    var tourImg = new TourImg
                    {
                        FK_TourId = tour.Id,
                        IsActive = imgDto.IsActive,
                        ImageCarouselUrl = imageUrl,
                        Translations = imgDto.Translations?
                            .Select(tr => new TourImgTranslation
                            {
                                Language = tr.Language.ToLower(),
                                Title = tr.Title,
                                TourName = tr.TourName
                            }).ToList() ?? new()
                    };

                    tour.TourImgs.Add(tourImg);
                }
            }

            // ✅ تحديث الكيان
            unitOfWork.Repository<Tour>().Update(tour);
            // ✅ حفظ التغييرات
           await unitOfWork.CompleteAsync();
            return  true;
        }

        public async Task<bool> DeleteTour(int id)
        {
            var spec = new TourForUpdateSpec(id);

            var tour = unitOfWork.Repository<Tour>().GetByIdSpecTEntityAsync(spec);
            if (tour == null)
            {
                return false;
            }

            unitOfWork.Repository<Tour>().Delete(tour);

          await unitOfWork.CompleteAsync();

            return true;
        }

       

     
    }

}
