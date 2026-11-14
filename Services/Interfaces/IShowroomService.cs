using FinalProjectWebAPI.DTOs;

namespace FinalProjectWebAPI.Services.Interfaces
{
    public interface IShowroomService
    {
        Task<ShowroomReadDto> CreateShowroomAsync(ShowroomCreateDto dto, int userId, string userRole);
        Task<ShowroomReadDto> GetMyShowroomAsync(int userId);
        Task<BranchReadDto> AddBranchAsync(BranchCreateDto dto, int userId, string userRole);
        Task<IEnumerable<ShowroomReadDto>> GetAllShowroomsAsync();
    }
}