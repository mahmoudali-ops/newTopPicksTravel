using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourSite.APIs.Attributes;
using TourSite.APIs.Errors;
using TourSite.Core.DTOs.CategorToutCreateDto;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.Transfer;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.CatgeoryTour;
using TourSite.Core.Specification.Tours;
using TourSite.Repository.Data.Contexts;
using TourSite.Service.Services.CatTours;

namespace TourSite.APIs.Controllers
{
  
    public class CategorTourController : BaseApiController
    {
        private readonly ICategoryTourService cattoursService;
        private readonly TourDbContext _context;
        public CategorTourController(ICategoryTourService _cattoursService, TourDbContext context)
        {
            cattoursService = _cattoursService;
            _context = context;
        }
        //[Cached(4)]
        [HttpGet("client")]
        public async Task<IActionResult> GetAllTours([FromQuery] CatTourSpecParams SpecParams)
        {
            var categories = await cattoursService.GetAllCatToursAsync(SpecParams);
            if(categories is null) return NotFound(new APIErrerResponse(404, "There is no categoriesTours .."));
            return Ok(categories);
        }
        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdminTours([FromQuery] CatTourSpecParams SpecParams)
        {
            var categories = await cattoursService.GetAllCatToursAdminAsync(SpecParams);
            if (categories is null) return NotFound(new APIErrerResponse(404, "There is no categoriesTours .."));
            return Ok(categories);
        }

   
        [HttpGet("by-slug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug, [FromQuery] string? lang)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest(new APIErrerResponse(400, "Slug is required"));

            var categorTout = await cattoursService.GetCatTourByIdAsync(slug, lang);

            if (categorTout == null)
                return NotFound(new APIErrerResponse(404, "Category tour not found"));

            return Ok(categorTout);
        }
        [HttpPost("create")]
        public async Task<IActionResult> CreateCategoryTour([FromForm] CategorToutCreateDto dto)
        {
            await cattoursService.AddCatTourAsync(dto);
            return Ok(new { message = "Category tour created successfully"});
        }

        [HttpPut("update/{id}")]
        public  async Task<IActionResult> UpdataCategoryTour([FromForm] CategorToutCreateDto dto,int id)
        {
            if (id <= 0) return BadRequest(new APIErrerResponse(400, "Id required .. can not be less than or equal 0"));
            var result=await cattoursService.UpdateCatTour(dto,id);
            if (!result)
            {
                return NotFound(new APIErrerResponse(404, $"There is no categoryTour with this Id : {id}"));
            }
            return Ok(new { message = "Category tour updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategoryTour(int id)
        {
            if (id <= 0) return BadRequest(new APIErrerResponse(400, "Id required .. can not be less than or equal 0"));
           var result=await cattoursService.DeleteCatTour(id);
            if (!result)
            {
                return NotFound(new APIErrerResponse(404, $"There is no categoryTour with this Id : {id}"));
            }
            return Ok(new { message = "Category tour deleted successfully" });
        }



        [HttpGet("get-CatgeroyTour-for-update/{id}")]
        public async Task<IActionResult> GetTransferForUpdate(int id)
        {
            var categorytour = await _context.CategoryTours
                .Include(t => t.Translations)  
                .FirstOrDefaultAsync(t => t.Id == id);

            if (categorytour == null)
                return NotFound();


            var dto = new CategoryTourUpdateDto
            {
                Id = categorytour.Id,
                ImageCover = categorytour.ImageCover,
                IsActive = categorytour.IsActive,
                ReferneceName = categorytour.ReferneceName,

                Translations = categorytour.Translations?
        .Select(t => new CategoryTourTranslationDto
        {
            Language = t.Language,
            Title = t.Title,
            Description = t.Description,
            MetaDescription = t.MetaDescription,
            MetaKeyWords = t.MetaKeyWords

        }).ToList() ?? new()
            };

            return Ok(dto);

        }
    }
}
