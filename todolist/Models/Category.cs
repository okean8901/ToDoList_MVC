using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models
{
    /// <summary>
    /// Danh mục/Thẻ cho các mục công việc
    /// </summary>
    public class Category
    {
        /// <summary>
        /// ID danh mục
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Tên danh mục
        /// </summary>
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Tên danh mục phải có 2-100 ký tự")]
        public required string Name { get; set; }

        /// <summary>
        /// Mô tả danh mục
        /// </summary>
        [StringLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Màu sắc (hex code) cho danh mục
        /// </summary>
        [RegularExpression(@"^#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})$", ErrorMessage = "Mã màu phải là định dạng hex hợp lệ")]
        public string Color { get; set; } = "#007bff"; // Màu xanh dương mặc định

        /// <summary>
        /// ID người dùng sở hữu danh mục
        /// </summary>
        [Required]
        public required string UserId { get; set; }

        /// <summary>
        /// Người dùng sở hữu danh mục
        /// </summary>
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// Ngày tạo danh mục
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Danh sách các mục công việc trong danh mục
        /// </summary>
        public virtual ICollection<ToDoItem> ToDoItems { get; set; } = new List<ToDoItem>();
    }
}
