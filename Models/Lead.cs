
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Lead
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lead name is required")]
        [StringLength(100)]
        public string LeadName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[0-9]{10}$")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        public string Source { get; set; } = "Website";

        public string Status { get; set; } = "New";

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
