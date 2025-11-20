using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// DTO (Data Transfer Object) chứa các tiêu chí lọc cho ToDoItems
    /// </summary>
    public class ToDoFilterCriteria
    {
        /// <summary>Lọc theo trạng thái</summary>
        public ToDoStatus? Status { get; set; }

        /// <summary>Lọc theo mức độ ưu tiên</summary>
        public Priority? Priority { get; set; }

        /// <summary>Lọc theo từ khóa tìm kiếm (tìm trong tiêu đề và mô tả)</summary>
        public string? SearchText { get; set; }

        /// <summary>Lọc công việc có hạn chót từ ngày này trở lại</summary>
        public DateTime? DueDateFrom { get; set; }

        /// <summary>Lọc công việc có hạn chót đến ngày này</summary>
        public DateTime? DueDateTo { get; set; }

        /// <summary>Sắp xếp theo: CreatedAt (mặc định), DueDate, Priority, Title</summary>
        public string SortBy { get; set; } = "CreatedAt";

        /// <summary>Thứ tự sắp xếp: Descending (mặc định) hoặc Ascending</summary>
        public string SortOrder { get; set; } = "Descending";

        /// <summary>Chỉ lấy các công việc đã hoàn thành</summary>
        public bool? IsCompleted { get; set; }

        /// <summary>Chỉ lấy các công việc sắp hết hạn (quá hạn hoặc trong vòng N ngày)</summary>
        public int? DaysUntilDue { get; set; }
    }

    /// <summary>
    /// Interface IToDoFilterService định nghĩa các phương thức lọc và tìm kiếm
    /// </summary>
    public interface IToDoFilterService
    {
        /// <summary>
        /// Lọc danh sách công việc theo các tiêu chí
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc cần lọc</param>
        /// <param name="criteria">Tiêu chí lọc</param>
        /// <returns>Danh sách công việc đã lọc</returns>
        List<ToDoItem> FilterToDoItems(List<ToDoItem> toDoItems, ToDoFilterCriteria criteria);

        /// <summary>
        /// Lấy các công việc quá hạn
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <returns>Danh sách công việc quá hạn</returns>
        List<ToDoItem> GetOverdueItems(List<ToDoItem> toDoItems);

        /// <summary>
        /// Lấy các công việc sắp hết hạn
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <param name="daysUntilDue">Số ngày tính từ hôm nay</param>
        /// <returns>Danh sách công việc sắp hết hạn</returns>
        List<ToDoItem> GetDueItemsSoon(List<ToDoItem> toDoItems, int daysUntilDue);

        /// <summary>
        /// Đếm các công việc theo trạng thái
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <returns>Dictionary chứa số lượng theo trạng thái</returns>
        Dictionary<ToDoStatus, int> CountByStatus(List<ToDoItem> toDoItems);

        /// <summary>
        /// Đếm các công việc theo mức độ ưu tiên
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <returns>Dictionary chứa số lượng theo mức độ ưu tiên</returns>
        Dictionary<Priority, int> CountByPriority(List<ToDoItem> toDoItems);
    }
}
