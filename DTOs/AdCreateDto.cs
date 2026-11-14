using System.ComponentModel.DataAnnotations;

namespace FinalProjectWebAPI.DTOs
{
    public class AdCreateDto
    {
        [Required][StringLength(100)] public string Title { get; set; }
        [Required][StringLength(50)] public string Brand { get; set; }
        [Required][StringLength(50)] public string Model { get; set; }
        [Required][Range(1900, 2100)] public int Year { get; set; }
        [Required][Range(0, double.MaxValue)] public decimal Price { get; set; }
        [StringLength(1000)] public string Description { get; set; }
        [Required][Phone] public string PhoneNumber { get; set; }

        // يُملأ فقط إذا كان المستخدم صاحب معرض
        public int? BranchId { get; set; }
    }
}