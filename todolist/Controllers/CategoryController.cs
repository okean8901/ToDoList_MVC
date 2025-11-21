using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    /// <summary>
    /// Controller quản lý danh mục công việc
    /// </summary>
    [Authorize]
    [Route("[controller]")]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<CategoryController> _logger;

        /// <summary>
        /// Constructor cho CategoryController
        /// </summary>
        public CategoryController(
            ICategoryService categoryService,
            UserManager<ApplicationUser> userManager,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Hiển thị danh sách tất cả danh mục
        /// </summary>
        [HttpGet("index")]
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var categories = await _categoryService.GetCategoriesAsync(user.Id);
            return View(categories);
        }

        /// <summary>
        /// Hiển thị form tạo danh mục mới
        /// </summary>
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Tạo danh mục mới
        /// </summary>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var result = await _categoryService.CreateCategoryAsync(
                user.Id,
                model.Name,
                model.Description,
                model.Color);

            if (result != null)
            {
                TempData["SuccessMessage"] = $"Danh mục '{model.Name}' đã được tạo thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Không thể tạo danh mục. Vui lòng kiểm tra lại dữ liệu.");
            return View(model);
        }

        /// <summary>
        /// Hiển thị form chỉnh sửa danh mục
        /// </summary>
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var category = await _categoryService.GetCategoryByIdAsync(id, user.Id);
            if (category == null)
                return NotFound();

            return View(category);
        }

        /// <summary>
        /// Cập nhật danh mục
        /// </summary>
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var result = await _categoryService.UpdateCategoryAsync(
                id,
                user.Id,
                model.Name,
                model.Description,
                model.Color);

            if (result)
            {
                TempData["SuccessMessage"] = $"Danh mục '{model.Name}' đã được cập nhật thành công!";
                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Không thể cập nhật danh mục. Vui lòng kiểm tra lại dữ liệu.");
            return View(model);
        }

        /// <summary>
        /// Xóa danh mục
        /// </summary>
        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login", "Auth");

            var category = await _categoryService.GetCategoryByIdAsync(id, user.Id);
            if (category == null)
                return NotFound();

            var result = await _categoryService.DeleteCategoryAsync(id, user.Id);
            if (result)
            {
                TempData["SuccessMessage"] = $"Danh mục '{category.Name}' đã được xóa thành công!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Không thể xóa danh mục. Vui lòng thử lại.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// API: Lấy danh sách danh mục (dùng cho AJAX)
        /// </summary>
        [HttpGet("api/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var categories = await _categoryService.GetCategoriesAsync(user.Id);
            return Json(categories.Select(c => new { id = c.Id, name = c.Name, color = c.Color }));
        }

        /// <summary>
        /// API: Lấy thông tin chi tiết danh mục
        /// </summary>
        [HttpGet("api/categories/{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var category = await _categoryService.GetCategoryByIdAsync(id, user.Id);
            if (category == null)
                return NotFound();

            var itemCount = await _categoryService.GetCategoryItemCountAsync(id, user.Id);
            return Json(new
            {
                id = category.Id,
                name = category.Name,
                description = category.Description,
                color = category.Color,
                itemCount = itemCount
            });
        }
    }
}
