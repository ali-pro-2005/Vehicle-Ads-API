using FinalProjectWebAPI.Data;
using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Models;
using FinalProjectWebAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace FinalProjectWebAPI.Services.Implementations
{
    public class AdService : IAdService
    {
        private readonly AppDbContext _context;

        public AdService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AdReadDto> CreateAdAsync(AdCreateDto dto, int userId, string userRole)
        {
            // 1. تطبيق منطق الحد الأقصى للإعلانات (Business Logic)
            int currentAdsCount = await _context.Ads.CountAsync(a => a.UserId == userId && !a.IsDeleted);

            if (userRole == "NormalUser" && currentAdsCount >= 5)
                throw new Exception("لقد وصلت للحد الأقصى للإعلانات في النسخة المجانية (5 إعلانات).");

            if (userRole == "ShowroomOwner" && currentAdsCount >= 20)
                throw new Exception("لقد وصلت للحد الأقصى للإعلانات للمعارض (20 إعلاناً).");

            // 2. التحقق من الفرع (إذا كان صاحب معرض)
            if (userRole == "ShowroomOwner")
            {
                if (dto.BranchId == null)
                    throw new Exception("يجب تحديد الفرع التابع للمعرض.");

                // التأكد أن هذا الفرع يتبع للمعرض الخاص بهذا المستخدم
                var ownsBranch = await _context.Branches
                    .AnyAsync(b => b.Id == dto.BranchId && b.Showroom.UserId == userId);

                if (!ownsBranch)
                    throw new Exception("الفرع غير موجود أو لا تملك صلاحية النشر فيه.");
            }
            else
            {
                // إذا كان شخصاً عادياً، لا يمكنه الربط بفرع
                dto.BranchId = null;
            }

            // 3. إنشاء الإعلان
            var ad = new Ad
            {
                Title = dto.Title,
                Brand = dto.Brand,
                Model = dto.Model,
                Year = dto.Year,
                Price = dto.Price,
                Description = dto.Description,
                PhoneNumber = dto.PhoneNumber,
                UserId = userId,
                BranchId = dto.BranchId,
                IsDeleted = false
            };

            _context.Ads.Add(ad);
            await _context.SaveChangesAsync();

            return await GetAdByIdAsync(ad.Id);
        }

        public async Task<IEnumerable<AdReadDto>> GetAllAdsAsync()
        {
            // بفضل الـ Global Query Filter في DbContext، الإعلانات المحذوفة لن تظهر هنا
            return await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Branch)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => MapToReadDto(a))
                .ToListAsync();
        }

        public async Task<IEnumerable<AdReadDto>> GetMyAdsAsync(int userId)
        {
            return await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Branch)
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => MapToReadDto(a))
                .ToListAsync();
        }

        public async Task<AdReadDto> GetAdByIdAsync(int id)
        {
            var ad = await _context.Ads
                .Include(a => a.User)
                .Include(a => a.Branch)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null) throw new Exception("الإعلان غير موجود.");

            return MapToReadDto(ad);
        }

        public async Task<bool> DeleteAdAsync(int id, int userId, string userRole)
        {
            var ad = await _context.Ads.FirstOrDefaultAsync(a => a.Id == id);
            if (ad == null) throw new Exception("الإعلان غير موجود.");

            // التحقق من الصلاحية: هل هو صاحب الإعلان أم مدير النظام؟
            if (ad.UserId != userId && userRole != "Admin")
                throw new Exception("غير مصرح لك بحذف هذا الإعلان.");

            // تطبيق الحذف الناعم (Soft Delete)
            ad.IsDeleted = true;
            _context.Ads.Update(ad);
            await _context.SaveChangesAsync();

            return true;
        }

        // دالة مساعدة لتحويل الكيان إلى DTO
        private static AdReadDto MapToReadDto(Ad ad)
        {
            return new AdReadDto
            {
                Id = ad.Id,
                Title = ad.Title,
                Brand = ad.Brand,
                Model = ad.Model,
                Year = ad.Year,
                Price = ad.Price,
                Description = ad.Description,
                PhoneNumber = ad.PhoneNumber,
                CreatedAt = ad.CreatedAt,
                SellerName = ad.User?.Name ?? "غير معروف",
                BranchCity = ad.Branch?.City
            };
        }
    }
}