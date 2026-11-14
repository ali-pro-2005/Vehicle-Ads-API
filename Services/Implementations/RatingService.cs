using Microsoft.EntityFrameworkCore;
using FinalProjectWebAPI.Data;
using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Models;
using FinalProjectWebAPI.Services.Interfaces;

namespace FinalProjectWebAPI.Services.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly AppDbContext _context;

        public RatingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddRatingAsync(RatingCreateDto dto, int raterId)
        {
            // 1. منع المستخدم من تقييم نفسه
            if (dto.RatedUserId == raterId)
                throw new Exception("لا يمكنك تقييم نفسك.");

            // 2. التحقق من وجود البائع
            var userExists = await _context.Users.AnyAsync(u => u.Id == dto.RatedUserId);
            if (!userExists)
                throw new Exception("المستخدم المراد تقييمه غير موجود.");

            // 3. منع التكرار (هل قام هذا المستخدم بتقييم هذا البائع مسبقاً؟)
            var alreadyRated = await _context.Ratings.AnyAsync(r =>
                r.RaterId == raterId &&
                r.RatedUserId == dto.RatedUserId &&
                r.ShowroomId == dto.ShowroomId);

            if (alreadyRated)
                throw new Exception("لقد قمت بتقييم هذا البائع مسبقاً.");

            // 4. إضافة التقييم
            var rating = new Rating
            {
                Score = dto.Score,
                Comment = dto.Comment,
                RatedUserId = dto.RatedUserId,
                ShowroomId = dto.ShowroomId,
                RaterId = raterId
            };

            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<RatingReadDto>> GetRatingsForUserAsync(int userId)
        {
            return await _context.Ratings
                .Include(r => r.Rater) // لجلب اسم من قام بالتقييم
                .Where(r => r.RatedUserId == userId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingReadDto
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    RaterName = r.Rater.Name,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<RatingReadDto>> GetRatingsForShowroomAsync(int showroomId)
        {
            return await _context.Ratings
                .Include(r => r.Rater)
                .Where(r => r.ShowroomId == showroomId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new RatingReadDto
                {
                    Id = r.Id,
                    Score = r.Score,
                    Comment = r.Comment,
                    RaterName = r.Rater.Name,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<double> GetAverageRatingForUserAsync(int userId)
        {
            // إذا لم يكن هناك تقييمات، نرجع 0
            var ratings = await _context.Ratings.Where(r => r.RatedUserId == userId).ToListAsync();
            if (!ratings.Any()) return 0;

            return Math.Round(ratings.Average(r => r.Score), 1); // تقريب لرقم واحد بعد الفاصلة
        }

        public async Task<double> GetAverageRatingForShowroomAsync(int showroomId)
        {
            var ratings = await _context.Ratings.Where(r => r.ShowroomId == showroomId).ToListAsync();
            if (!ratings.Any()) return 0;

            return Math.Round(ratings.Average(r => r.Score), 1);
        }
        public async Task<bool> UpdateRatingAsync(int ratingId, RatingUpdateDto dto, int raterId)
        {
            // 1. جلب التقييم
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.Id == ratingId);
            if (rating == null)
                throw new Exception("التقييم غير موجود.");

            // 2. التحقق من الصلاحية (هل هذا التقييم يخص المستخدم الحالي؟)
            if (rating.RaterId != raterId)
                throw new Exception("غير مصرح لك بتعديل تقييم لم تكتبه.");

            // 3. تحديث البيانات
            rating.Score = dto.Score;
            rating.Comment = dto.Comment;

            _context.Ratings.Update(rating);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteRatingAsync(int ratingId, int raterId)
        {
            // 1. جلب التقييم
            var rating = await _context.Ratings.FirstOrDefaultAsync(r => r.Id == ratingId);
            if (rating == null)
                throw new Exception("التقييم غير موجود.");

            // 2. التحقق من الصلاحية
            if (rating.RaterId != raterId)
                throw new Exception("غير مصرح لك بحذف تقييم لم تكتبه.");

            // 3. الحذف الفعلي (Hard Delete)
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}