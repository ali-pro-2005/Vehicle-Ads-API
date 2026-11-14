using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebAPI.Models
{
    public class Branch
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "المدينة مطلوبة")]
        [StringLength(50)]
        public string City { get; set; }

        [Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(200)]
        public string Address { get; set; }

        [Required]
        public int ShowroomId { get; set; } // Foreign Key

        // --- Navigation Properties ---

        // علاقة الفرع بالمعرض (M:1)
        [ForeignKey(nameof(ShowroomId))]
        public virtual Showroom Showroom { get; set; }

        // علاقة الفرع بالإعلانات (1:M)
        public virtual ICollection<Ad> Ads { get; set; } = new List<Ad>();
    }
}