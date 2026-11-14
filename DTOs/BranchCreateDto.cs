using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class BranchCreateDto
    {
        [Required][StringLength(50)] public string City { get; set; }
        [Required][StringLength(200)] public string Address { get; set; }
        [Required] public int ShowroomId { get; set; }
    }
}