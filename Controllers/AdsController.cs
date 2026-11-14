using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FinalProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdsController : ControllerBase
    {
        private readonly IAdService _adService;

        public AdsController(IAdService adService)
        {
            _adService = adService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var ads = await _adService.GetAllAdsAsync();
            return Ok(ads);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var ad = await _adService.GetAdByIdAsync(id);
                return Ok(ad);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize] // يجب أن يكون المستخدم مسجلاً دخوله
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdCreateDto dto)
        {
            try
            {
                // استخراج معرف المستخدم ودوره من التوكن
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var result = await _adService.CreateAdAsync(dto, userId, userRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("my-ads")]
        public async Task<IActionResult> GetMyAds()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var ads = await _adService.GetMyAdsAsync(userId);
            return Ok(ads);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                await _adService.DeleteAdAsync(id, userId, userRole);
                return Ok(new { message = "تم حذف الإعلان بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}