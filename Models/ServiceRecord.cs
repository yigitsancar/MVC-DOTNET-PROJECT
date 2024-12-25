using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityApp.Models
{
    public class ServiceRecord
    {
        [Key]
        public int ServiceRecordId { get; set; } 

        [Required]
        public string? CustomerId { get; set; } 
        public string? CustomerName { get; set; }

        [ForeignKey("CustomerId")]
        public AppUser? Customer { get; set; } 

        [Required(ErrorMessage ="Personnel field is required.")]
        public string? PersonnelId { get; set; } 
        public string? PersonnelName { get; set; }

        [ForeignKey("PersonnelId")]
        public AppUser? Personnel { get; set; } 

        [Required]
        [MaxLength(100)]
        [Display(Name = "Vehicle Model")]
        public string? VehicleModel { get; set; } 

        [Required]
        public string? Complaint { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
    }
}
