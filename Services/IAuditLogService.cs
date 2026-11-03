
using System.Threading.Tasks;

namespace AcxiomCRM.Services
{
    public interface IAuditLogService
    {
        Task LogAsync(string action, string entityName, string details);
    }
}
