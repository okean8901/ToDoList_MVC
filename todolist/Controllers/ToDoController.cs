using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDoList.Models;
using ToDoList.Models.ViewModels;
using ToDoList.Services;

namespace ToDoList.Controllers
{
    /// <summary>
    /// ToDoController quản lý tất cả các chức năng liên quan đến công việc cần làm:
    /// - Tạo mới (Create)
    /// - Xem danh sách (Read)
    /// - Cập nhật (Update)
    /// - Xóa (Delete)
    /// - Lọc nâng cao
    /// 
    /// Yêu cầu xác thực người dùng
    /// </summary>
    [Authorize]
    [Route("todo")]
    public class ToDoController : Controller
    {
        private readonly IToDoService _toDoService;
        private readonly IToDoFilterService _filterService;
        private readonly IValidationService _validationService;
        private readonly IExportService _exportService;
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ToDoController> _logger;

        /// <summary>
        /// Constructor khởi tạo controller với các dependencies
        /// </summary>
        public ToDoController(
            IToDoService toDoService,
            IToDoFilterService filterService,
            IValidationService validationService,
            IExportService exportService,
            INotificationService notificationService,
            UserManager<ApplicationUser> userManager,
            ILogger<ToDoController> logger)
        {
            _toDoService = toDoService;
            _filterService = filterService;
            _validationService = validationService;
            _exportService = exportService;
            _notificationService = notificationService;
            _userManager = userManager;
            _logger = logger;
        }

        // ===== READ - Danh sách công việc =====

        /// <summary>
        /// Hiển thị danh sách công việc của người dùng hiện tại
        /// Hỗ trợ lọc và tìm kiếm nâng cao
        /// </summary>
        /// <param name="status">Lọc theo trạng thái</param>
        /// <param name="priority">Lọc theo mức độ ưu tiên</param>
        /// <param name="searchText">Tìm kiếm theo từ khóa</param>
        /// <param name="dueDateFrom">Lọc ngày hạn chót từ</param>
        /// <param name="dueDateTo">Lọc ngày hạn chót đến</param>
        /// <param name="sortBy">Sắp xếp theo trường</param>
        /// <param name="sortOrder">Thứ tự sắp xếp</param>
        /// <param name="isCompleted">Chỉ hiển thị công việc đã hoàn thành</param>
        /// <param name="daysUntilDue">Chỉ hiển thị công việc sắp hết hạn</param>
        [HttpGet("")]
        [HttpGet("index")]
        public async Task<IActionResult> Index(
            ToDoStatus? status = null,
            Priority? priority = null,
            string? searchText = null,
            DateTime? dueDateFrom = null,
            DateTime? dueDateTo = null,
            string sortBy = "CreatedAt",
            string sortOrder = "Descending",
            bool? isCompleted = null,
            int? daysUntilDue = null)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Lấy tất cả công việc của người dùng
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Tạo tiêu chí lọc từ các tham số
                var filterCriteria = new ToDoFilterCriteria
                {
                    Status = status,
                    Priority = priority,
                    SearchText = searchText,
                    DueDateFrom = dueDateFrom,
                    DueDateTo = dueDateTo,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    IsCompleted = isCompleted,
                    DaysUntilDue = daysUntilDue
                };

                // Áp dụng bộ lọc
                var filteredItems = _filterService.FilterToDoItems(allItems, filterCriteria);

                // Tính toán thống kê
                var statusCounts = _filterService.CountByStatus(allItems);
                var priorityCounts = _filterService.CountByPriority(allItems);
                var overdueCount = _filterService.GetOverdueItems(allItems).Count;
                var dueSoonCount = _filterService.GetDueItemsSoon(allItems, 7).Count;

                // Tạo view model
                var viewModel = new ToDoListViewModel
                {
                    Items = filteredItems,
                    CurrentFilter = filterCriteria,
                    StatusCounts = statusCounts,
                    PriorityCounts = priorityCounts,
                    OverdueCount = overdueCount,
                    DueSoonCount = dueSoonCount
                };

                _logger.LogInformation($"Người dùng {user.Email} xem danh sách {filteredItems.Count} công việc");

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách công việc");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải danh sách công việc";
                return View(new ToDoListViewModel());
            }
        }

        // ===== CREATE - Tạo mới công việc =====

        /// <summary>
        /// Hiển thị form tạo mới công việc
        /// </summary>
        [HttpGet("create")]
        public IActionResult Create()
        {
            return View(new ToDoItemViewModel());
        }

        /// <summary>
        /// Xử lý tạo mới công việc
        /// </summary>
        /// <param name="model">Thông tin công việc từ form</param>
        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ToDoItemViewModel model)
        {
            // ===== BƯỚC 1: Validate dữ liệu đầu vào =====
            var validationResult = _validationService.ValidateToDoItem(
                model.Title, 
                model.Description, 
                model.DueDate);

            if (!validationResult.IsValid)
            {
                // Thêm tất cả lỗi vào ModelState để hiển thị trên form
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.FieldName, error.Message);
                    _logger.LogWarning($"Validation Error - {error.FieldName}: {error.Message}");
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
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Tạo đối tượng ToDoItem mới
                var toDoItem = new ToDoItem
                {
                    Title = model.Title,
                    Description = model.Description,
                    Status = model.Status,
                    Priority = model.Priority,
                    DueDate = model.DueDate,
                    UserId = user.Id
                };

                // Lưu vào cơ sở dữ liệu thông qua service
                await _toDoService.CreateAsync(toDoItem);

                _logger.LogInformation($"Người dùng {user.Email} tạo công việc: {model.Title}");

                TempData["SuccessMessage"] = "Công việc đã được tạo thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo công việc mới");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo công việc");
                return View(model);
            }
        }

        // ===== UPDATE - Cập nhật công việc =====

        /// <summary>
        /// Hiển thị form cập nhật công việc
        /// </summary>
        /// <param name="id">ID của công việc cần cập nhật</param>
        [HttpGet("edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Lấy công việc từ cơ sở dữ liệu
                var toDoItem = await _toDoService.GetByIdAsync(id, user.Id);
                if (toDoItem == null)
                {
                    return NotFound("Không tìm thấy công việc");
                }

                // Chuyển đổi sang view model
                var model = new ToDoItemViewModel
                {
                    Id = toDoItem.Id,
                    Title = toDoItem.Title,
                    Description = toDoItem.Description,
                    Status = toDoItem.Status,
                    Priority = toDoItem.Priority,
                    DueDate = toDoItem.DueDate
                };

                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy công việc {id}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải công việc";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Xử lý cập nhật công việc
        /// </summary>
        /// <param name="id">ID của công việc cần cập nhật</param>
        /// <param name="model">Thông tin cập nhật từ form</param>
        [HttpPost("edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ToDoItemViewModel model)
        {
            // Kiểm tra ID khớp
            if (id != model.Id)
            {
                return BadRequest("ID không khớp");
            }

            // ===== BƯỚC 1: Validate dữ liệu đầu vào =====
            var validationResult = _validationService.ValidateToDoItem(
                model.Title,
                model.Description,
                model.DueDate);

            if (!validationResult.IsValid)
            {
                // Thêm tất cả lỗi vào ModelState để hiển thị trên form
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.FieldName, error.Message);
                    _logger.LogWarning($"Validation Error - {error.FieldName}: {error.Message}");
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
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Lấy công việc từ cơ sở dữ liệu
                var toDoItem = await _toDoService.GetByIdAsync(id, user.Id);
                if (toDoItem == null)
                {
                    return NotFound("Không tìm thấy công việc");
                }

                // Cập nhật thông tin
                toDoItem.Title = model.Title;
                toDoItem.Description = model.Description;
                toDoItem.Status = model.Status;
                toDoItem.Priority = model.Priority;
                toDoItem.DueDate = model.DueDate;

                // Lưu thay đổi
                await _toDoService.UpdateAsync(toDoItem);

                _logger.LogInformation($"Người dùng {user.Email} cập nhật công việc: {model.Title}");

                TempData["SuccessMessage"] = "Công việc đã được cập nhật thành công!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật công việc");
                ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật công việc");
                return View(model);
            }
        }

        // ===== DELETE - Xóa công việc =====

        /// <summary>
        /// Xóa một công việc (AJAX)
        /// </summary>
        /// <param name="id">ID của công việc cần xóa</param>
        [HttpPost("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Xóa công việc
                var result = await _toDoService.DeleteAsync(id, user.Id);

                if (result)
                {
                    _logger.LogInformation($"Người dùng {user.Email} xóa công việc {id}");
                    TempData["SuccessMessage"] = "Công việc đã được xóa thành công!";
                    return Json(new { success = true, message = "Xóa thành công" });
                }
                else
                {
                    return Json(new { success = false, message = "Không tìm thấy công việc" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xóa công việc {id}");
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa" });
            }
        }

        /// <summary>
        /// Hiển thị trang xác nhận xóa công việc
        /// </summary>
        /// <param name="id">ID của công việc cần xóa</param>
        [HttpGet("delete-confirm/{id}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Lấy công việc
                var toDoItem = await _toDoService.GetByIdAsync(id, user.Id);
                if (toDoItem == null)
                {
                    return NotFound("Không tìm thấy công việc");
                }

                return View(toDoItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy công việc {id}");
                TempData["ErrorMessage"] = "Có lỗi xảy ra";
                return RedirectToAction("Index");
            }
        }

        // ===== QUICK ACTIONS - Các hành động nhanh =====

        /// <summary>
        /// Đánh dấu công việc hoàn thành (AJAX)
        /// </summary>
        /// <param name="id">ID của công việc</param>
        [HttpPost("mark-complete/{id}")]
        public async Task<IActionResult> MarkComplete(int id)
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng" });
                }

                // Lấy công việc
                var toDoItem = await _toDoService.GetByIdAsync(id, user.Id);
                if (toDoItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy công việc" });
                }

                // Cập nhật trạng thái
                toDoItem.Status = ToDoStatus.Completed;
                await _toDoService.UpdateAsync(toDoItem);

                _logger.LogInformation($"Người dùng {user.Email} đánh dấu hoàn thành công việc {id}");

                return Json(new { success = true, message = "Đã đánh dấu hoàn thành" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi đánh dấu hoàn thành");
                return Json(new { success = false, message = "Có lỗi xảy ra" });
            }
        }

        // ===== EXPORT - Xuất dữ liệu =====

        /// <summary>
        /// Xuất danh sách công việc sang CSV
        /// </summary>
        [HttpGet("export-csv")]
        public async Task<IActionResult> ExportCsv()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người dùng";
                    return RedirectToAction("Index");
                }

                // Lấy tất cả công việc của người dùng
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Xuất sang CSV
                var csvContent = _exportService.ExportTooCsv(allItems);

                // Trả về file download
                var fileName = $"ToDoList_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                var bytes = System.Text.Encoding.UTF8.GetBytes(csvContent);

                _logger.LogInformation($"Người dùng {user.Email} xuất danh sách công việc sang CSV");

                return File(bytes, "text/csv", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xuất CSV");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xuất dữ liệu";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Lấy thống kê chi tiết
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> Statistics()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Lấy tất cả công việc
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Tính toán thống kê
                var statistics = _exportService.GetStatistics(allItems);

                _logger.LogInformation($"Người dùng {user.Email} xem thống kê");

                return View(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thống kê";
                return RedirectToAction("Index");
            }
        }

        // ===== NOTIFICATIONS - Thông báo và Reminder =====

        /// <summary>
        /// Lấy danh sách thông báo quan trọng
        /// </summary>
        [HttpGet("notifications")]
        public async Task<IActionResult> Notifications()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                // Lấy tất cả công việc
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Lấy thông báo quan trọng
                var notifications = _notificationService.GetAllImportantNotifications(allItems);

                _logger.LogInformation($"Người dùng {user.Email} xem danh sách thông báo");

                return View(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo");
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi tải thông báo";
                return RedirectToAction("Index");
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo quá hạn (API)
        /// </summary>
        [HttpGet("api/notifications/overdue")]
        [Produces("application/json")]
        public async Task<IActionResult> ApiGetOverdueNotifications()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng", notifications = new List<object>() });
                }

                // Lấy tất cả công việc
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Lấy thông báo quá hạn
                var notifications = _notificationService.GetOverdueNotifications(allItems);

                // Chuyển đổi sang JSON
                var result = notifications.Select(n => new
                {
                    type = n.Type,
                    title = n.Title,
                    message = n.Message,
                    level = n.Level,
                    itemId = n.RelatedItem?.Id
                }).ToList();

                return Json(new { success = true, count = result.Count, notifications = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo quá hạn");
                return Json(new { success = false, message = "Có lỗi xảy ra", notifications = new List<object>() });
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo sắp tới hạn (API)
        /// </summary>
        [HttpGet("api/notifications/due-soon")]
        [Produces("application/json")]
        public async Task<IActionResult> ApiGetDueSoonNotifications()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng", notifications = new List<object>() });
                }

                // Lấy tất cả công việc
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Lấy thông báo sắp tới hạn
                var notifications = _notificationService.GetDueSoonNotifications(allItems, 7);

                // Chuyển đổi sang JSON
                var result = notifications.Select(n => new
                {
                    type = n.Type,
                    title = n.Title,
                    message = n.Message,
                    level = n.Level,
                    itemId = n.RelatedItem?.Id
                }).ToList();

                return Json(new { success = true, count = result.Count, notifications = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo sắp tới hạn");
                return Json(new { success = false, message = "Có lỗi xảy ra", notifications = new List<object>() });
            }
        }

        /// <summary>
        /// Lấy danh sách thông báo ưu tiên cao (API)
        /// </summary>
        [HttpGet("api/notifications/high-priority")]
        [Produces("application/json")]
        public async Task<IActionResult> ApiGetHighPriorityNotifications()
        {
            try
            {
                // Lấy thông tin người dùng hiện tại
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy người dùng", notifications = new List<object>() });
                }

                // Lấy tất cả công việc
                var allItems = await _toDoService.GetAllByUserIdAsync(user.Id);

                // Lấy thông báo ưu tiên cao
                var notifications = _notificationService.GetHighPriorityNotifications(allItems);

                // Chuyển đổi sang JSON
                var result = notifications.Select(n => new
                {
                    type = n.Type,
                    title = n.Title,
                    message = n.Message,
                    level = n.Level,
                    itemId = n.RelatedItem?.Id
                }).ToList();

                return Json(new { success = true, count = result.Count, notifications = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thông báo ưu tiên cao");
                return Json(new { success = false, message = "Có lỗi xảy ra", notifications = new List<object>() });
            }
        }
    }
}
