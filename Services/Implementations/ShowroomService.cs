using Microsoft.EntityFrameworkCore;
using FinalProjectWebAPI.Data;
using FinalProjectWebAPI.DTOs;
using FinalProjectWebAPI.Models;
using FinalProjectWebAPI.Services.Interfaces;

namespace FinalProjectWebAPI.Services.Implementations
{
    public class ShowroomService : IShowroomService
    {
        private readonly AppDbContext _context;

        public ShowroomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ShowroomReadDto> CreateShowroomAsync(ShowroomCreateDto dto, int userId, string userRole)
        {
            // 1. التحقق من الصلاحية (يجب أن يكون صاحب معرض)
            if (userRole != "ShowroomOwner")
                throw new Exception("فقط أصحاب المعارض يمكنهم إنشاء معرض.");

            // 2. التحقق من قاعدة "معرض واحد فقط لكل مستخدم"
            var hasShowroom = await _context.Showrooms.AnyAsync(s => s.UserId == userId);
            if (hasShowroom)
                throw new Exception("لديك معرض مسجل بالفعل. لا يمكنك إنشاء أكثر من معرض.");

            // 3. التحقق من عدم تكرار رقم الترخيص
            var licenseExists = await _context.Showrooms.AnyAsync(s => s.LicenseNumber == dto.LicenseNumber);
            if (licenseExists)
                throw new Exception("رقم الترخيص مسجل مسبقاً.");

            var showroom = new Showroom
            {
                Name = dto.Name,
                LicenseNumber = dto.LicenseNumber,
                UserId = userId
            };

            _context.Showrooms.Add(showroom);
            await _context.SaveChangesAsync();

            return new ShowroomReadDto
            {
                Id = showroom.Id,
                Name = showroom.Name,
                LicenseNumber = showroom.LicenseNumber
            };
        }

        public async Task<BranchReadDto> AddBranchAsync(BranchCreateDto dto, int userId, string userRole)
        {
            // 1. التحقق من الصلاحية
            if (userRole != "ShowroomOwner")
                throw new Exception("فقط أصحاب المعارض يمكنهم إضافة فروع.");

            // 2. التحقق من أن المعرض يخص هذا المستخدم
            var showroom = await _context.Showrooms.FirstOrDefaultAsync(s => s.Id == dto.ShowroomId && s.UserId == userId);
            if (showroom == null)
                throw new Exception("المعرض غير موجود أو لا تملك صلاحية إضافة فروع له.");

            var branch = new Branch
            {
                City = dto.City,
                Address = dto.Address,
                ShowroomId = dto.ShowroomId
            };

            _context.Branches.Add(branch);
            await _context.SaveChangesAsync();

            return new BranchReadDto
            {
                Id = branch.Id,
                City = branch.City,
                Address = branch.Address
            };
        }

        public async Task<ShowroomReadDto> GetMyShowroomAsync(int userId)
        {
            var showroom = await _context.Showrooms
                .Include(s => s.User)
                .Include(s => s.Branches)
                .FirstOrDefaultAsync(s => s.UserId == userId);

            if (showroom == null) throw new Exception("لا يوجد لديك معرض مسجل.");

            return new ShowroomReadDto
            {
                Id = showroom.Id,
                Name = showroom.Name,
                LicenseNumber = showroom.LicenseNumber,
                OwnerName = showroom.User.Name,
                Branches = showroom.Branches.Select(b => new BranchReadDto
                {
                    Id = b.Id,
                    City = b.City,
                    Address = b.Address
                }).ToList()
            };
        }

        public async Task<IEnumerable<ShowroomReadDto>> GetAllShowroomsAsync()
        {
            return await _context.Showrooms
                .Include(s => s.User)
                .Include(s => s.Branches)
                .Select(s => new ShowroomReadDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    LicenseNumber = s.LicenseNumber,
                    OwnerName = s.User.Name,
                    Branches = s.Branches.Select(b => new BranchReadDto
                    {
                        Id = b.Id,
                        City = b.City,
                        Address = b.Address
                    }).ToList()
                }).ToListAsync();
        }
    }
}