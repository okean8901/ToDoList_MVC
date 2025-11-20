using System;
using System.ComponentModel.DataAnnotations;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Models.ViewModels
{
    /// <summary>
    /// ViewModel để tạo mới hoặc cập nhật một ToDoItem
    /// </summary>
    public class ToDoItemViewModel
    {
        /// <summary>ID của công việc (để update)</summary>
        public int? Id { get; set; }

        /// <summary>Tiêu đề công việc</summary>
        [Required(ErrorMessage = "Tiêu đề không được để trống")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; } = string.Empty;

        /// <summary>Mô tả chi tiết</summary>
        [StringLength(2000, ErrorMessage = "Mô tả không được vượt quá 2000 ký tự")]
        public string? Description { get; set; }

        /// <summary>Trạng thái công việc</summary>
        [Required(ErrorMessage = "Trạng thái không được để trống")]
        public ToDoStatus Status { get; set; } = ToDoStatus.Pending;

        /// <summary>Mức độ ưu tiên</summary>
        [Required(ErrorMessage = "Mức độ ưu tiên không được để trống")]
        public Priority Priority { get; set; } = Priority.Medium;

        /// <summary>Ngày hạn chót</summary>
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }

    /// <summary>
    /// ViewModel hiển thị danh sách công việc với các tiêu chí lọc
    /// </summary>
    public class ToDoListViewModel
    {
        /// <summary>Danh sách công việc</summary>
        public List<ToDoItem> Items { get; set; } = new List<ToDoItem>();

        /// <summary>Tiêu chí lọc hiện tại</summary>
        public ToDoFilterCriteria? CurrentFilter { get; set; }

        /// <summary>Thống kê số công việc theo trạng thái</summary>
        public Dictionary<ToDoStatus, int>? StatusCounts { get; set; }

        /// <summary>Thống kê số công việc theo mức độ ưu tiên</summary>
        public Dictionary<Priority, int>? PriorityCounts { get; set; }

        /// <summary>Số công việc quá hạn</summary>
        public int OverdueCount { get; set; }

        /// <summary>Số công việc sắp hết hạn (7 ngày)</summary>
        public int DueSoonCount { get; set; }
    }

    /// <summary>
    /// ViewModel cho filter form
    /// </summary>
    public class FilterViewModel
    {
        /// <summary>Trạng thái lọc</summary>
        public ToDoStatus? Status { get; set; }

        /// <summary>Mức độ ưu tiên lọc</summary>
        public Priority? Priority { get; set; }

        /// <summary>Từ khóa tìm kiếm</summary>
        public string? SearchText { get; set; }

        /// <summary>Ngày bắt đầu khoảng lọc</summary>
        [DataType(DataType.Date)]
        public DateTime? DueDateFrom { get; set; }

        /// <summary>Ngày kết thúc khoảng lọc</summary>
        [DataType(DataType.Date)]
        public DateTime? DueDateTo { get; set; }

        /// <summary>Trường sắp xếp</summary>
        public string SortBy { get; set; } = "CreatedAt";

        /// <summary>Thứ tự sắp xếp</summary>
        public string SortOrder { get; set; } = "Descending";

        /// <summary>Chỉ hiển thị công việc đã hoàn thành</summary>
        public bool? IsCompleted { get; set; }

        /// <summary>Chỉ hiển thị công việc sắp hết hạn</summary>
        public int? DaysUntilDue { get; set; }
    }
}
