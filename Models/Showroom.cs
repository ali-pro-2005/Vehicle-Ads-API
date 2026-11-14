using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FinalProjectWebAPI.Models
{
    public class Showroom
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم المعرض مطلوب")]
        [StringLength(150)]
        public string Name { get; set; }

        [Required(ErrorMessage = "رقم الترخيص مطلوب")]
        [StringLength(50)]
        public string LicenseNumber { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign Key

        // --- Navigation Properties ---

        // علاقة المعرض بصاحبه (M:1)
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; set; }

        // علاقة المعرض بفروعه (1:M)
        public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

        // علاقة المعرض بالتقييمات (1:M)
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
    }
}