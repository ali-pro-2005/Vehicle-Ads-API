using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class RatingCreateDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون من 1 إلى 5")]
        public int Score { get; set; }

        [StringLength(500, ErrorMessage = "التعليق لا يمكن أن يتجاوز 500 حرف")]
        public string? Comment { get; set; }

        // معرف البائع (سواء كان شخصاً عادياً أو صاحب معرض)
        [Required]
        public int RatedUserId { get; set; }

        // معرف المعرض (اختياري، يُملأ إذا كان التقييم للمعرض ككل)
        public int? ShowroomId { get; set; }
    }
}