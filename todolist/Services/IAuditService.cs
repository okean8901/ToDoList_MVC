using System.Threading.Tasks;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Interface dịch vụ ghi lịch sử thay đổi
    /// </summary>
    public interface IAuditService
    {
        Task LogAsync(int toDoItemId, string userId, string action, object? changes = null);
        Task<System.Collections.Generic.IEnumerable<AuditLog>> GetLogsForItemAsync(int toDoItemId, string userId);
    }
}
