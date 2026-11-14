using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class ShowroomCreateDto
    {
        [Required][StringLength(150)] public string Name { get; set; }
        [Required][StringLength(50)] public string LicenseNumber { get; set; }
    }
}