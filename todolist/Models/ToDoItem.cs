using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Models
{
    /// <summary>
    /// Lớp ToDoItem đại diện cho một công việc cần làm
    /// </summary>
    public class ToDoItem
    {
        /// <summary>
        /// Id duy nhất của công việc (khóa chính)
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Tiêu đề của công việc
        /// </summary>
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Mô tả chi tiết công việc
        /// </summary>
        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; }

        /// <summary>
        /// Trạng thái của công việc (Pending: Chưa làm, InProgress: Đang làm, Completed: Hoàn thành)
        /// </summary>
        [Required]
        public ToDoStatus Status { get; set; } = ToDoStatus.Pending;

        /// <summary>
        /// Mức độ ưu tiên (Low: Thấp, Medium: Trung bình, High: Cao)
        /// </summary>
        [Required]
        public Priority Priority { get; set; } = Priority.Medium;

        /// <summary>
        /// Ngày hạn chót hoàn thành công việc
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Ngày tạo công việc
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Ngày cập nhật cuối cùng
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Trường dùng để sắp xếp thủ công (giá trị càng nhỏ thì hiển thị càng lên trên)
        /// </summary>
        public int? Order { get; set; }

        /// <summary>
        /// Đánh dấu công việc quan trọng
        /// </summary>
        public bool IsStarred { get; set; } = false;

        /// <summary>
        /// ID của người dùng sở hữu công việc này (khóa ngoài)
        /// </summary>
        [Required]
        public string UserId { get; set; } = string.Empty;

        /// <summary>
        /// Tham chiếu đến người dùng sở hữu công việc
        /// </summary>
        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; }

        /// <summary>
        /// ID danh mục của công việc (khóa ngoài)
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// Tham chiếu đến danh mục của công việc
        /// </summary>
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }

    /// <summary>
    /// Enum đại diện cho các trạng thái của công việc
    /// </summary>
    public enum ToDoStatus
    {
        /// <summary>Chưa bắt đầu</summary>
        Pending = 0,

        /// <summary>Đang thực hiện</summary>
        InProgress = 1,

        /// <summary>Hoàn thành</summary>
        Completed = 2
    }

    /// <summary>
    /// Enum đại diện cho mức độ ưu tiên
    /// </summary>
    public enum Priority
    {
        /// <summary>Mức độ thấp</summary>
        Low = 0,

        /// <summary>Mức độ trung bình</summary>
        Medium = 1,

        /// <summary>Mức độ cao</summary>
        High = 2
    }
}
