using System;
using System.Collections.Generic;
using System.Linq;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// NotificationService: Xử lý hệ thống thông báo và reminder
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Lấy thông báo về công việc quá hạn
        /// </summary>
        public List<NotificationInfo> GetOverdueNotifications(List<ToDoItem> items)
        {
            var notifications = new List<NotificationInfo>();

            if (items == null || items.Count == 0)
            {
                return notifications;
            }

            var today = DateTime.Today;
            var overdueItems = items
                .Where(x =>
                    x.DueDate.HasValue &&
                    x.DueDate.Value.Date < today &&
                    x.Status != ToDoStatus.Completed)
                .ToList();

            foreach (var item in overdueItems)
            {
                var daysOverdue = item.DueDate.HasValue 
                    ? (int)(today - item.DueDate.Value.Date).TotalDays 
                    : 0;

                notifications.Add(new NotificationInfo
                {
                    Type = "overdue",
                    Title = "Công việc quá hạn",
                    Message = $"'{item.Title}' đã quá hạn {daysOverdue} ngày. Vui lòng hoàn thành càng sớm càng tốt.",
                    Level = daysOverdue > 7 ? "danger" : "warning",
                    RelatedItem = item
                });
            }

            return notifications;
        }

        /// <summary>
        /// Lấy thông báo về công việc sắp tới hạn
        /// </summary>
        public List<NotificationInfo> GetDueSoonNotifications(List<ToDoItem> items, int daysUntilDue = 7)
        {
            var notifications = new List<NotificationInfo>();

            if (items == null || items.Count == 0)
            {
                return notifications;
            }

            var today = DateTime.Today;
            var dueDateThreshold = today.AddDays(daysUntilDue);

            var dueSoonItems = items
                .Where(x =>
                    x.DueDate.HasValue &&
                    x.DueDate.Value.Date >= today &&
                    x.DueDate.Value.Date <= dueDateThreshold &&
                    x.Status != ToDoStatus.Completed)
                .OrderBy(x => x.DueDate)
                .ToList();

            foreach (var item in dueSoonItems)
            {
                var daysUntilDueDate = item.DueDate.HasValue
                    ? (int)(item.DueDate.Value.Date - today).TotalDays
                    : 0;
                var levelText = daysUntilDueDate <= 1 ? "danger" : daysUntilDueDate <= 3 ? "warning" : "info";

                notifications.Add(new NotificationInfo
                {
                    Type = "due_soon",
                    Title = "Công việc sắp tới hạn",
                    Message = $"'{item.Title}' sẽ tới hạn trong {daysUntilDueDate} ngày (ngày {item.DueDate?.ToString("dd/MM/yyyy")}).",
                    Level = levelText,
                    RelatedItem = item
                });
            }

            return notifications;
        }

        /// <summary>
        /// Lấy thông báo về công việc ưu tiên cao chưa hoàn thành
        /// </summary>
        public List<NotificationInfo> GetHighPriorityNotifications(List<ToDoItem> items)
        {
            var notifications = new List<NotificationInfo>();

            if (items == null || items.Count == 0)
            {
                return notifications;
            }

            var highPriorityItems = items
                .Where(x =>
                    x.Priority == Priority.High &&
                    x.Status != ToDoStatus.Completed)
                .OrderByDescending(x => x.DueDate ?? DateTime.MaxValue)
                .ToList();

            foreach (var item in highPriorityItems)
            {
                notifications.Add(new NotificationInfo
                {
                    Type = "high_priority",
                    Title = "Công việc ưu tiên cao",
                    Message = $"Bạn có công việc ưu tiên cao: '{item.Title}'. Trạng thái: {GetStatusText(item.Status)}.",
                    Level = "warning",
                    RelatedItem = item
                });
            }

            return notifications;
        }

        /// <summary>
        /// Lấy thông báo về công việc đang thực hiện
        /// </summary>
        public List<NotificationInfo> GetInProgressNotifications(List<ToDoItem> items)
        {
            var notifications = new List<NotificationInfo>();

            if (items == null || items.Count == 0)
            {
                return notifications;
            }

            var inProgressItems = items
                .Where(x => x.Status == ToDoStatus.InProgress)
                .ToList();

            // Tạo một thông báo tổng hợp
            if (inProgressItems.Count > 0)
            {
                notifications.Add(new NotificationInfo
                {
                    Type = "in_progress",
                    Title = "Công việc đang thực hiện",
                    Message = $"Bạn có {inProgressItems.Count} công việc đang thực hiện.",
                    Level = "info",
                    RelatedItem = null
                });

                // Thêm chi tiết nếu số lượng không quá nhiều
                if (inProgressItems.Count <= 5)
                {
                    foreach (var item in inProgressItems)
                    {
                        notifications.Add(new NotificationInfo
                        {
                            Type = "in_progress_detail",
                            Title = "Chi tiết công việc đang thực hiện",
                            Message = $"- '{item.Title}' (Ưu tiên: {GetPriorityText(item.Priority)})",
                            Level = "info",
                            RelatedItem = item
                        });
                    }
                }
            }

            return notifications;
        }

        /// <summary>
        /// Lấy tất cả thông báo quan trọng
        /// </summary>
        public List<NotificationInfo> GetAllImportantNotifications(List<ToDoItem> items)
        {
            var allNotifications = new List<NotificationInfo>();

            // Thêm thông báo quá hạn (mức độ cao nhất)
            allNotifications.AddRange(GetOverdueNotifications(items));

            // Thêm thông báo ưu tiên cao
            allNotifications.AddRange(GetHighPriorityNotifications(items));

            // Thêm thông báo sắp tới hạn
            allNotifications.AddRange(GetDueSoonNotifications(items, 3)); // Chỉ 3 ngày

            // Lọc để giữ tối đa 10 thông báo quan trọng nhất
            return allNotifications
                .OrderByDescending(x => x.Level == "danger" ? 3 : x.Level == "warning" ? 2 : 1)
                .ThenByDescending(x => x.CreatedAt)
                .Take(10)
                .ToList();
        }

        /// <summary>
        /// Lấy text trạng thái tiếng Việt
        /// </summary>
        private static string GetStatusText(ToDoStatus status)
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
        /// Lấy text ưu tiên tiếng Việt
        /// </summary>
        private static string GetPriorityText(Priority priority)
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
