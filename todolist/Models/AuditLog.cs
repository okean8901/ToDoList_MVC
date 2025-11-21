using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    /// <summary>
    /// Lưu lịch sử thay đổi cho ToDoItem
    /// </summary>
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Id công việc liên quan
        /// </summary>
        public int ToDoItemId { get; set; }

        [ForeignKey("ToDoItemId")]
        public virtual ToDoItem? ToDoItem { get; set; }

        /// <summary>
        /// Người dùng thực hiện hành động
        /// </summary>
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Hành động thực hiện: Create, Update, Delete
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Nội dung thay đổi (định dạng JSON)
        /// </summary>
        public string? Changes { get; set; }

        /// <summary>
        /// Thời điểm thực hiện hành động
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
