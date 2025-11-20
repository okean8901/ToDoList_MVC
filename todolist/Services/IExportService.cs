using System;
using System.Collections.Generic;
using ToDoList.Models;

namespace ToDoList.Services
{
    /// <summary>
    /// IExportService: Interface cho chức năng xuất dữ liệu
    /// Hỗ trợ xuất sang CSV, Excel
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Xuất danh sách công việc sang CSV
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <returns>CSV string</returns>
        string ExportTooCsv(List<ToDoItem> items);

        /// <summary>
        /// Lấy dữ liệu thống kê
        /// </summary>
        /// <param name="items">Danh sách công việc</param>
        /// <returns>Đối tượng thống kê</returns>
        ToDoStatistics GetStatistics(List<ToDoItem> items);
    }

    /// <summary>
    /// DTO chứa thông tin thống kê
    /// </summary>
    public class ToDoStatistics
    {
        /// <summary>Tổng số công việc</summary>
        public int TotalItems { get; set; }

        /// <summary>Số công việc đã hoàn thành</summary>
        public int CompletedItems { get; set; }

        /// <summary>Số công việc chưa hoàn thành</summary>
        public int PendingItems { get; set; }

        /// <summary>Số công việc quá hạn</summary>
        public int OverdueItems { get; set; }

        /// <summary>Số công việc ưu tiên cao</summary>
        public int HighPriorityItems { get; set; }

        /// <summary>Tỷ lệ hoàn thành (%)</summary>
        public decimal CompletionRate { get; set; }

        /// <summary>Số công việc sắp tới hạn (7 ngày)</summary>
        public int DueSoonItems { get; set; }

        /// <summary>Ngày tạo công việc sớm nhất</summary>
        public DateTime? EarliestCreatedDate { get; set; }

        /// <summary>Ngày tạo công việc gần nhất</summary>
        public DateTime? LatestCreatedDate { get; set; }
    }
}
