using System;
using System.Collections.Generic;
using System.Linq;

namespace ToDoList.Services
{
    /// <summary>
    /// DTO chứa thông tin lỗi validation
    /// </summary>
    public class ValidationError
    {
        /// <summary>Tên trường có lỗi</summary>
        public string FieldName { get; set; } = string.Empty;

        /// <summary>Mã lỗi (error code)</summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>Thông báo lỗi chi tiết</summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>Giá trị được nhập</summary>
        public object? AttemptedValue { get; set; }

        /// <summary>Mức độ nghiêm trọng: Info, Warning, Error</summary>
        public string Severity { get; set; } = "Error";
    }

    /// <summary>
    /// Kết quả validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>Kết quả: true = hợp lệ, false = có lỗi</summary>
        public bool IsValid { get; set; } = true;

        /// <summary>Danh sách các lỗi tìm thấy</summary>
        public List<ValidationError> Errors { get; set; } = new List<ValidationError>();

        /// <summary>Tổng số lỗi</summary>
        public int ErrorCount => Errors.Count;

        /// <summary>Lỗi đầu tiên (nếu có)</summary>
        public ValidationError? FirstError => Errors.FirstOrDefault();

        /// <summary>Tất cả lỗi dưới dạng string</summary>
        public string AllErrorsAsString => string.Join(", ", Errors.Select(e => e.Message));
    }

    /// <summary>
    /// Interface IValidationService định nghĩa các phương thức validation
    /// Áp dụng Single Responsibility Principle
    /// </summary>
    public interface IValidationService
    {
        /// <summary>
        /// Validate tiêu đề công việc
        /// </summary>
        /// <param name="title">Tiêu đề cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateTitle(string? title);

        /// <summary>
        /// Validate mô tả công việc
        /// </summary>
        /// <param name="description">Mô tả cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateDescription(string? description);

        /// <summary>
        /// Validate email
        /// </summary>
        /// <param name="email">Email cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateEmail(string? email);

        /// <summary>
        /// Validate mật khẩu
        /// </summary>
        /// <param name="password">Mật khẩu cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidatePassword(string? password);

        /// <summary>
        /// Validate tên người dùng
        /// </summary>
        /// <param name="fullName">Tên cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateFullName(string? fullName);

        /// <summary>
        /// Validate ngày hạn chót
        /// </summary>
        /// <param name="dueDate">Ngày hạn chót cần validate</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateDueDate(DateTime? dueDate);

        /// <summary>
        /// Validate toàn bộ ToDoItem
        /// </summary>
        /// <param name="title">Tiêu đề</param>
        /// <param name="description">Mô tả</param>
        /// <param name="dueDate">Ngày hạn chót</param>
        /// <returns>Kết quả validation tổng hợp</returns>
        ValidationResult ValidateToDoItem(string? title, string? description, DateTime? dueDate);

        /// <summary>
        /// Validate dữ liệu đăng ký
        /// </summary>
        /// <param name="email">Email</param>
        /// <param name="fullName">Tên đầy đủ</param>
        /// <param name="password">Mật khẩu</param>
        /// <param name="confirmPassword">Xác nhận mật khẩu</param>
        /// <returns>Kết quả validation</returns>
        ValidationResult ValidateRegistration(string? email, string? fullName, string? password, string? confirmPassword);
    }
}
