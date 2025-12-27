using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
using TourSite.Core.DTOs.CategoryTour;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.CatgeoryTour;
using TourSite.Core.Specification.TourImgs;

namespace TourSite.Service.Services.TourImgs
{
    public class TourImgService : ITourImgService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public IWebHostEnvironment env { get; }


        public TourImgService(IUnitOfWork _unitOfWork, IMapper _mapper, IWebHostEnvironment _env)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            env = _env;
        }

        

        public async Task<PageinationResponse<TourImgDto>> GetAllTourImgsAsync(TourImgSpeciParams specParams)
        {
            var allowedLangs = new[] { "en", "ar" };
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            var spec = new TourImgSpecification(specParams);
            var allData = await unitOfWork.Repository<TourImg>().GetAllSpecAsync(spec);

            if (allData is null) return null;

            // ✅ استخدم AutoMapper مباشرة مع الـ language
            var data = mapper.Map<IEnumerable<TourImgDto>>(allData, opt =>
                opt.Items["Lang"] = specParams.Lang.ToLower()
            );

            // Count بدون ترجمات
            var CountSpec = new TourImgWithCountSpecifications(specParams);
            var Count = await unitOfWork.Repository<TourImg>().GetCountAsync(CountSpec);

            return new PageinationResponse<TourImgDto>(specParams.pageIndex, specParams.pageSize, Count, data);
        }

        public async Task<PageinationResponse<TourImgDto>> GetAllTourImgsAdminAsync(TourImgSpeciParams specParams)
        {
            var allowedLangs = new[] { "en", "ar" };
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            var spec = new TourImgSpecificationForAdmin(specParams);
            var allData = await unitOfWork.Repository<TourImg>().GetAllSpecAsync(spec);

            if (allData is null) return null;

            // ✅ استخدم AutoMapper مباشرة مع الـ language
            var data = mapper.Map<IEnumerable<TourImgDto>>(allData, opt =>
                opt.Items["Lang"] = specParams.Lang.ToLower()
            );

            // Count بدون ترجمات
            var CountSpec = new TourImgWithCountSpecifications(specParams);
            var Count = await unitOfWork.Repository<TourImg>().GetCountAsync(CountSpec);

            return new PageinationResponse<TourImgDto>(specParams.pageIndex, specParams.pageSize, Count, data);
        }

        public async Task<TourImgDto> GetTourImgByIdAsync(int id,string? lang="en")
        {
            var spec = new TourImgSpecification(id);
            var CategoryTour = await unitOfWork.Repository<TourImg>().GetByIdSpecAsync(spec);
            lang ??= "en";

            return mapper.Map<TourImgDto>(CategoryTour, opt => opt.Items["Lang"] = lang.ToLower());
        }

        public async Task AddImgTourAsync(TourImgCreateDto ImgTourToutDto)
        {

            if (!string.IsNullOrEmpty(ImgTourToutDto.TranslationsJson))
            {
                ImgTourToutDto.Translations = JsonSerializer.Deserialize<List<TourImgTranslationDto>>(ImgTourToutDto.TranslationsJson);
            }

            // 🖼️ حفظ الصورة في wwwroot/images/categoryTours
            string imagePath = string.Empty;
            if (ImgTourToutDto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/tourImgs");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);

                // فتح الصورة باستخدام ImageSharp
                using (var image = await Image.LoadAsync(ImgTourToutDto.ImageFile.OpenReadStream()))
                {
                    // تغيير الأبعاد
                    image.Mutate(x => x.Resize(1600, 900));

                    // حفظ WebP بجودة ممتازة وحجم صغير جدًا
                    await image.SaveAsync(fullPath, new WebpEncoder()
                    {
                        Quality = 80
                    });
                }

                imagePath = $"images/tourImgs/{fileName}";
            }


            // 🧩 إنشاء الكيان
            var tourImg = new TourImg
            {
                ImageCarouselUrl = imagePath,
                IsActive = ImgTourToutDto.IsActive,
                FK_TourId = ImgTourToutDto.FK_TourId,
                
                Translations = ImgTourToutDto.Translations.Select(t => new TourImgTranslation
                {
                    Language = t.Language.ToLower(),
                    Title = t.Title,
                    TourName = t.TourName
                }).ToList()
            };

            await unitOfWork.Repository<TourImg>().AddAsync(tourImg);
            await unitOfWork.CompleteAsync();
        }

   
        public async Task<bool> UpdatImgTour(TourImgCreateDto dto, int id)
        {
            // استخدم الـ spec الجديد
            var spec = new TourImgForUpdateSpec(id);
            var tourimg = unitOfWork.Repository<TourImg>().GetByIdSpecTEntityAsync(spec);

            if (tourimg == null)
            {
                return false;
            }


            // ✅ فك JSON الخاص بالترجمات
            if (!string.IsNullOrEmpty(dto.TranslationsJson))
            {
                dto.Translations = JsonSerializer.Deserialize<List<TourImgTranslationDto>>(
                    dto.TranslationsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }


            // ✅ تحديث الصورة (لو تم رفع واحدة جديدة)
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/tourImgs");
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

                tourimg.ImageCarouselUrl = $"images/tourImgs/{fileName}";
            }

            // ✅ تحديث الحالة
            tourimg.IsActive = dto.IsActive;
            tourimg.FK_TourId = dto.FK_TourId;
            // ✅ تحديث الترجمات حسب اللغة
            foreach (var translationDto in dto.Translations)
            {
                var existingTranslation = tourimg.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == translationDto.Language.ToLower());

                if (existingTranslation != null)
                {
                    existingTranslation.Title = translationDto.Title;
                }
                else
                {
                    tourimg.Translations.Add(new TourImgTranslation
                    {
                        FK_TourImgId= tourimg.Id,
                        Language = translationDto.Language.ToLower(),
                        Title = translationDto.Title,
                    });
                }
            }

            // ✅ تحديث الكيان
            unitOfWork.Repository<TourImg>().Update(tourimg);
            // ✅ حفظ التغييرات
           await  unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteImgTour(int id)
        {
            var spec = new TourImgForUpdateSpec(id);

            var tourimg = unitOfWork.Repository<TourImg>().GetByIdSpecTEntityAsync(spec);
            if (tourimg == null)
            {
                return false;
            }

            unitOfWork.Repository<TourImg>().Delete(tourimg);

           await unitOfWork.CompleteAsync();

            return true;
        }

    }
}
