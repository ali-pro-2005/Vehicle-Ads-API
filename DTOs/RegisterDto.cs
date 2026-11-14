using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class RegisterDto
    {
        [Required(ErrorMessage = "الاسم مطلوب")]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [RegularExpression(@"^\+?\d{9,15}$", ErrorMessage = "رقم الهاتف يجب أن يكون بين 9 و 15 رقم")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(6, ErrorMessage = "كلمة المرور يجب أن لا تقل عن 6 أحرف")]
        public string Password { get; set; }

        [Required(ErrorMessage = "الدور مطلوب")]
        [RegularExpression("^(Admin|ShowroomOwner|NormalUser)$", ErrorMessage = "الدور غير صالح")]
        // القيم المتاحة: Admin, ShowroomOwner, NormalUser
        public string Role { get; set; }
    }
}