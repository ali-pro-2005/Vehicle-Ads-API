using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebAPI.Models
{
    public class Ad
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان الإعلان مطلوب")]
        [StringLength(100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "الماركة مطلوبة")]
        [StringLength(50)]
        public string Brand { get; set; }

        [Required(ErrorMessage = "الموديل مطلوب")]
        [StringLength(50)]
        public string Model { get; set; }

        [Range(1900, 2100, ErrorMessage = "سنة الصنع غير صحيحة")]
        public int Year { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "السعر يجب أن يكون قيمة موجبة")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "رقم الهاتف غير صحيح")]
        public string PhoneNumber { get; set; }

        // يمتلئ إذا كان الناشر شخصاً عادياً
        public int? UserId { get; set; }

        // يمتلئ إذا كان الناشر معرضاً/فرعاً
        public int? BranchId { get; set; }

        public bool IsDeleted { get; set; } = false; // Soft Delete

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---

        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        [ForeignKey(nameof(BranchId))]
        public virtual Branch Branch { get; set; }
    }
}