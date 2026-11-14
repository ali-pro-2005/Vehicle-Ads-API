using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class LoginDto
    {
        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "رقم الهاتف يجب أن يكون بين 9 و 15 رقم")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        public string Password { get; set; }
    }
}