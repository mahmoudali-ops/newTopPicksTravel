using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
using TourSite.Core.DTOs.CategorToutCreateDto;
using TourSite.Core.DTOs.CategoryTour;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.CatgeoryTour;
using TourSite.Core.Specification.Tours;
using TourSite.Repository.Data.Contexts;
using TourSite.Repository.Repositories;

namespace TourSite.Service.Services.CatTours
{
    public class CategoryTourService : ICategoryTourService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private IConfiguration configuration;


        public IWebHostEnvironment env { get; }



        public CategoryTourService(IUnitOfWork _unitOfWork, IMapper _mapper, IWebHostEnvironment _env,IConfiguration _configuration)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            env = _env;
            configuration = _configuration;
        }

        public async Task<PageinationResponse<CategorToutAllDto>> GetAllCatToursAsync(CatTourSpecParams specParams)
        {
            var allowedLangs = new[] { "en", "de", "nl" };
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            string lang = specParams.Lang.ToLower();

            var query = unitOfWork.Repository<CategoryTour>()
                .Query()
                .Where(c => c.IsActive) // ✅ شرط client
                .AsNoTracking();

            // ✅ Pagination
            int skip = specParams.pageSize * (specParams.pageIndex - 1);
            int take = specParams.pageSize;

            // ✅ Projection مباشر
            var dataQuery = query
                .OrderBy(c => c.Id)
                .Skip(skip)
                .Take(take)
                .Select(c => new CategorToutAllDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    ReferneceName = c.ReferneceName,
                    ImageCover = $"{configuration["BaseUrl"]}{c.ImageCover}",
                    IsActive = c.IsActive,

                    Titles = c.Translations
                        .Where(tr => tr.Language.ToLower() == lang)
                        .Select(tr => tr.Title)
                        .FirstOrDefault()
                        ?? c.Translations.FirstOrDefault().Title,

                    Descriptions = c.Translations
                        .Where(tr => tr.Language.ToLower() == lang)
                        .Select(tr => tr.Description)
                        .FirstOrDefault()
                        ?? c.Translations.FirstOrDefault().Description
                });

            var data = await dataQuery.ToListAsync();

            // ✅ Count خفيف جدًا
            int totalCount = await query.CountAsync();

            return new PageinationResponse<CategorToutAllDto>(
                specParams.pageIndex,
                specParams.pageSize,
                totalCount,
                data
            );
        }

        public async Task<PageinationResponse<CategorToutAllDto>> GetAllCatToursAdminAsync(CatTourSpecParams cattourSpecParams)
        {
            var allowedLangs = new[] { "en", "de", "nl" };
            if (string.IsNullOrEmpty(cattourSpecParams.Lang) || !allowedLangs.Contains(cattourSpecParams.Lang.ToLower()))
                cattourSpecParams.Lang = "en";

            string lang = cattourSpecParams.Lang.ToLower();

            var query = unitOfWork.Repository<CategoryTour>()
                .Query()
                .AsNoTracking();

            // ✅ Pagination
            int skip = cattourSpecParams.pageSize * (cattourSpecParams.pageIndex - 1);
            int take = cattourSpecParams.pageSize;

            // ✅ Projection مباشر
            var dataQuery = query
                .OrderBy(c => c.Id)
                .Skip(skip)
                .Take(take)
                .Select(c => new CategorToutAllDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    ReferneceName = c.ReferneceName,
                    ImageCover = $"{configuration["BaseUrl"]}{c.ImageCover}",
                    IsActive = c.IsActive,

                    Titles = c.Translations
                        .Where(tr => tr.Language.ToLower() == lang)
                        .Select(tr => tr.Title)
                        .FirstOrDefault()
                        ?? c.Translations.FirstOrDefault().Title,

                    Descriptions = c.Translations
                        .Where(tr => tr.Language.ToLower() == lang)
                        .Select(tr => tr.Description)
                        .FirstOrDefault()
                        ?? c.Translations.FirstOrDefault().Description
                });

            var data = await dataQuery.ToListAsync();

            // ✅ Count خفيف جدًا
            int totalCount = await query.CountAsync();

            return new PageinationResponse<CategorToutAllDto>(
                cattourSpecParams.pageIndex,
                cattourSpecParams.pageSize,
                totalCount,
                data
            );
        }

        public async Task<CategorToutDto> GetCatTourByIdAsync(string slug, string? lang = "en")
        {
            lang ??= "en";
            lang = lang.ToLower();

            var categoryDto = await unitOfWork.Repository<CategoryTour>()
                .Query()
                .Where(c => c.Slug == slug && c.IsActive)
                .Select(c => new CategorToutDto
                {
                    Id = c.Id,
                    Slug = c.Slug,
                    ImageCover = $"{configuration["BaseUrl"]}{c.ImageCover}",
                    IsActive = c.IsActive,
                    ReferneceName = c.ReferneceName,

                    Titles = c.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Title)
                                .FirstOrDefault() ?? c.Translations.FirstOrDefault().Title,
                    MetaKeyWords = c.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.MetaKeyWords)
                                .FirstOrDefault() ?? c.Translations.FirstOrDefault().MetaKeyWords,
                    MetaDescription = c.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.MetaDescription)
                                .FirstOrDefault() ?? c.Translations.FirstOrDefault().MetaDescription,

                    Descriptions = c.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Description)
                                .FirstOrDefault() ?? c.Translations.FirstOrDefault().Description,

                    // جلب الـ Tours التابعة للـ Category
                    Tours = c.Tours
                            .Where(t => t.IsActive)
                            .Select(t => new TourDto
                            {
                                Id = t.Id,
                                Slug = t.Slug,
                                Price = t.Price,
                                Duration = t.Duration,
                                StartLocation = t.StartLocation,
                                EndLocation = t.EndLocation,
                                ReferneceName = t.ReferneceName,
                                IsActive=t.IsActive,

                                ImageCover = $"{configuration["BaseUrl"]}{t.ImageCover}",
                                Titles = t.Translations
                                            .Where(tr => tr.Language.ToLower() == lang)
                                            .Select(tr => tr.Title)
                                            .FirstOrDefault() ?? t.Translations.FirstOrDefault().Title,
                                Descriptions = t.Translations
                                            .Where(tr => tr.Language.ToLower() == lang)
                                            .Select(tr => tr.Description)
                                            .FirstOrDefault() ?? t.Translations.FirstOrDefault().Description,
                            }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return categoryDto;
        }


        public async  Task AddCatTourAsync(CategorToutCreateDto categorToutDto)
        {

            var test = categorToutDto.ImageFile; // ضع breakpoint هنا


            if (!string.IsNullOrEmpty(categorToutDto.TranslationsJson))
            {
                categorToutDto.Translations = JsonSerializer.Deserialize<List<CategoryTourTranslationDto>>(categorToutDto.TranslationsJson);
            }

            // 🖼️ حفظ الصورة في wwwroot/images/categoryTours
            string imagePath = string.Empty;
            if (categorToutDto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/categoryTours");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);

                // فتح الصورة باستخدام ImageSharp
                using (var image = await Image.LoadAsync(categorToutDto.ImageFile.OpenReadStream()))
                {
                    // تغيير الأبعاد
                    image.Mutate(x => x.Resize(1600, 900));

                    // حفظ WebP بجودة ممتازة وحجم صغير جدًا
                    await image.SaveAsync(fullPath, new WebpEncoder()
                    {
                        Quality = 80
                    });
                }

                imagePath = $"images/categoryTours/{fileName}";
            }
            var baseSlug = SlugHelper.GenerateSlug(categorToutDto.ReferneceName);

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
            var CategoryTour = new CategoryTour
            {
                Slug = slug,
                ImageCover = imagePath,
                IsActive = categorToutDto.IsActive,
                ReferneceName = categorToutDto.ReferneceName,
                Translations = categorToutDto.Translations.Select(t => new CategoryTourTranslation
                {
                    Language = t.Language.ToLower(),
                    Title = t.Title,
                    Description = t.Description,
                    MetaDescription = t.MetaDescription,
                    MetaKeyWords = t.MetaKeyWords

                }).ToList()
            };

            await unitOfWork.Repository<CategoryTour>().AddAsync(CategoryTour);
            await unitOfWork.CompleteAsync();
        }

        public  async Task<Boolean> UpdateCatTour(CategorToutCreateDto dto, int id)
        {

            // استخدم الـ spec الجديد
            var spec = new CategoryTourForUpdateSpec(id);
            var categoryTour = unitOfWork.Repository<CategoryTour>().GetByIdSpecTEntityAsync(spec);

            if (categoryTour == null)
            {
                return false ;
            }


            // ✅ فك JSON الخاص بالترجمات
            if (!string.IsNullOrEmpty(dto.TranslationsJson))
            {
                dto.Translations = JsonSerializer.Deserialize<List<CategoryTourTranslationDto>>(
                    dto.TranslationsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }


            // ✅ تحديث الصورة (لو تم رفع واحدة جديدة)
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/categoryTours");
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

                categoryTour.ImageCover = $"images/categoryTours/{fileName}";
            }

            // ✅ تحديث الحالة
            categoryTour.IsActive = dto.IsActive;
            categoryTour.ReferneceName = dto.ReferneceName;

            // ✅ تحديث الترجمات حسب اللغة
            foreach (var translationDto in dto.Translations)
            {
                var existingTranslation = categoryTour.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == translationDto.Language.ToLower());

                if (existingTranslation != null)
                {
                    existingTranslation.Title = translationDto.Title;
                    existingTranslation.Description = translationDto.Description;
                    existingTranslation.Language = translationDto.Language.ToLower();
                    existingTranslation.MetaDescription = translationDto.MetaDescription;
                    existingTranslation.MetaKeyWords = translationDto.MetaKeyWords;



                }
                else
                {
                    categoryTour.Translations.Add(new CategoryTourTranslation
                    {
                        CategoryTourId=categoryTour.Id,
                        Language = translationDto.Language.ToLower(),
                        Title = translationDto.Title,
                        Description = translationDto.Description,
                        MetaDescription = translationDto.MetaDescription,
                        MetaKeyWords = translationDto.MetaKeyWords
                    });
                }
            }

            // ✅ تحديث الكيان
            unitOfWork.Repository<CategoryTour>().Update(categoryTour);
            // ✅ حفظ التغييرات
          await   unitOfWork.CompleteAsync();
            return true;

        }

        public async Task<Boolean>  DeleteCatTour(int id)
        {
            var spec = new CategoryTourForUpdateSpec(id);
        
            var categoryTour = unitOfWork.Repository<CategoryTour>().GetByIdSpecTEntityAsync(spec);
            if (categoryTour == null)
            {
                return false;
            }

            unitOfWork.Repository<CategoryTour>().Delete(categoryTour);

          await  unitOfWork.CompleteAsync();

            return true;
        }

    }
}
