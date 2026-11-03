
using System;
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class FollowUp
    {
        public int Id { get; set; }

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string Subject { get; set; } = string.Empty;

        [Required]
        public DateTime FollowUpDate { get; set; }

        public string Type { get; set; } = "Call";

        public string Status { get; set; } = "Pending";

        public string Notes { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
