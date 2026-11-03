
using System;
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        public string ActivityName { get; set; } = string.Empty;

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        [Required]
        public string Type { get; set; } = "Call";

        [Required]
        public DateTime ActivityDate { get; set; }

        public string Status { get; set; } = "Pending";

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
