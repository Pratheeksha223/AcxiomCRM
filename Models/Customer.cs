
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer name is required")]
        [StringLength(100)]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Enter a valid email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$",
            ErrorMessage = "Enter a valid 10-digit phone number")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        public string Address { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string State { get; set; } = string.Empty;

        public string Status { get; set; } = "Active";

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
