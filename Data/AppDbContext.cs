using FinalProjectWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectWebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // 1. تعريف الجداول (DbSets)
        public DbSet<User> Users { get; set; }
        public DbSet<Showroom> Showrooms { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Ad> Ads { get; set; }
        public DbSet<Rating> Ratings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 2. إعدادات المستخدم (User)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Phone)
                .IsUnique(); // رقم الهاتف فريد لا يتكرر

            // 3. إعدادات المعرض (Showroom)
            modelBuilder.Entity<Showroom>()
                .HasIndex(s => s.LicenseNumber)
                .IsUnique(); // رقم الترخيص فريد

            modelBuilder.Entity<Showroom>()
                .HasOne(s => s.User)
                .WithMany(u => u.Showrooms)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); // منع الحذف المتسلسل

            // 4. إعدادات الإعلان (Ad) - منع الحذف المتسلسل
            modelBuilder.Entity<Ad>()
                .HasOne(a => a.User)
                .WithMany(u => u.Ads)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull); // إذا حُذف المستخدم، الإعلان يبقى بدون مستخدم

            modelBuilder.Entity<Ad>()
                .HasOne(a => a.Branch)
                .WithMany(b => b.Ads)
                .HasForeignKey(a => a.BranchId)
                .OnDelete(DeleteBehavior.SetNull); // إذا حُذف الفرع، الإعلان يبقى

            // 5. إعدادات التقييم (Rating) - منع تضارب الحذف المتسلسل
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.RatedUser)
                .WithMany(u => u.ReceivedRatings)
                .HasForeignKey(r => r.RatedUserId)
                .OnDelete(DeleteBehavior.Restrict); // لا تحذف التقييمات إذا حُذف المستخدم الذي تم تقييمه

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Rater)
                .WithMany(u => u.GivenRatings)
                .HasForeignKey(r => r.RaterId)
                .OnDelete(DeleteBehavior.Restrict); // لا تحذف التقييمات إذا حُذف الكاتب

            // 6. تفعيل الحذف الناعم (Soft Delete) للإعلانات
            // هذا السطر يمنع ظهور الإعلانات المحذوفة (IsDeleted = true) في أي استعلام
            modelBuilder.Entity<Ad>()
                .HasQueryFilter(a => !a.IsDeleted);
        }
    }
}