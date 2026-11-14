using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class RatingUpdateDto
    {
        [Required]
        [Range(1, 5, ErrorMessage = "التقييم يجب أن يكون من 1 إلى 5")]
        public int Score { get; set; }

        [StringLength(500, ErrorMessage = "التعليق لا يمكن أن يتجاوز 500 حرف")]
        public string? Comment { get; set; }
    }
}