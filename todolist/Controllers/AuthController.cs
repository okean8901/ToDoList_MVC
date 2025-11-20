using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using ToDoList.Models.Auth;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    /// <summary>
    /// AuthController quản lý các chức năng xác thực:
    /// - Đăng ký tài khoản mới
    /// - Đăng nhập
    /// - Đăng xuất
    /// - Xem/cập nhật profile
    /// </summary>
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IValidationService _validationService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// Constructor khởi tạo controller với các dependency
        /// </summary>
        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IValidationService validationService,
            ILogger<AuthController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _validationService = validationService;
            _logger = logger;
        }

        // ===== ĐĂNG KÝ =====

        /// <summary>
        /// Hiển thị trang đăng ký
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// Xử lý đăng ký tài khoản mới
        /// </summary>
        /// <param name="model">Thông tin đăng ký từ form</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // ===== BƯỚC 1: Validate dữ liệu đầu vào =====
            var validationResult = _validationService.ValidateRegistration(
                model.Email,
                model.FullName,
                model.Password,
                model.ConfirmPassword);

            if (!validationResult.IsValid)
            {
                // Thêm tất cả lỗi vào ModelState để hiển thị trên form
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.FieldName, error.Message);
                    _logger.LogWarning($"Registration Validation Error - {error.FieldName}: {error.Message}");
                }
                return View(model);
            }

            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kiểm tra email đã tồn tại chưa
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Email này đã được đăng ký. Vui lòng sử dụng email khác.");
                    _logger.LogWarning($"Cố gắng đăng ký với email đã tồn tại: {model.Email}");
                    return View(model);
                }

                // Tạo đối tượng user mới
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    CreatedAt = DateTime.UtcNow
                };

                // Tạo user trong cơ sở dữ liệu
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Người dùng {user.Email} đã đăng ký thành công");

                    // Tự động đăng nhập sau khi đăng ký
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // Chuyển hướng đến trang chủ
                    return RedirectToAction("Index", "Home");
                }

                // Nếu có lỗi, thêm vào ModelState
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký người dùng");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đăng ký. Vui lòng thử lại.");
            }

            return View(model);
        }

        // ===== ĐĂNG NHẬP =====

        /// <summary>
        /// Hiển thị trang đăng nhập
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// Xử lý đăng nhập
        /// </summary>
        /// <param name="model">Thông tin đăng nhập từ form</param>
        /// <param name="returnUrl">URL để quay lại sau khi đăng nhập</param>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            // ===== BƯỚC 1: Validate dữ liệu đầu vào =====
            var emailValidation = _validationService.ValidateEmail(model.Email);
            if (!emailValidation.IsValid)
            {
                foreach (var error in emailValidation.Errors)
                {
                    ModelState.AddModelError(error.FieldName, error.Message);
                    _logger.LogWarning($"Login Validation Error - {error.FieldName}: {error.Message}");
                }
                return View(model);
            }

            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Mật khẩu không được để trống");
                return View(model);
            }

            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Đăng nhập người dùng
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    model.RememberMe,
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"Người dùng {model.Email} đã đăng nhập thành công");

                    // Nếu có returnUrl, quay lại URL đó. Ngược lại quay về trang chủ
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                // Nếu đăng nhập thất bại
                ModelState.AddModelError(string.Empty, "Email hoặc mật khẩu không chính xác");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi đăng nhập. Vui lòng thử lại.");
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(model);
        }

        // ===== ĐĂNG XUẤT =====

        /// <summary>
        /// Xử lý đăng xuất
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("Người dùng đã đăng xuất");
            return RedirectToAction("Index", "Home");
        }

        // ===== PROFILE =====

        /// <summary>
        /// Hiển thị profile của người dùng hiện tại
        /// </summary>
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            // Lấy thông tin người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Tạo view model từ user
            var model = new ProfileViewModel
            {
                Email = user.Email ?? string.Empty,
                FullName = user.FullName ?? string.Empty,
                CreatedAt = user.CreatedAt
            };

            return View(model);
        }
    }
}
