
using System;
using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } = "System";

        [Required]
        public string Action { get; set; } = string.Empty;

        [Required]
        public string EntityName { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
