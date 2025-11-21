using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Interface dịch vụ quản lý danh mục công việc
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Lấy tất cả danh mục của người dùng
        /// </summary>
        Task<IEnumerable<Category>> GetCategoriesAsync(string userId);

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        Task<Category?> GetCategoryByIdAsync(int categoryId, string userId);

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        Task<Category> CreateCategoryAsync(string userId, string name, string? description, string? color);

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        Task<bool> UpdateCategoryAsync(int categoryId, string userId, string name, string? description, string? color);

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        Task<bool> DeleteCategoryAsync(int categoryId, string userId);

        /// <summary>
        /// Lấy số lượng công việc trong danh mục
        /// </summary>
        Task<int> GetCategoryItemCountAsync(int categoryId, string userId);

        /// <summary>
        /// Kiểm tra xem tên danh mục đã tồn tại chưa
        /// </summary>
        Task<bool> CategoryNameExistsAsync(string userId, string name, int? excludeCategoryId = null);

        /// <summary>
        /// Xóa tất cả danh mục khi xóa người dùng
        /// </summary>
        Task<bool> DeleteUserCategoriesAsync(string userId);
    }
}
