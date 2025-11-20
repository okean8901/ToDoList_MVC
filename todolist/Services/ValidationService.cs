using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ToDoList.Services
{
    /// <summary>
    /// ValidationService: Xử lý kiểm tra và báo lỗi dữ liệu đầu vào
    /// Áp dụng Open/Closed Principle - dễ mở rộng
    /// </summary>
    public class ValidationService : IValidationService
    {
        // Regex patterns cho validation
        private static readonly string EmailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        /// <summary>
        /// Validate tiêu đề: không trống, 1-200 ký tự
        /// </summary>
        public ValidationResult ValidateTitle(string? title)
        {
            var result = new ValidationResult();

            // Kiểm tra null hoặc trống
            if (string.IsNullOrWhiteSpace(title))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Title",
                    ErrorCode = "TITLE_REQUIRED",
                    Message = "Tiêu đề công việc không được để trống",
                    Severity = "Error"
                });
                return result;
            }

            // Kiểm tra độ dài
            if (title.Length > 200)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Title",
                    ErrorCode = "TITLE_TOO_LONG",
                    Message = "Tiêu đề không được vượt quá 200 ký tự",
                    AttemptedValue = title,
                    Severity = "Error"
                });
                return result;
            }

            // Kiểm tra độ dài tối thiểu
            if (title.Length < 3)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Title",
                    ErrorCode = "TITLE_TOO_SHORT",
                    Message = "Tiêu đề phải có ít nhất 3 ký tự",
                    AttemptedValue = title,
                    Severity = "Warning"
                });
            }

            return result;
        }

        /// <summary>
        /// Validate mô tả: tùy chọn, tối đa 2000 ký tự
        /// </summary>
        public ValidationResult ValidateDescription(string? description)
        {
            var result = new ValidationResult();

            // Mô tả không bắt buộc
            if (string.IsNullOrWhiteSpace(description))
            {
                return result; // Valid
            }

            // Kiểm tra độ dài
            if (description.Length > 2000)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Description",
                    ErrorCode = "DESCRIPTION_TOO_LONG",
                    Message = "Mô tả không được vượt quá 2000 ký tự",
                    AttemptedValue = description.Substring(0, 50) + "...",
                    Severity = "Error"
                });
            }

            return result;
        }

        /// <summary>
        /// Validate email: định dạng hợp lệ
        /// </summary>
        public ValidationResult ValidateEmail(string? email)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(email))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Email",
                    ErrorCode = "EMAIL_REQUIRED",
                    Message = "Email không được để trống",
                    Severity = "Error"
                });
                return result;
            }

            // Kiểm tra định dạng email
            if (!Regex.IsMatch(email, EmailPattern))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Email",
                    ErrorCode = "EMAIL_INVALID_FORMAT",
                    Message = "Email không hợp lệ. Vui lòng nhập email theo định dạng: user@example.com",
                    AttemptedValue = email,
                    Severity = "Error"
                });
            }

            // Kiểm tra độ dài
            if (email.Length > 256)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Email",
                    ErrorCode = "EMAIL_TOO_LONG",
                    Message = "Email không được vượt quá 256 ký tự",
                    AttemptedValue = email,
                    Severity = "Error"
                });
            }

            return result;
        }

        /// <summary>
        /// Validate mật khẩu: ít nhất 8 ký tự, chữ hoa, chữ thường, số, ký tự đặc biệt
        /// </summary>
        public ValidationResult ValidatePassword(string? password)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(password))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_REQUIRED",
                    Message = "Mật khẩu không được để trống",
                    Severity = "Error"
                });
                return result;
            }

            // Kiểm tra độ dài tối thiểu
            if (password.Length < 8)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_TOO_SHORT",
                    Message = "Mật khẩu phải có ít nhất 8 ký tự",
                    Severity = "Error"
                });
            }

            // Kiểm tra độ dài tối đa
            if (password.Length > 128)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_TOO_LONG",
                    Message = "Mật khẩu không được vượt quá 128 ký tự",
                    Severity = "Error"
                });
            }

            // Kiểm tra chữ hoa
            if (!Regex.IsMatch(password, "[A-Z]"))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_MISSING_UPPERCASE",
                    Message = "Mật khẩu phải chứa ít nhất một chữ cái hoa (A-Z)",
                    Severity = "Error"
                });
            }

            // Kiểm tra chữ thường
            if (!Regex.IsMatch(password, "[a-z]"))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_MISSING_LOWERCASE",
                    Message = "Mật khẩu phải chứa ít nhất một chữ cái thường (a-z)",
                    Severity = "Error"
                });
            }

            // Kiểm tra số
            if (!Regex.IsMatch(password, "[0-9]"))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_MISSING_DIGIT",
                    Message = "Mật khẩu phải chứa ít nhất một chữ số (0-9)",
                    Severity = "Error"
                });
            }

            // Kiểm tra ký tự đặc biệt
            if (!Regex.IsMatch(password, "[@$!%*?&]"))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "Password",
                    ErrorCode = "PASSWORD_MISSING_SPECIAL_CHAR",
                    Message = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt (@$!%*?&)",
                    Severity = "Error"
                });
            }

            return result;
        }

        /// <summary>
        /// Validate tên người dùng: 2-100 ký tự
        /// </summary>
        public ValidationResult ValidateFullName(string? fullName)
        {
            var result = new ValidationResult();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "FullName",
                    ErrorCode = "FULLNAME_REQUIRED",
                    Message = "Tên đầy đủ không được để trống",
                    Severity = "Error"
                });
                return result;
            }

            // Kiểm tra độ dài tối thiểu
            if (fullName.Length < 2)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "FullName",
                    ErrorCode = "FULLNAME_TOO_SHORT",
                    Message = "Tên phải có ít nhất 2 ký tự",
                    AttemptedValue = fullName,
                    Severity = "Error"
                });
            }

            // Kiểm tra độ dài tối đa
            if (fullName.Length > 100)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "FullName",
                    ErrorCode = "FULLNAME_TOO_LONG",
                    Message = "Tên không được vượt quá 100 ký tự",
                    AttemptedValue = fullName,
                    Severity = "Error"
                });
            }

            return result;
        }

        /// <summary>
        /// Validate ngày hạn chót: phải lớn hơn ngày hiện tại
        /// </summary>
        public ValidationResult ValidateDueDate(DateTime? dueDate)
        {
            var result = new ValidationResult();

            // Ngày hạn chót không bắt buộc
            if (!dueDate.HasValue)
            {
                return result; // Valid
            }

            // Kiểm tra không phải quá khứ
            if (dueDate.Value.Date < DateTime.Today)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "DueDate",
                    ErrorCode = "DUEDATE_IN_PAST",
                    Message = "Ngày hạn chót không được là ngày trong quá khứ",
                    AttemptedValue = dueDate.Value.ToString("yyyy-MM-dd"),
                    Severity = "Warning"
                });
            }

            // Cảnh báo nếu quá xa (>1 năm)
            if (dueDate.Value.Date > DateTime.Today.AddYears(1))
            {
                result.Errors.Add(new ValidationError
                {
                    FieldName = "DueDate",
                    ErrorCode = "DUEDATE_FAR_FUTURE",
                    Message = "Ngày hạn chót quá xa trong tương lai (>1 năm)",
                    AttemptedValue = dueDate.Value.ToString("yyyy-MM-dd"),
                    Severity = "Warning"
                });
            }

            return result;
        }

        /// <summary>
        /// Validate toàn bộ dữ liệu công việc
        /// </summary>
        public ValidationResult ValidateToDoItem(string? title, string? description, DateTime? dueDate)
        {
            var result = new ValidationResult();

            // Validate từng trường
            var titleResult = ValidateTitle(title);
            var descriptionResult = ValidateDescription(description);
            var dueDateResult = ValidateDueDate(dueDate);

            // Gộp tất cả lỗi
            if (!titleResult.IsValid)
            {
                result.Errors.AddRange(titleResult.Errors);
                result.IsValid = false;
            }

            if (!descriptionResult.IsValid)
            {
                result.Errors.AddRange(descriptionResult.Errors);
                result.IsValid = false;
            }

            if (!dueDateResult.IsValid)
            {
                result.Errors.AddRange(dueDateResult.Errors);
                // Chỉ đặt IsValid = false nếu là Error, không phải Warning
                if (dueDateResult.Errors.Any(e => e.Severity == "Error"))
                {
                    result.IsValid = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Validate dữ liệu đăng ký
        /// </summary>
        public ValidationResult ValidateRegistration(string? email, string? fullName, string? password, string? confirmPassword)
        {
            var result = new ValidationResult();

            // Validate từng trường
            var emailResult = ValidateEmail(email);
            var fullNameResult = ValidateFullName(fullName);
            var passwordResult = ValidatePassword(password);

            // Gộp lỗi
            if (!emailResult.IsValid)
            {
                result.Errors.AddRange(emailResult.Errors);
                result.IsValid = false;
            }

            if (!fullNameResult.IsValid)
            {
                result.Errors.AddRange(fullNameResult.Errors);
                result.IsValid = false;
            }

            if (!passwordResult.IsValid)
            {
                result.Errors.AddRange(passwordResult.Errors);
                result.IsValid = false;
            }

            // Kiểm tra xác nhận mật khẩu
            if (string.IsNullOrEmpty(confirmPassword))
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "ConfirmPassword",
                    ErrorCode = "CONFIRM_PASSWORD_REQUIRED",
                    Message = "Vui lòng xác nhận mật khẩu",
                    Severity = "Error"
                });
            }
            else if (password != confirmPassword)
            {
                result.IsValid = false;
                result.Errors.Add(new ValidationError
                {
                    FieldName = "ConfirmPassword",
                    ErrorCode = "PASSWORD_MISMATCH",
                    Message = "Mật khẩu xác nhận không khớp",
                    Severity = "Error"
                });
            }

            return result;
        }
    }
}
