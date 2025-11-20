using System.ComponentModel.DataAnnotations;

namespace ToDoList.Models.Auth
{
    /// <summary>
    /// Model cho form đăng ký tài khoản
    /// </summary>
    public class RegisterViewModel
    {
        /// <summary>Địa chỉ email</summary>
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Tên đầy đủ</summary>
        [Required(ErrorMessage = "Tên không được để trống")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string FullName { get; set; } = string.Empty;

        /// <summary>Mật khẩu</summary>
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(100, MinimumLength = 6, 
            ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>Xác nhận mật khẩu</summary>
        [Required(ErrorMessage = "Xác nhận mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    /// <summary>
    /// Model cho form đăng nhập
    /// </summary>
    public class LoginViewModel
    {
        /// <summary>Địa chỉ email</summary>
        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        /// <summary>Mật khẩu</summary>
        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        /// <summary>Ghi nhớ tôi trong lần đăng nhập tiếp theo</summary>
        public bool RememberMe { get; set; }
    }

    /// <summary>
    /// Model để hiển thị thông tin profile người dùng
    /// </summary>
    public class ProfileViewModel
    {
        /// <summary>Email người dùng</summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>Tên đầy đủ</summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>Ngày tạo tài khoản</summary>
        public DateTime CreatedAt { get; set; }
    }
}
