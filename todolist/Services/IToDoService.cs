using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Interface IToDoService định nghĩa các phương thức CRUD cho ToDoItem
    /// Áp dụng Dependency Injection và SOLID principles
    /// </summary>
    public interface IToDoService
    {
        // ===== CREATE =====
        /// <summary>
        /// Tạo mới một công việc
        /// </summary>
        /// <param name="toDoItem">Đối tượng ToDoItem cần tạo</param>
        /// <returns>Công việc đã được tạo</returns>
        Task<ToDoItem> CreateAsync(ToDoItem toDoItem);

        // ===== READ =====
        /// <summary>
        /// Lấy tất cả công việc của một người dùng
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Danh sách công việc</returns>
        Task<List<ToDoItem>> GetAllByUserIdAsync(string userId);

        /// <summary>
        /// Lấy công việc theo ID
        /// </summary>
        /// <param name="id">ID của công việc</param>
        /// <param name="userId">ID của người dùng (để xác thực quyền)</param>
        /// <returns>Công việc hoặc null nếu không tìm thấy</returns>
        Task<ToDoItem?> GetByIdAsync(int id, string userId);

        // ===== UPDATE =====
        /// <summary>
        /// Cập nhật một công việc
        /// </summary>
        /// <param name="toDoItem">Đối tượng ToDoItem đã cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu không</returns>
        Task<bool> UpdateAsync(ToDoItem toDoItem);

        // ===== DELETE =====
        /// <summary>
        /// Xóa một công việc
        /// </summary>
        /// <param name="id">ID của công việc cần xóa</param>
        /// <param name="userId">ID của người dùng (để xác thực quyền)</param>
        /// <returns>True nếu xóa thành công, False nếu không</returns>
        Task<bool> DeleteAsync(int id, string userId);

        /// <summary>
        /// Xóa nhiều công việc theo danh sách ID
        /// </summary>
        /// <param name="ids">Danh sách ID công việc cần xóa</param>
        /// <param name="userId">ID của người dùng (để xác thực quyền)</param>
        /// <returns>Số công việc đã xóa</returns>
        Task<int> DeleteMultipleAsync(List<int> ids, string userId);

        /// <summary>
        /// Cập nhật thứ tự cho danh sách công việc
        /// </summary>
        Task UpdateOrderAsync(List<int> orderedIds, string userId);

        /// <summary>
        /// Bật/tắt đánh dấu quan trọng
        /// </summary>
        Task<bool> ToggleStarAsync(int id, string userId);
    }
}
