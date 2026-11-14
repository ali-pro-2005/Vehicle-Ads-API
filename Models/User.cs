using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "الاسم يجب أن يكون بين 3 و 100 حرف")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "رقم الهاتف غير صحيح")]
        [StringLength(20)]
        public string Phone { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(20)]
        public string Role { get; set; } // Admin, ShowroomOwner, NormalUser

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---

        // المستخدم (صاحب معرض) يملك معرضاً أو أكثر (1:M)
        public virtual ICollection<Showroom> Showrooms { get; set; } = new List<Showroom>();

        // المستخدم (شخص عادي) ينشر إعلانات (1:M)
        public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();

        // التقييمات التي كتبها هذا المستخدم (1:M)
        public virtual ICollection<Rating> GivenRatings { get; set; } = new List<Rating>();

        // التقييمات التي استلمها هذا المستخدم (1:M)
        public virtual ICollection<Rating> ReceivedRatings { get; set; } = new List<Rating>();
    }
}