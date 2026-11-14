using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebAPI.Models
{
    public class Rating
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "التقييم مطلوب")]
        [Range(1, 5, ErrorMessage = "يجب أن يكون التقييم بين 1 و 5")]
        public int Score { get; set; }

        [StringLength(500, ErrorMessage = "التعليق لا يمكن أن يتجاوز 500 حرف")]
        public string Comment { get; set; }

        // 1. الشخص الذي تم تقييمه (البائع)
        [Required]
        public int RatedUserId { get; set; }

        // 2. المعرض الذي تم تقييمه (اختياري)
        public int? ShowroomId { get; set; }

        // 3. الشخص الذي قام بكتابة التقييم (المشتري)
        [Required]
        public int RaterId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Navigation Properties ---

        // علاقة التقييم بالشخص الذي تم تقييمه (M:1)
        [ForeignKey(nameof(RatedUserId))]
        public virtual User RatedUser { get; set; }

        // علاقة التقييم بالشخص الذي كتب التقييم (M:1)
        [ForeignKey(nameof(RaterId))]
        public virtual User Rater { get; set; }

        // علاقة التقييم بالمعرض (M:1)
        [ForeignKey(nameof(ShowroomId))]
        public virtual Showroom Showroom { get; set; }
    }
}