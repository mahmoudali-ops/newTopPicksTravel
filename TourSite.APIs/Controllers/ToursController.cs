using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourSite.APIs.Errors;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.TourImg;
using TourSite.Core.DTOs.Tours;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.Tours;
using TourSite.Repository.Data.Contexts;
using TourSite.Service.Services.TourImgs;

namespace TourSite.APIs.Controllers
{
    
    public class ToursController : BaseApiController
    {
        private readonly IToursService toursService;

        private readonly TourDbContext _context;
        public ToursController(IToursService _toursService, TourDbContext context)
        {
            toursService = _toursService;
            _context = context;
        }
        [HttpGet("client")]
        public async Task<IActionResult> GetAllTours([FromQuery] TourSpecParams tourSpecParams)
        {
            var tours = await toursService.GetAllToursAsync(tourSpecParams);
            return Ok(tours);
        }
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllTrueTours([FromQuery] TourSpecParams tourSpecParams)
        {
            var tours = await toursService.GetAllToursTrueAsync(tourSpecParams);
            return Ok(tours);
        }

        [HttpGet("by-slug/{slug}")]
        public async Task<IActionResult> GetTourBySlug(string slug, [FromQuery] string? lang)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest(new APIErrerResponse(400, "Slug is required"));

            var tour = await toursService.GetTourByIdAsync(slug, lang);

            if (tour == null)
                return NotFound(new APIErrerResponse(404, "Tour not found"));

            return Ok(tour);
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateTour([FromForm] TourCreateDto dto)
        {
            await toursService.AddTourAsync(dto);
            return Ok(new { message = "Tour created successfully" });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdataTour([FromForm] TourCreateDto dto, int id)
        {
            if (id <= 0)
                return BadRequest(new APIErrerResponse(400, "Id required .. can not be less than or equal 0"));

            var result = await toursService.UpdateTour(dto, id);

            if (!result)
            {
                return NotFound(new APIErrerResponse(404, $"There is no Tour with this Id : {id}"));
            }

            return Ok(new { message = "Tour updated successfully" });
        }


        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTour(int id)
        {
            if (id <= 0) return BadRequest(new APIErrerResponse(400, "Id required .. can not be less than or equal 0"));
            var result =await toursService.DeleteTour(id);
            if (!result)
            {
                return NotFound(new APIErrerResponse(404, $"There is no Tour with this Id : {id}"));
            }
            return Ok(new { message = "Tour  deleted successfully" });
        }

        [HttpGet("get-tour-for-update/{id}")]
        public async Task<IActionResult> GetTourForUpdate(int id)
        {
            var dto = await _context.Tours
                .AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => new TourUpdateDto
                {
                    Id = t.Id,
                    ImageCover = t.ImageCover,
                    IsActive = t.IsActive,
                    FK_DestinationID = t.FK_DestinationID,
                    DestinationName = t.Destination.Translations
                        .Where(tr => tr.Language == "en")
                        .Select(tr => tr.Name)
                        .FirstOrDefault() ?? string.Empty,

                    FK_CategoryID = t.FK_CategoryID,
                    CategoryName = t.Category.ReferneceName,

                    ReferneceName = t.ReferneceName,
                    StartLocation = t.StartLocation,
                    EndLocation = t.EndLocation,
                    Price = t.Price,
                    Duration = t.Duration,
                    LanguageOptions = t.LanguageOptions,
                    LinkVideo = t.LinkVideo,

                    Translations = t.Translations
                        .Select(tr => new TourTranslationDto
                        {
                            Language = tr.Language,
                            Title = tr.Title,
                            Description = tr.Description
                        }).ToList(),

                    IncludedItems = t.Included
                        .Select(i => new TourIncludedDto
                        {
                            Id = i.Id,
                            Language = i.Language,
                            Text = i.Text
                        }).ToList(),

                    NotIncludedItems = t.NotIncluded
                        .Select(n => new TourNotIncludedDto
                        {
                            Id = n.Id,
                            Language = n.Language,
                            Text = n.Text
                        }).ToList(),

                    Highlights = t.Highlights
                        .Select(h => new TourHighlightDto
                        {
                            Id = h.Id,
                            Language = h.Language,
                            Text = h.Text
                        }).ToList(),

                    TourImgs = t.TourImgs
                        .Select(img => new TourImgUpdateDto
                        {
                            Id = img.Id,
                            ImageCarouselUrl = img.ImageCarouselUrl,
                            IsActive = img.IsActive,
                            Translations = img.Translations
                                .Select(tr => new TourImgTranslationDto
                                {
                                    Language = tr.Language,
                                    Title = tr.Title,
                                    TourName = tr.TourName
                                }).ToList()
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            if (dto == null)
                return NotFound();

            return Ok(dto);
        }



    }
}
