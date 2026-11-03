
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Opportunity
    {
        public int Id { get; set; }

        [Required]
        public string OpportunityName { get; set; } = string.Empty;

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string CompanyName { get; set; } = string.Empty;

        [Range(1, 100000000)]
        public decimal DealValue { get; set; }

        public string Stage { get; set; } = "Qualification";

        public string Status { get; set; } = "Open";

        public DateTime ExpectedCloseDate { get; set; }
            = DateTime.UtcNow.AddMonths(1);

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;
    }
}
