using FinalProjectWebAPI.DTOs;

namespace FinalProjectWebAPI.Services.Interfaces
{
    public interface IAdService
    {
        Task<AdReadDto> CreateAdAsync(AdCreateDto dto, int userId, string userRole);
        Task<IEnumerable<AdReadDto>> GetAllAdsAsync();
        Task<IEnumerable<AdReadDto>> GetMyAdsAsync(int userId);
        Task<AdReadDto> GetAdByIdAsync(int id);
        Task<bool> DeleteAdAsync(int id, int userId, string userRole);
    }
}