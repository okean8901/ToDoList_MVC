using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Lớp ToDoService thực hiện các phương thức CRUD cho ToDoItem
    /// Sử dụng Entity Framework Core để tương tác với cơ sở dữ liệu
    /// </summary>
    public class ToDoService : IToDoService
    {
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor khởi tạo service với DbContext
        /// </summary>
        /// <param name="context">ApplicationDbContext được inject</param>
        public ToDoService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Tạo mới một công việc trong cơ sở dữ liệu
        /// </summary>
        /// <param name="toDoItem">Đối tượng ToDoItem cần tạo</param>
        /// <returns>Công việc đã được tạo</returns>
        public async Task<ToDoItem> CreateAsync(ToDoItem toDoItem)
        {
            // Thiết lập thời gian tạo
            toDoItem.CreatedAt = DateTime.UtcNow;
            toDoItem.UpdatedAt = DateTime.UtcNow;

            // Nếu chưa có Order, đặt ở cuối danh sách của người dùng
            if (!toDoItem.Order.HasValue)
            {
                var maxOrder = await _context.ToDoItems
                    .Where(t => t.UserId == toDoItem.UserId && t.Order.HasValue)
                    .MaxAsync(t => (int?)t.Order) ?? 0;
                toDoItem.Order = maxOrder + 1;
            }

            // Thêm vào DbSet
            _context.ToDoItems.Add(toDoItem);

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            return toDoItem;
        }

        /// <summary>
        /// Lấy tất cả công việc của một người dùng cụ thể
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Danh sách công việc được sắp xếp theo ngày tạo gần nhất trước</returns>
        public async Task<List<ToDoItem>> GetAllByUserIdAsync(string userId)
        {
            return await _context.ToDoItems
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Lấy một công việc theo ID (có xác thực quyền sở hữu)
        /// </summary>
        /// <param name="id">ID của công việc</param>
        /// <param name="userId">ID của người dùng (để xác thực quyền)</param>
        /// <returns>Công việc nếu tìm thấy và người dùng có quyền, ngược lại null</returns>
        public async Task<ToDoItem?> GetByIdAsync(int id, string userId)
        {
            return await _context.ToDoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        }

        /// <summary>
        /// Cập nhật thông tin của một công việc
        /// </summary>
        /// <param name="toDoItem">Đối tượng ToDoItem đã cập nhật</param>
        /// <returns>True nếu cập nhật thành công, False nếu không tìm thấy</returns>
        public async Task<bool> UpdateAsync(ToDoItem toDoItem)
        {
            // Kiểm tra xem công việc có tồn tại không
            var existingItem = await _context.ToDoItems.FindAsync(toDoItem.Id);
            if (existingItem == null)
            {
                return false;
            }

            // Cập nhật thời gian sửa đổi
            toDoItem.UpdatedAt = DateTime.UtcNow;
            toDoItem.CreatedAt = existingItem.CreatedAt; // Giữ nguyên thời gian tạo

            // Cập nhật các trường
            _context.Entry(existingItem).CurrentValues.SetValues(toDoItem);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Cập nhật thứ tự (Order) cho nhiều công việc
        /// </summary>
        public async Task UpdateOrderAsync(List<int> orderedIds, string userId)
        {
            var items = await _context.ToDoItems
                .Where(t => orderedIds.Contains(t.Id) && t.UserId == userId)
                .ToListAsync();

            for (int i = 0; i < orderedIds.Count; i++)
            {
                var id = orderedIds[i];
                var item = items.FirstOrDefault(t => t.Id == id);
                if (item != null)
                {
                    item.Order = i + 1;
                    _context.ToDoItems.Update(item);
                }
            }

            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Toggle IsStarred cho một công việc
        /// </summary>
        public async Task<bool> ToggleStarAsync(int id, string userId)
        {
            var item = await _context.ToDoItems.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
            if (item == null) return false;
            item.IsStarred = !item.IsStarred;
            item.UpdatedAt = DateTime.UtcNow;
            _context.ToDoItems.Update(item);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Xóa một công việc (có xác thực quyền sở hữu)
        /// </summary>
        /// <param name="id">ID của công việc cần xóa</param>
        /// <param name="userId">ID của người dùng (để xác thực quyền)</param>
        /// <returns>True nếu xóa thành công, False nếu không tìm thấy hoặc không có quyền</returns>
        public async Task<bool> DeleteAsync(int id, string userId)
        {
            // Tìm công việc với xác thực quyền sở hữu
            var toDoItem = await _context.ToDoItems
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            if (toDoItem == null)
            {
                return false;
            }

            // Xóa công việc
            _context.ToDoItems.Remove(toDoItem);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Xóa nhiều công việc cùng lúc (có xác thực quyền sở hữu)
        /// </summary>
        /// <param name="ids">Danh sách ID công việc cần xóa</param>
        /// <param name="userId">ID của người dùng (để xác thực quyền)</param>
        /// <returns>Số công việc đã xóa thành công</returns>
        public async Task<int> DeleteMultipleAsync(List<int> ids, string userId)
        {
            // Tìm tất cả công việc của người dùng có ID trong danh sách
            var toDoItems = await _context.ToDoItems
                .Where(t => ids.Contains(t.Id) && t.UserId == userId)
                .ToListAsync();

            if (toDoItems.Count == 0)
            {
                return 0;
            }

            // Xóa tất cả công việc tìm thấy
            _context.ToDoItems.RemoveRange(toDoItems);

            // Lưu thay đổi
            await _context.SaveChangesAsync();

            return toDoItems.Count;
        }
    }
}
