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
using TourSite.Core.DTOs.Destnation;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.DTOs.Transfer;
using TourSite.Core.DTOs.User;
using TourSite.Core.Entities;
using TourSite.Core.Helper;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.Destnations;
using TourSite.Core.Specification.Transfers;
using TourSite.Core.Specification.Users;

namespace TourSite.Service.Services.Trasnfers
{
    public class TransferService : ITransferService
    {

        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private IConfiguration configuration;

        public IWebHostEnvironment env { get; }


        public TransferService(IUnitOfWork _unitOfWork, IMapper _mapper, IWebHostEnvironment _env,IConfiguration _configuration)
        {
            unitOfWork = _unitOfWork;
            mapper = _mapper;
            env = _env;
            configuration = _configuration;
        }



        public async Task<PageinationResponse<TransferAllDto>> GetAllTransToursAsync(TrasferSpecParam specParams)
        {
            var allowedLangs = new[] { "en", "ar" };
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            var spec = new TransferSpecification(specParams);
            var allData = await unitOfWork.Repository<Transfer>().GetAllSpecAsync(spec);

            if (allData is null) return null;

            // ✅ استخدم AutoMapper مباشرة مع الـ language
            var data = mapper.Map<IEnumerable<TransferAllDto>>(allData, opt =>
                opt.Items["Lang"] = specParams.Lang.ToLower()
            );

            // Count بدون ترجمات
            var CountSpec = new TransferWithCountSpecifications(specParams);
            var Count = await unitOfWork.Repository<Transfer>().GetCountAsync(CountSpec);

            return new PageinationResponse<TransferAllDto>(specParams.pageIndex, specParams.pageSize, Count, data);

        }


        public async Task<TransferDto> GetCatTransByIdAsync(string slug, string? lang = "en")
        {
            lang ??= "en";
            lang = lang.ToLower();

            var transferDto = await unitOfWork.Repository<Transfer>()
                .Query()
                .Where(t => t.Slug == slug && t.IsActive)
                .Select(t => new TransferDto
                {
                    Id = t.Id,
                    Slug = t.Slug,
                    ReferneceName = t.ReferneceName,
                    ImageCover = $"{configuration["BaseUrl"]}{t.ImageCover}",
                    IsActive = t.IsActive,

                    FK_DestinationID = t.FK_DestinationID,
                    DestinationName = t.Destination.Translations
                                        .Where(d => d.Language.ToLower() == lang)
                                        .Select(d => d.Name)
                                        .FirstOrDefault(),

                    // الترجمات حسب اللغة
                    Names = t.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Name)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Name,
                    Descriptions = t.Translations
                                .Where(tr => tr.Language.ToLower() == lang)
                                .Select(tr => tr.Description)
                                .FirstOrDefault() ?? t.Translations.FirstOrDefault().Description,

                    // Included / NotIncluded / Highlights
                    Includeds = t.Includeds
                                        .Where(i => i.Language.ToLower() == lang)
                                        .Select(i => new TransferIncludedDto { Language = i.Language, Text = i.Text })
                                        .ToList(),
                    NotIncludeds = t.NotIncludeds
                                        .Where(i => i.Language.ToLower() == lang)
                                        .Select(i => new TransferNotIncludedDto { Language = i.Language, Text = i.Text })
                                        .ToList(),
                    Highlights = t.Highlights
                                        .Where(h => h.Language.ToLower() == lang)
                                        .Select(h => new TransferHighlightDto { Language = h.Language, Text = h.Text })
                                        .ToList(),

                    // PricesList حسب اللغة
                    PricesList = t.PricesList
                                        .Where(p => p.Language.ToLower() == lang)
                                        .Select(p => new TransferPricesDTO
                                        {
                                            Id = p.Id,
                                            ReferneceName = p.ReferneceName,
                                            Title = p.Title,
                                            PrivtePrice = p.PrivtePrice,
                                            SharedPrice = p.SharedPrice,
                                            Language = p.Language
                                        }).ToList()
                })
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return transferDto;
        }


        public async Task AddTransferAsync(TransferCreateDto TransferDto)
        {
            if (!string.IsNullOrEmpty(TransferDto.TranslationsJson))
            {
                TransferDto.Translations = JsonSerializer.Deserialize<List<TransferTranslationDto>>(TransferDto.TranslationsJson);
            }

            if (!string.IsNullOrEmpty(TransferDto.PriecesListJson))
            {
                TransferDto.PricesList = JsonSerializer.Deserialize<List<TransferPricesDTO>>(TransferDto.PriecesListJson);
            }

            if (!string.IsNullOrEmpty(TransferDto.IncludesJson))
            {
                TransferDto.Includeds = JsonSerializer.Deserialize<List<TransferIncludedDto>>(TransferDto.IncludesJson);
            }

            if (!string.IsNullOrEmpty(TransferDto.NonIncludesJson))
            {
                TransferDto.NotIncludeds = JsonSerializer.Deserialize<List<TransferNotIncludedDto>>(TransferDto.NonIncludesJson);
            }

            if (!string.IsNullOrEmpty(TransferDto.hightlightJson))
            {
                TransferDto.Highlights = JsonSerializer.Deserialize<List<TransferHighlightDto>>(TransferDto.hightlightJson);
            }

            // 🖼️ حفظ الصورة في wwwroot/images/categoryTours
            string imagePath = string.Empty;
            if (TransferDto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/transfers");
                Directory.CreateDirectory(uploadDir);

                string fileName = Guid.NewGuid() + ".webp";
                string fullPath = Path.Combine(uploadDir, fileName);

                // فتح الصورة باستخدام ImageSharp
                using (var image = await Image.LoadAsync(TransferDto.ImageFile.OpenReadStream()))
                {
                    // تغيير الأبعاد
                    image.Mutate(x => x.Resize(1600, 900));

                    // حفظ WebP بجودة ممتازة وحجم صغير جدًا
                    await image.SaveAsync(fullPath, new WebpEncoder()
                    {
                        Quality = 80
                    });
                }

                imagePath = $"images/transfers/{fileName}";
            }
            var baseSlug = SlugHelper.GenerateSlug(TransferDto.ReferneceName);

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
            var transfer = new Transfer
            {   Slug= slug,
                ImageCover = imagePath,
                IsActive = TransferDto.IsActive,
                FK_DestinationID = TransferDto.FK_DestinationID,
                ReferneceName = TransferDto.ReferneceName,
                Translations = TransferDto.Translations.Select(t => new TransferTranslation
                {
                    Language = t.Language.ToLower(),
                    Name = t.Name,
                    Description = t.Description
                }).ToList(),
                PricesList = TransferDto.PricesList.Select(t => new TrasnferPrices
                {
                    Language = t.Language.ToLower(),
                    Title= t.Title,
                    PrivtePrice=t.PrivtePrice,
                    SharedPrice=t.SharedPrice
                }).ToList(),
                Includeds= TransferDto.Includeds.Select(t => new TransferIncludes
                {
                    Language = t.Language.ToLower(),
                    Text = t.Text,
                }).ToList(),
                NotIncludeds=TransferDto.NotIncludeds.Select(t => new TransferNotIncludes
                {
                    Language = t.Language.ToLower(),
                    Text = t.Text,
                }).ToList(),
                Highlights = TransferDto.Highlights.Select(t => new TransferIHighlights
                {
                    Language = t.Language.ToLower(),
                    Text = t.Text,
                }).ToList(),
                

            };

            await unitOfWork.Repository<Transfer>().AddAsync(transfer);
            await unitOfWork.CompleteAsync();
        }

        public async Task<bool> UpdateTransfer(TransferCreateDto dto, int id)
        {
            // استخدم الـ spec الجديد
            var spec = new TransferForUpdateSpec(id);
            var transfer = unitOfWork.Repository<Transfer>().GetByIdSpecTEntityAsync(spec);

            if (transfer == null)
            {
                return false;
            }


            // ✅ فك JSON الخاص بالترجمات
            if (!string.IsNullOrEmpty(dto.TranslationsJson))
            {
                dto.Translations = JsonSerializer.Deserialize<List<TransferTranslationDto>>(
                    dto.TranslationsJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }

            if (!string.IsNullOrEmpty(dto.PriecesListJson))
            {
                dto.PricesList = JsonSerializer.Deserialize<List<TransferPricesDTO>>(
                    dto.PriecesListJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }

            if (!string.IsNullOrEmpty(dto.IncludesJson))
            {
                dto.Includeds = JsonSerializer.Deserialize<List<TransferIncludedDto>>(
                    dto.IncludesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }

            if (!string.IsNullOrEmpty(dto.NonIncludesJson))
            {
                dto.NotIncludeds = JsonSerializer.Deserialize<List<TransferNotIncludedDto>>(
                    dto.NonIncludesJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }

            if (!string.IsNullOrEmpty(dto.hightlightJson))
            {
                dto.Highlights = JsonSerializer.Deserialize<List<TransferHighlightDto>>(
                    dto.hightlightJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
            }


            // ✅ تحديث الصورة (لو تم رفع واحدة جديدة)
            if (dto.ImageFile != null)
            {
                string uploadDir = Path.Combine(env.WebRootPath, "images/transfers");
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

                transfer.ImageCover = $"images/transfers/{fileName}";
            }

            // ✅ تحديث الحالة
            transfer.IsActive = dto.IsActive;
            transfer.FK_DestinationID = dto.FK_DestinationID;
            transfer.ReferneceName = dto.ReferneceName;


            // ✅ تحديث الترجمات حسب اللغة
            foreach (var translationDto in dto.Translations)
            {
                var existingTranslation = transfer.Translations
                    .FirstOrDefault(t => t.Language.ToLower() == translationDto.Language.ToLower());

                if (existingTranslation != null)
                {
                    existingTranslation.Name = translationDto.Name;
                    existingTranslation.Description = translationDto.Description;
                }
                else
                {
                    transfer.Translations.Add(new TransferTranslation
                    {
                        TransferId=transfer.Id,
                        Language = translationDto.Language.ToLower(),
                        Name = translationDto.Name,
                        Description = translationDto.Description
                    });
                }
            }

            transfer.PricesList.Clear();

            foreach (var dtoItem in dto.PricesList)
            {
                transfer.PricesList.Add(new TrasnferPrices
                {
                    TransferId = transfer.Id,
                    Language = dtoItem.Language.ToLower(),
                    Title = dtoItem.Title,
                    SharedPrice = dtoItem.SharedPrice,
                    PrivtePrice = dtoItem.PrivtePrice
                });
            }


            transfer.Includeds.Clear();

            foreach (var dtoItem in dto.Includeds)
            {
                transfer.Includeds.Add(new TransferIncludes
                {
                    TransferId = transfer.Id,
                    Language = dtoItem.Language.ToLower(),
                    Text = dtoItem.Text
                });
            }


            transfer.NotIncludeds.Clear();

            foreach (var dtoItem in dto.NotIncludeds)
            {
                transfer.NotIncludeds.Add(new TransferNotIncludes
                {
                    TransferId = transfer.Id,
                    Language = dtoItem.Language.ToLower(),
                    Text = dtoItem.Text
                });
            }



            transfer.Highlights.Clear();

            foreach (var dtoItem in dto.Highlights)
            {
                transfer.Highlights.Add(new TransferIHighlights
                {
                    TransferId = transfer.Id,
                    Language = dtoItem.Language.ToLower(),
                    Text = dtoItem.Text
                });
            }

            // ✅ تحديث الكيان
            unitOfWork.Repository<Transfer>().Update(transfer);
            // ✅ حفظ التغييرات
          await  unitOfWork.CompleteAsync();
            return true;
        }

        public  async Task<bool> Deletetransfer(int id)
        {
            var spec = new TransferForUpdateSpec(id);

            var transfer = unitOfWork.Repository<Transfer>().GetByIdSpecTEntityAsync(spec);
            if (transfer == null)
            {
                return false;
            }

            unitOfWork.Repository<Transfer>().Delete(transfer);

           await unitOfWork.CompleteAsync();

            return true;
        }

        public async Task<PageinationResponse<TransferAllDto>> GetAllTransToursAdminAsync(TrasferSpecParam specParams)
        {
            var allowedLangs = new[] { "en", "ar" };
            if (string.IsNullOrEmpty(specParams.Lang) || !allowedLangs.Contains(specParams.Lang.ToLower()))
                specParams.Lang = "en";

            var spec = new TrasnsferSpecificationForAdmin(specParams);
            var allData = await unitOfWork.Repository<Transfer>().GetAllSpecAsync(spec);

            if (allData is null) return null;

            // ✅ استخدم AutoMapper مباشرة مع الـ language
            var data = mapper.Map<IEnumerable<TransferAllDto>>(allData, opt =>
                opt.Items["Lang"] = specParams.Lang.ToLower()
            );

            // Count بدون ترجمات
            var CountSpec = new TransferWithCountSpecifications(specParams);
            var Count = await unitOfWork.Repository<Transfer>().GetCountAsync(CountSpec);

            return new PageinationResponse<TransferAllDto>(specParams.pageIndex, specParams.pageSize, Count, data);
        }
    }
}