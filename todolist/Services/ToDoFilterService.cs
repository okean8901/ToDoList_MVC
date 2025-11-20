using System;
using System.Collections.Generic;
using System.Linq;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// Lớp ToDoFilterService cung cấp các phương thức lọc, tìm kiếm và thống kê
    /// Áp dụng Single Responsibility Principle từ SOLID
    /// </summary>
    public class ToDoFilterService : IToDoFilterService
    {
        /// <summary>
        /// Lọc danh sách công việc theo các tiêu chí đa dạng
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc cần lọc</param>
        /// <param name="criteria">Tiêu chí lọc</param>
        /// <returns>Danh sách công việc đã lọc và sắp xếp</returns>
        public List<ToDoItem> FilterToDoItems(List<ToDoItem> toDoItems, ToDoFilterCriteria criteria)
        {
            if (toDoItems == null || toDoItems.Count == 0)
            {
                return toDoItems ?? new List<ToDoItem>();
            }

            var result = toDoItems.AsEnumerable();

            // ===== Lọc theo trạng thái =====
            if (criteria.Status.HasValue)
            {
                result = result.Where(t => t.Status == criteria.Status.Value);
            }

            // ===== Lọc theo mức độ ưu tiên =====
            if (criteria.Priority.HasValue)
            {
                result = result.Where(t => t.Priority == criteria.Priority.Value);
            }

            // ===== Lọc theo từ khóa tìm kiếm =====
            if (!string.IsNullOrWhiteSpace(criteria.SearchText))
            {
                var searchText = criteria.SearchText.ToLower();
                result = result.Where(t =>
                    t.Title.ToLower().Contains(searchText) ||
                    (t.Description != null && t.Description.ToLower().Contains(searchText))
                );
            }

            // ===== Lọc theo khoảng ngày hạn chót =====
            if (criteria.DueDateFrom.HasValue)
            {
                result = result.Where(t => t.DueDate >= criteria.DueDateFrom.Value);
            }

            if (criteria.DueDateTo.HasValue)
            {
                result = result.Where(t => t.DueDate <= criteria.DueDateTo.Value);
            }

            // ===== Lọc theo trạng thái hoàn thành =====
            if (criteria.IsCompleted.HasValue)
            {
                var isCompleted = criteria.IsCompleted.Value;
                result = result.Where(t => (t.Status == ToDoStatus.Completed) == isCompleted);
            }

            // ===== Lọc công việc sắp hết hạn =====
            if (criteria.DaysUntilDue.HasValue && criteria.DaysUntilDue.Value > 0)
            {
                result = result.Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value.Date >= DateTime.Today &&
                    t.DueDate.Value.Date <= DateTime.Today.AddDays(criteria.DaysUntilDue.Value)
                );
            }

            // ===== Sắp xếp kết quả =====
            result = SortToDoItems(result, criteria.SortBy, criteria.SortOrder);

            return result.ToList();
        }

        /// <summary>
        /// Lấy các công việc quá hạn (deadline đã qua và chưa hoàn thành)
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <returns>Danh sách công việc quá hạn</returns>
        public List<ToDoItem> GetOverdueItems(List<ToDoItem> toDoItems)
        {
            return toDoItems
                .Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value.Date < DateTime.Today &&
                    t.Status != ToDoStatus.Completed
                )
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        /// <summary>
        /// Lấy các công việc sắp hết hạn trong N ngày
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <param name="daysUntilDue">Số ngày tính từ hôm nay</param>
        /// <returns>Danh sách công việc sắp hết hạn</returns>
        public List<ToDoItem> GetDueItemsSoon(List<ToDoItem> toDoItems, int daysUntilDue)
        {
            var today = DateTime.Today;
            var dueDate = today.AddDays(daysUntilDue);

            return toDoItems
                .Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value.Date >= today &&
                    t.DueDate.Value.Date <= dueDate &&
                    t.Status != ToDoStatus.Completed
                )
                .OrderBy(t => t.DueDate)
                .ToList();
        }

        /// <summary>
        /// Đếm số lượng công việc theo từng trạng thái
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <returns>Dictionary với key là ToDoStatus, value là số lượng</returns>
        public Dictionary<ToDoStatus, int> CountByStatus(List<ToDoItem> toDoItems)
        {
            var result = new Dictionary<ToDoStatus, int>();

            // Khởi tạo tất cả trạng thái với giá trị 0
            foreach (ToDoStatus status in Enum.GetValues(typeof(ToDoStatus)))
            {
                result[status] = 0;
            }

            // Đếm từng trạng thái
            foreach (var item in toDoItems)
            {
                result[item.Status]++;
            }

            return result;
        }

        /// <summary>
        /// Đếm số lượng công việc theo từng mức độ ưu tiên
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <returns>Dictionary với key là Priority, value là số lượng</returns>
        public Dictionary<Priority, int> CountByPriority(List<ToDoItem> toDoItems)
        {
            var result = new Dictionary<Priority, int>();

            // Khởi tạo tất cả mức độ ưu tiên với giá trị 0
            foreach (Priority priority in Enum.GetValues(typeof(Priority)))
            {
                result[priority] = 0;
            }

            // Đếm từng mức độ ưu tiên
            foreach (var item in toDoItems)
            {
                result[item.Priority]++;
            }

            return result;
        }

        /// <summary>
        /// Hàm phụ trợ: Sắp xếp danh sách công việc
        /// </summary>
        /// <param name="toDoItems">Danh sách công việc</param>
        /// <param name="sortBy">Trường cần sắp xếp</param>
        /// <param name="sortOrder">Thứ tự sắp xếp (Ascending/Descending)</param>
        /// <returns>Danh sách công việc đã sắp xếp</returns>
        private IEnumerable<ToDoItem> SortToDoItems(IEnumerable<ToDoItem> toDoItems, string sortBy, string sortOrder)
        {
            var isDescending = sortOrder.Equals("Descending", StringComparison.OrdinalIgnoreCase);

            return sortBy.ToLower() switch
            {
                // Sắp xếp theo ngày hạn chót
                "duedate" => isDescending
                    ? toDoItems.OrderByDescending(t => t.DueDate ?? DateTime.MaxValue)
                    : toDoItems.OrderBy(t => t.DueDate ?? DateTime.MaxValue),

                // Sắp xếp theo mức độ ưu tiên
                "priority" => isDescending
                    ? toDoItems.OrderByDescending(t => t.Priority)
                    : toDoItems.OrderBy(t => t.Priority),

                // Sắp xếp theo tiêu đề
                "title" => isDescending
                    ? toDoItems.OrderByDescending(t => t.Title)
                    : toDoItems.OrderBy(t => t.Title),

                // Mặc định: sắp xếp theo ngày tạo
                _ => isDescending
                    ? toDoItems.OrderByDescending(t => t.CreatedAt)
                    : toDoItems.OrderBy(t => t.CreatedAt)
            };
        }
    }
}
