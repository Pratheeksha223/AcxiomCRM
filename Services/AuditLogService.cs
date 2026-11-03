
using AcxiomCRM.Data;
using AcxiomCRM.Models;

namespace AcxiomCRM.Services
{
    public class AuditLogService : IAuditLogService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(
            string action,
            string entityName,
            string details)
        {
            var userName =
                _httpContextAccessor.HttpContext?.User?.Identity?.Name
                ?? "System";

            var log = new AuditLog
            {
                UserName = userName,
                Action = action,
                EntityName = entityName,
                Details = details,
                Timestamp = DateTime.UtcNow
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}
