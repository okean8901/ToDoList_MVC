using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Dịch vụ quản lý danh mục công việc
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        /// <summary>
        /// Constructor cho CategoryService
        /// </summary>
        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy tất cả danh mục của người dùng
        /// </summary>
        public async Task<IEnumerable<Category>> GetCategoriesAsync(string userId)
        {
            try
            {
                return await _context.Categories
                    .Where(c => c.UserId == userId)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh mục cho người dùng {userId}: {ex.Message}");
                return Enumerable.Empty<Category>();
            }
        }

        /// <summary>
        /// Lấy danh mục theo ID
        /// </summary>
        public async Task<Category?> GetCategoryByIdAsync(int categoryId, string userId)
        {
            try
            {
                return await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy danh mục {categoryId}: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        public async Task<Category> CreateCategoryAsync(string userId, string name, string? description, string? color)
        {
            try
            {
                // Kiểm tra tên danh mục đã tồn tại chưa
                if (await CategoryNameExistsAsync(userId, name))
                {
                    _logger.LogWarning($"Danh mục '{name}' đã tồn tại cho người dùng {userId}");
                return null;
                }

                var category = new Category
                {
                    Name = name,
                    Description = description,
                    Color = color ?? "#007bff",
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Tạo danh mục '{name}' thành công cho người dùng {userId}");
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi tạo danh mục: {ex.Message}");
            return null;
            }
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        public async Task<bool> UpdateCategoryAsync(int categoryId, string userId, string name, string? description, string? color)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

                if (category == null)
                {
                    _logger.LogWarning($"Không tìm thấy danh mục {categoryId} cho người dùng {userId}");
                    return false;
                }

                // Kiểm tra tên danh mục đã tồn tại chưa (ngoại trừ chính nó)
                if (name != category.Name && await CategoryNameExistsAsync(userId, name, categoryId))
                {
                    _logger.LogWarning($"Danh mục '{name}' đã tồn tại cho người dùng {userId}");
                    return false;
                }

                category.Name = name;
                category.Description = description;
                category.Color = color ?? "#007bff";

                _context.Categories.Update(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Cập nhật danh mục '{name}' thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi cập nhật danh mục: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        public async Task<bool> DeleteCategoryAsync(int categoryId, string userId)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.Id == categoryId && c.UserId == userId);

                if (category == null)
                {
                    _logger.LogWarning($"Không tìm thấy danh mục {categoryId} cho người dùng {userId}");
                    return false;
                }

                // Xóa liên kết danh mục từ các công việc
                var todosWithCategory = await _context.ToDoItems
                    .Where(t => t.CategoryId == categoryId && t.UserId == userId)
                    .ToListAsync();

                foreach (var todo in todosWithCategory)
                {
                    todo.CategoryId = null;
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Xóa danh mục {categoryId} thành công");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa danh mục: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Lấy số lượng công việc trong danh mục
        /// </summary>
        public async Task<int> GetCategoryItemCountAsync(int categoryId, string userId)
        {
            try
            {
                return await _context.ToDoItems
                    .CountAsync(t => t.CategoryId == categoryId && t.UserId == userId);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi lấy số lượng công việc: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Kiểm tra xem tên danh mục đã tồn tại chưa
        /// </summary>
        public async Task<bool> CategoryNameExistsAsync(string userId, string name, int? excludeCategoryId = null)
        {
            try
            {
                var query = _context.Categories
                    .Where(c => c.UserId == userId && c.Name.ToLower() == name.ToLower());

                if (excludeCategoryId.HasValue)
                {
                    query = query.Where(c => c.Id != excludeCategoryId.Value);
                }

                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi kiểm tra tên danh mục: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Xóa tất cả danh mục khi xóa người dùng
        /// </summary>
        public async Task<bool> DeleteUserCategoriesAsync(string userId)
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.UserId == userId)
                    .ToListAsync();

                foreach (var category in categories)
                {
                    await DeleteCategoryAsync(category.Id, userId);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Lỗi khi xóa danh mục người dùng: {ex.Message}");
                return false;
            }
        }
    }
}
