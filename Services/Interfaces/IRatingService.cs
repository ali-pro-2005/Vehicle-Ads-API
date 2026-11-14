using FinalProjectWebAPI.DTOs;

namespace FinalProjectWebAPI.Services.Interfaces
{
    public interface IRatingService
    {
        Task<bool> AddRatingAsync(RatingCreateDto dto, int raterId);
        Task<IEnumerable<RatingReadDto>> GetRatingsForUserAsync(int userId);
        Task<IEnumerable<RatingReadDto>> GetRatingsForShowroomAsync(int showroomId);
        Task<double> GetAverageRatingForUserAsync(int userId);
        Task<double> GetAverageRatingForShowroomAsync(int showroomId);
        Task<bool> UpdateRatingAsync(int ratingId, RatingUpdateDto dto, int raterId);
        Task<bool> DeleteRatingAsync(int ratingId, int raterId);
    }
}