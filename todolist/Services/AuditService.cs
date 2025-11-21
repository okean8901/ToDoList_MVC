using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Dịch vụ thực hiện ghi AuditLog vào database
    /// </summary>
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuditService> _logger;

        public AuditService(ApplicationDbContext context, ILogger<AuditService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LogAsync(int toDoItemId, string userId, string action, object? changes = null)
        {
            try
            {
                var log = new AuditLog
                {
                    ToDoItemId = toDoItemId,
                    UserId = userId,
                    Action = action,
                    Changes = changes == null ? null : JsonSerializer.Serialize(changes),
                    Timestamp = DateTime.UtcNow
                };

                _context.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi ghi audit log");
            }
        }

        public async Task<System.Collections.Generic.IEnumerable<AuditLog>> GetLogsForItemAsync(int toDoItemId, string userId)
        {
            return await _context.AuditLogs
                .Where(a => a.ToDoItemId == toDoItemId && a.UserId == userId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }
}
