using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// ExportService: Xử lý xuất dữ liệu và tính toán thống kê
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public class ExportService : IExportService
    {
        /// <summary>
        /// Xuất danh sách công việc sang CSV format
        /// </summary>
        public string ExportTooCsv(List<ToDoItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return "Không có dữ liệu để xuất";
            }

            var csv = new StringBuilder();

            // Thêm header
            csv.AppendLine("ID,Tiêu đề,Mô tả,Trạng thái,Ưu tiên,Ngày hạn chót,Ngày tạo,Ngày cập nhật");

            // Thêm dữ liệu
            foreach (var item in items)
            {
                var row = new List<string>
                {
                    item.Id.ToString(),
                    EscapeCsvValue(item.Title),
                    EscapeCsvValue(item.Description ?? ""),
                    GetStatusName(item.Status),
                    GetPriorityName(item.Priority),
                    item.DueDate?.ToString("yyyy-MM-dd") ?? "",
                    item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    item.UpdatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                };

                csv.AppendLine(string.Join(",", row));
            }

            return csv.ToString();
        }

        /// <summary>
        /// Tính toán thống kê từ danh sách công việc
        /// </summary>
        public ToDoStatistics GetStatistics(List<ToDoItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return new ToDoStatistics
                {
                    TotalItems = 0,
                    CompletedItems = 0,
                    PendingItems = 0,
                    OverdueItems = 0,
                    HighPriorityItems = 0,
                    CompletionRate = 0,
                    DueSoonItems = 0,
                    EarliestCreatedDate = null,
                    LatestCreatedDate = null
                };
            }

            var today = DateTime.Today;
            var sevenDaysLater = today.AddDays(7);

            var completedItems = items.Count(x => x.Status == ToDoStatus.Completed);
            var pendingItems = items.Count(x => x.Status != ToDoStatus.Completed);
            var overdueItems = items.Count(x =>
                x.DueDate.HasValue &&
                x.DueDate.Value.Date < today &&
                x.Status != ToDoStatus.Completed);
            var highPriorityItems = items.Count(x => x.Priority == Priority.High);
            var dueSoonItems = items.Count(x =>
                x.DueDate.HasValue &&
                x.DueDate.Value.Date >= today &&
                x.DueDate.Value.Date <= sevenDaysLater &&
                x.Status != ToDoStatus.Completed);

            var completionRate = items.Count > 0
                ? (decimal)completedItems / items.Count * 100
                : 0;

            return new ToDoStatistics
            {
                TotalItems = items.Count,
                CompletedItems = completedItems,
                PendingItems = pendingItems,
                OverdueItems = overdueItems,
                HighPriorityItems = highPriorityItems,
                CompletionRate = Math.Round(completionRate, 2),
                DueSoonItems = dueSoonItems,
                EarliestCreatedDate = items.Min(x => x.CreatedAt),
                LatestCreatedDate = items.Max(x => x.CreatedAt)
            };
        }

        /// <summary>
        /// Escape giá trị CSV (bao gồm dấu ngoặc kép nếu chứa dấu phẩy)
        /// </summary>
        private static string EscapeCsvValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return string.Empty;
            }

            // Nếu chứa dấu phẩy, dấu ngoặc kép, hoặc ký tự xuống dòng, bao bọc bằng dấu ngoặc kép
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                // Escape dấu ngoặc kép bằng cách nhân đôi
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }

            return value;
        }

        /// <summary>
        /// Lấy tên trạng thái tiếng Việt
        /// </summary>
        private static string GetStatusName(ToDoStatus status)
        {
            return status switch
            {
                ToDoStatus.Pending => "Chưa bắt đầu",
                ToDoStatus.InProgress => "Đang thực hiện",
                ToDoStatus.Completed => "Hoàn thành",
                _ => "Không xác định"
            };
        }

        /// <summary>
        /// Lấy tên ưu tiên tiếng Việt
        /// </summary>
        private static string GetPriorityName(Priority priority)
        {
            return priority switch
            {
                Priority.Low => "Thấp",
                Priority.Medium => "Trung bình",
                Priority.High => "Cao",
                _ => "Không xác định"
            };
        }
    }
}
