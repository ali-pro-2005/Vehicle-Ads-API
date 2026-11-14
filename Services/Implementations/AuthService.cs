using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using FinalProjectWebAPI.Data;
using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Models;
using FinalProjectWebAPI.Services.Interfaces;

namespace FinalProjectWebAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IWhatsAppService _whatsAppService;

        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration, IWhatsAppService whatsAppService)
        {
            _context = context;
            _configuration = configuration;
            _whatsAppService = whatsAppService;
        }
        public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
        {
            // 1. التحقق من عدم وجود المستخدم مسبقاً
            if (await _context.Users.AnyAsync(u => u.Phone == dto.Phone))
                throw new Exception("رقم الهاتف مسجل مسبقاً.");

            // 2. تشفير كلمة المرور
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 3. إنشاء المستخدم
            var user = new User
            {
                Name = dto.Name,
                Phone = dto.Phone,
                PasswordHash = passwordHash,
                Role = dto.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 4. توليد التوكن وإرجاعه
            return GenerateJwtToken(user);
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            // 1. البحث عن المستخدم
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);
            if (user == null)
                throw new Exception("رقم الهاتف أو كلمة المرور غير صحيحة.");

            // 2. التحقق من كلمة المرور
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("رقم الهاتف أو كلمة المرور غير صحيحة.");

            // 3. توليد التوكن
            return GenerateJwtToken(user);
        }

        // دالة مساعدة لتوليد الـ JWT Token
        private AuthResponseDto GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            // ما هي البيانات التي سنضعها داخل التوكن؟ (Claims)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Phone", user.Phone)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(double.Parse(jwtSettings["DurationInDays"])),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new AuthResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                Name = user.Name,
                Role = user.Role,
                UserId = user.Id
            };
        }
        // 1. دالة تغيير كلمة المرور
        public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("المستخدم غير موجود.");

            // التحقق من كلمة المرور الحالية
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("كلمة المرور الحالية غير صحيحة.");

            // تشفير وحفظ كلمة المرور الجديدة
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        // 2. دالة حذف الحساب وكل ما يتعلق به
        public async Task<bool> DeleteAccountAsync(int userId, string password)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) throw new Exception("المستخدم غير موجود.");

            // التحقق من كلمة المرور قبل الحذف
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!isPasswordValid)
                throw new Exception("كلمة المرور غير صحيحة. لا يمكن حذف الحساب.");

            // بدء Transaction لضمان حذف كل شيء أو التراجع
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // أ. حذف التقييمات (التي كتبها والتي استلمها)
                var ratings = await _context.Ratings
                    .Where(r => r.RaterId == userId || r.RatedUserId == userId)
                    .ToListAsync();
                _context.Ratings.RemoveRange(ratings);

                // ب. حذف الإعلانات الخاصة بالمستخدم
                var ads = await _context.Ads.Where(a => a.UserId == userId).ToListAsync();
                _context.Ads.RemoveRange(ads);

                // ج. إذا كان صاحب معرض: حذف الفروع، إعلانات الفروع، والمعرض
                var showroom = await _context.Showrooms.FirstOrDefaultAsync(s => s.UserId == userId);
                if (showroom != null)
                {
                    var branches = await _context.Branches.Where(b => b.ShowroomId == showroom.Id).ToListAsync();

                    // حذف الإعلانات المرتبطة بفروع المعرض
                    var branchIds = branches.Select(b => b.Id).ToList();
                    var branchAds = await _context.Ads.Where(a => a.BranchId != null && branchIds.Contains(a.BranchId.Value)).ToListAsync();
                    _context.Ads.RemoveRange(branchAds);

                    _context.Branches.RemoveRange(branches);
                    _context.Showrooms.Remove(showroom);
                }

                // د. حذف المستخدم نفسه
                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw new Exception("حدث خطأ أثناء حذف الحساب. تم التراجع عن العملية.");
            }
        }      

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Phone == dto.Phone);
            if (user == null) throw new Exception("رقم الهاتف غير مسجل لدينا.");

            // 1. توليد كلمة مرور عشوائية (مثلاً 8 أحرف)
            var newPassword = Guid.NewGuid().ToString("N").Substring(0, 8);

            // 2. تشفيرها وحفظها
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            // 3. إرسالها عبر الواتساب (الوهمي)
            string message = $"مرحباً {user.Name}، كلمة المرور الجديدة الخاصة بك هي: {newPassword}";
            await _whatsAppService.SendMessageAsync(user.Phone, message);

            return true;
        }
    }
}