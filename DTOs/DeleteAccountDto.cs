using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class DeleteAccountDto
    {
        [Required] public string Password { get; set; }
    }
}