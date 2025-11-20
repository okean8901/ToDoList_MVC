using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;

namespace ToDoList.Controllers
{
    /// <summary>
    /// HomeController quản lý các trang công khai:
    /// - Trang chủ
    /// - Trang riêng tư
    /// - Trang lỗi
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Constructor khởi tạo controller
        /// </summary>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Trang chủ - hiển thị thông tin ứng dụng
        /// Nếu người dùng đã đăng nhập, chuyển hướng đến danh sách công việc
        /// </summary>
        public IActionResult Index()
        {
            // Nếu người dùng đã đăng nhập, chuyển hướng đến trang danh sách công việc
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "ToDo");
            }

            return View();
        }

        /// <summary>
        /// Trang riêng tư
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Trang lỗi
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
