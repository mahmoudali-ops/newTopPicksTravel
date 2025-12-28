using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TourSite.APIs.Errors;
using TourSite.Core.DTOs.HighLights;
using TourSite.Core.DTOs.Includes;
using TourSite.Core.DTOs.NotIncludes;
using TourSite.Core.DTOs.Transfer;
using TourSite.Core.Entities;
using TourSite.Core.Servicies.Contract;
using TourSite.Core.Specification.Transfers;
using TourSite.Core.Specification.Users;
using TourSite.Repository.Data.Contexts;

namespace TourSite.APIs.Controllers
{

    public class TransferController : BaseApiController
    {
        private readonly ITransferService transsService;

        private readonly TourDbContext _context;
        public TransferController(ITransferService _transsService, TourDbContext context)
        {
            transsService = _transsService;
            _context = context;
        }
        [HttpGet("client")]
        public async Task<IActionResult> GetAllTours([FromQuery] TrasferSpecParam SpecParams)
        {
            var transfers = await transsService.GetAllTransToursAsync(SpecParams);

            return Ok(transfers);
        }

        [HttpGet("admin")]
        public async Task<IActionResult> GetAllAdminTours([FromQuery] TrasferSpecParam SpecParams)
        {
            var transfers = await transsService.GetAllTransToursAdminAsync(SpecParams);

            return Ok(transfers);
        }
        [HttpGet("by-slug/{slug}")]
        public async Task<IActionResult> GetCategoryBySlug(string slug, [FromQuery] string? lang)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return BadRequest(new APIErrerResponse(400, "Slug is required"));

            var transfer = await transsService.GetCatTransByIdAsync(slug, lang);

            if (transfer == null)
                return NotFound(new APIErrerResponse(404, "Transfer not found"));

            return Ok(transfer);
        }
        // method to create new transfer
        [HttpPost("create")]
        public async Task<IActionResult> CreateHotel([FromForm] TransferCreateDto dto)
        {
            await transsService.AddTransferAsync(dto);
            return Ok(new { message = "Transfer created successfully" });

        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdataCategoryTour([FromForm] TransferCreateDto dto, int id)
        {
            if (id <= 0) return BadRequest(new APIErrerResponse(400, "Id required .. can not be less than or equal 0"));
            var result = await transsService.UpdateTransfer(dto, id);
            if (!result)
            {
                return NotFound(new APIErrerResponse(404, $"There is no Hotel with this Id : {id}"));
            }
            return Ok(new { message = "Transfer  updated successfully" });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategoryTour(int id)
        {
            if (id <= 0) return BadRequest(new APIErrerResponse(400, "Id required .. can not be less than or equal 0"));
            var result = await transsService.Deletetransfer(id);
            if (!result)
            {
                return NotFound(new APIErrerResponse(404, $"There is no Hotel with this Id : {id}"));
            }
            return Ok(new { message = "Transfer deleted successfully" });
        }


        [HttpGet("get-transfer-for-update/{id}")]
        public async Task<IActionResult> GetTransferForUpdate(int id)
        {
            var transfer = await _context.Transfers
                .Include(t => t.Destination)
                .Include(t => t.Translations)
                .Include(t => t.PricesList)
                .Include(t => t.Includeds)
                .Include(t => t.NotIncludeds)
                .Include(t => t.Highlights)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (transfer == null)
                return NotFound();


            var dto = new TransferUpdateDto
            {
                Id = transfer.Id,
                ImageCover = transfer.ImageCover,
                IsActive = transfer.IsActive,
                FK_DestinationID = transfer.FK_DestinationID,
                DestinationName =
        transfer.Destination?.Translations?
            .FirstOrDefault(tr => tr.Language == "en")?.Name
        ?? string.Empty,

                ReferneceName = transfer.ReferneceName,

                Translations = transfer.Translations?
        .Select(t => new TransferTranslationDto
        {
            Language = t.Language,
            Name = t.Name,
            Description = t.Description
        }).ToList() ?? new(),

                PricesList = transfer.PricesList?
        .Select(p => new TransferPricesDTO
        {
            Id = p.Id,
            Title = p.Title,
            PrivtePrice = p.PrivtePrice,
            SharedPrice = p.SharedPrice,
            ReferneceName = p.ReferneceName,
            Language = p.Language
        }).ToList() ?? new(),

                Includeds = transfer.Includeds?
        .Select(i => new TransferIncludedDto
        {
            Id = i.Id,
            Language = i.Language,
            Text = i.Text
        }).ToList() ?? new(),

                NotIncludeds = transfer.NotIncludeds?
        .Select(n => new TransferNotIncludedDto
        {
            Id = n.Id,
            Language = n.Language,
            Text = n.Text
        }).ToList() ?? new(),

                Highlights = transfer.Highlights?
        .Select(h => new TransferHighlightDto
        {
            Id = h.Id,
            Language = h.Language,
            Text = h.Text
        }).ToList() ?? new()
            };

            return Ok(dto);

        }
    }
}

