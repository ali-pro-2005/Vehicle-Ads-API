using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Services.Interfaces;

namespace FinalProjectWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowroomsController : ControllerBase
    {
        private readonly IShowroomService _showroomService;

        public ShowroomsController(IShowroomService showroomService)
        {
            _showroomService = showroomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var showrooms = await _showroomService.GetAllShowroomsAsync();
            return Ok(showrooms);
        }

        [Authorize]
        [HttpGet("my-showroom")]
        public async Task<IActionResult> GetMyShowroom()
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var result = await _showroomService.GetMyShowroomAsync(userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateShowroom([FromBody] ShowroomCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var result = await _showroomService.CreateShowroomAsync(dto, userId, userRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("branches")]
        public async Task<IActionResult> AddBranch([FromBody] BranchCreateDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var userRole = User.FindFirstValue(ClaimTypes.Role);

                var result = await _showroomService.AddBranchAsync(dto, userId, userRole);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}