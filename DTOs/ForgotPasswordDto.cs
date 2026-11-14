using System.ComponentModel.DataAnnotations;
namespace FinalProjectWebAPI.DTOs
{
    public class ForgotPasswordDto
    {
        [Required] public string Phone { get; set; }
    }
}