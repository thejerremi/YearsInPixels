using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YearsInPixels.Application.DTOs;
using YearsInPixels.Application.Interfaces;

namespace YearsInPixels.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PixelsController(IPixelService pixelService) : ControllerBase
    {
        [HttpGet("year/{year}")]
        public async Task<ActionResult<IEnumerable<DailyPixelDto>>> GetYear(int year)
            => Ok(await pixelService.GetYearlyPixelsAsync(year));

        [HttpPost("addOrUpdate")]
        public async Task<IActionResult> AddOrUpdate(DailyPixelDto dto)
        {
            await pixelService.AddOrUpdatePixelAsync(dto);
            return Ok(new { message = "Pixel has been saved!" });
        }

        [HttpGet("random-flashback/{id?}")]
        public async Task<ActionResult<RandomPixelDto>> GetRandom(int? id = null)
        {
            var result = await pixelService.GetRandomFlashbackAsync(id);
            return result != null ? Ok(result) : NotFound("No memories found yet.");
        }
    }
}
