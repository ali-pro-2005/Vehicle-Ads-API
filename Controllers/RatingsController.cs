using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Services.Interfaces;

namespace FinalProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingService _ratingService;

        public RatingsController(IRatingService ratingService)
        {
            _ratingService = ratingService;
        }

        [Authorize] // يجب تسجيل الدخول لإضافة تقييم
        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] RatingCreateDto dto)
        {
            try
            {
                // استخراج هوية من يقوم بالتقييم من التوكن
                var raterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _ratingService.AddRatingAsync(dto, raterId);
                return Ok(new { message = "تم إضافة التقييم بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserRatings(int userId)
        {
            var ratings = await _ratingService.GetRatingsForUserAsync(userId);
            var average = await _ratingService.GetAverageRatingForUserAsync(userId);

            return Ok(new { Average = average, Ratings = ratings });
        }

        [HttpGet("showroom/{showroomId}")]
        public async Task<IActionResult> GetShowroomRatings(int showroomId)
        {
            var ratings = await _ratingService.GetRatingsForShowroomAsync(showroomId);
            var average = await _ratingService.GetAverageRatingForShowroomAsync(showroomId);

            return Ok(new { Average = average, Ratings = ratings });
        }
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRating(int id, [FromBody] RatingUpdateDto dto)
        {
            try
            {
                var raterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _ratingService.UpdateRatingAsync(id, dto, raterId);
                return Ok(new { message = "تم تعديل التقييم بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            try
            {
                var raterId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                await _ratingService.DeleteRatingAsync(id, raterId);
                return Ok(new { message = "تم حذف التقييم بنجاح." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}