using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace ToDoList.Models
{
    /// <summary>
    /// Lớp ApplicationUser đại diện cho thông tin người dùng
    /// Kế thừa từ IdentityUser để sử dụng hệ thống xác thực của ASP.NET Identity
    /// </summary>
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Tên đầy đủ của người dùng
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Danh sách các công việc cần làm của người dùng
        /// </summary>
        public virtual ICollection<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();

        /// <summary>
        /// Danh sách danh mục của người dùng
        /// </summary>
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();

        /// <summary>
        /// Ngày tạo tài khoản
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
