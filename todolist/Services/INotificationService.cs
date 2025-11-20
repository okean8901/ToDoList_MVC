using System;
using System.Collections.Generic;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// DTO chứa thông tin thông báo
    /// </summary>
    public class NotificationInfo
    {
        /// <summary>Loại thông báo</summary>
        public string Type { get; set; } = string.Empty;

        /// <summary>Tiêu đề thông báo</summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>Nội dung thông báo</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Mức độ ưu tiên: info, warning, danger</summary>
        public string Level { get; set; } = "info";

        /// <summary>Dữ liệu liên quan (công việc nếu có)</summary>
        public ToDoItem? RelatedItem { get; set; }

        /// <summary>Ngày tạo thông báo</summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// INotificationService: Interface cho hệ thống thông báo
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Lấy danh sách công việc quá hạn
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <returns>Danh sách thông báo quá hạn</returns>
        List<NotificationInfo> GetOverdueNotifications(List<ToDoItem> items);

        /// <summary>
        /// Lấy danh sách công việc sắp tới hạn
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <param name="daysUntilDue">Số ngày để cảnh báo (mặc định 7)</param>
        /// <returns>Danh sách thông báo sắp tới hạn</returns>
        List<NotificationInfo> GetDueSoonNotifications(List<ToDoItem> items, int daysUntilDue = 7);

        /// <summary>
        /// Lấy danh sách công việc ưu tiên cao chưa hoàn thành
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <returns>Danh sách thông báo ưu tiên cao</returns>
        List<NotificationInfo> GetHighPriorityNotifications(List<ToDoItem> items);

        /// <summary>
        /// Lấy danh sách công việc đang thực hiện
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <returns>Danh sách thông báo công việc đang thực hiện</returns>
        List<NotificationInfo> GetInProgressNotifications(List<ToDoItem> items);

        /// <summary>
        /// Lấy tất cả thông báo quan trọng
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <returns>Danh sách tất cả thông báo quan trọng</returns>
        List<NotificationInfo> GetAllImportantNotifications(List<ToDoItem> items);
    }
}
