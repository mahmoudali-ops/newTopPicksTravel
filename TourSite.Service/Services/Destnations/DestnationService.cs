using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
using TourSite.Core.DTOs.Destnation;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.CatgeoryTour;
using TourSite.Core.Specification.Destnations;

namespace TourSite.Service.Services.Destnations
{
    public class DestnationService : IDestinationService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        public IWebHostEnvironment env { get; }


        public DestnationService(IUnitOfWork _unitOfWork, IMapper _mapper, IWebHostEnvironment _env)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            env = _env;
        }

        
     

        public async Task<PageinationResponse<DestnationAllDto>> GetAllDestToursAsync(DestnationSpecParams specParams)
        {
            var allowedLangs = new[] { "en", "ar" };
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            var spec = new DestnationSpecification(specParams);
            var allData = await unitOfWork.Repository<Destination>().GetAllSpecAsync(spec);

            if (allData is null) return null;

            // ✅ استخدم AutoMapper مباشرة مع الـ language
            var data = mapper.Map<IEnumerable<DestnationAllDto>>(allData, opt =>
                opt.Items["Lang"] = specParams.Lang.ToLower()
            );

            // Count بدون ترجمات
            var CountSpec = new DestnationWitCountSpecification(specParams);
            var Count = await unitOfWork.Repository<Destination>().GetCountAsync(CountSpec);

            return new PageinationResponse<DestnationAllDto>(specParams.pageIndex, specParams.pageSize, Count, data);
        }

        public async Task<PageinationResponse<DestnationAllDto>> GetAllDestToursAdminAsync(DestnationSpecParams DestnationSpecParams)
        {
            var allowedLangs = new[] { "en", "ar" };
            if (string.IsNullOrEmpty(DestnationSpecParams.Lang) || !allowedLangs.Contains(DestnationSpecParams.Lang.ToLower()))
                DestnationSpecParams.Lang = "en";

            var spec = new DestnationSpecificationForAdmin(DestnationSpecParams);
            var allData = await unitOfWork.Repository<Destination>().GetAllSpecAsync(spec);

            if (allData is null) return null;

            // ✅ استخدم AutoMapper مباشرة مع الـ language
            var data = mapper.Map<IEnumerable<DestnationAllDto>>(allData, opt =>
                opt.Items["Lang"] = DestnationSpecParams.Lang.ToLower()
            );

            // Count بدون ترجمات
            var CountSpec = new DestnationWitCountSpecification(DestnationSpecParams);
            var Count = await unitOfWork.Repository<Destination>().GetCountAsync(CountSpec);

            return new PageinationResponse<DestnationAllDto>(DestnationSpecParams.pageIndex, DestnationSpecParams.pageSize, Count, data);
        }

        public async Task<DestnationDto> GetDestByIdAsync(int id, string? lang = "en")
        {
            var spec = new DestnationSpecification(id);
            var CategoryTour = await unitOfWork.Repository<Destination>().GetByIdSpecAsync(spec);
            lang ??= "en";

            return mapper.Map<DestnationDto>(CategoryTour, opt => opt.Items["Lang"] = lang.ToLower());
        }

        public async Task AddDestnationAsync(DestnationToutCreateDto DestCreateDto)
        {
            if (!string.IsNullOrEmpty(DestCreateDto.TranslationsJson))
            {
                DestCreateDto.Translations = JsonSerializer.Deserialize<List<DestnaionTranslationDto>>(DestCreateDto.TranslationsJson);
            }


            // 🖼️ حفظ الصورة في wwwroot/images/categoryTours
            string imagePath = string.Empty;

            if (DestCreateDto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/destinations");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + Path.GetExtension(DestCreateDto.ImageFile.FileName);
                string fullPath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await DestCreateDto.ImageFile.CopyToAsync(stream);
                }

                imagePath = $"images/destinations/{fileName}";
            }

            // 🧩 إنشاء الكيان
            var destination = new Destination
            {
                ImageCover = imagePath,
                IsActive = DestCreateDto.IsActive,
                ReferneceName = DestCreateDto.ReferneceName,

                Translations = DestCreateDto.Translations.Select(t => new DestinationTranslation
                {
                    Language = t.Language.ToLower(),
                    Name = t.Name,
                    Description = t.Description
                }).ToList()
            };

            await unitOfWork.Repository<Destination>().AddAsync(destination);
            await unitOfWork.CompleteAsync();
        }


        public async Task<bool> UpdateDest(DestnationToutCreateDto dto, int id)
        {
            // استخدم الـ spec الجديد
            var spec = new DestnationForUpdateSpec(id);
            var destination = unitOfWork.Repository<Destination>().GetByIdSpecTEntityAsync(spec);

            if (destination == null)
            {
                return false;
            }


            // ✅ فك JSON الخاص بالترجمات
            if (!string.IsNullOrEmpty(dto.TranslationsJson))
            {
                dto.Translations = JsonSerializer.Deserialize<List<DestnaionTranslationDto>>(
                    dto.TranslationsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }


            // ✅ تحديث الصورة (لو تم رفع واحدة جديدة)
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/destinations");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + Path.GetExtension(dto.ImageFile.FileName);
                string fullPath = Path.Combine(uploadDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                   await dto.ImageFile.CopyToAsync(stream);
                }

                destination.ImageCover = $"images/destnations/{fileName}";
            }

            // ✅ تحديث الحالة
            destination.IsActive = dto.IsActive;
            destination.ReferneceName = dto.ReferneceName;

            // ✅ تحديث الترجمات حسب اللغة
            foreach (var translationDto in dto.Translations)
            {
                var existingTranslation = destination.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == translationDto.Language.ToLower());

                if (existingTranslation != null)
                {
                    existingTranslation.Name = translationDto.Name;
                    existingTranslation.Description = translationDto.Description;
                }
                else
                {
                    destination.Translations.Add(new DestinationTranslation
                    {
                        DestinationId=destination.Id,
                        Language = translationDto.Language.ToLower(),
                        Name = translationDto.Name,
                        Description = translationDto.Description
                    });
                }
            }

            // ✅ تحديث الكيان
            unitOfWork.Repository<Destination>().Update(destination);
            // ✅ حفظ التغييرات
          await  unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<bool> DeleteDest(int id)
        {
            var spec = new DestnationForUpdateSpec(id);

            var destination = unitOfWork.Repository<Destination>().GetByIdSpecTEntityAsync(spec);
            if (destination == null)
            {
                return false;
            }

            unitOfWork.Repository<Destination>().Delete(destination);

            await unitOfWork.CompleteAsync();

            return true;
        }

    }
}
