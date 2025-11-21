using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ToDoList.Data;
using ToDoList.Models;
using ToDoList.Services;

var builder = WebApplication.CreateBuilder(args);

// ===== Cấu hình Entity Framework Core =====
// Kết nối đến SQL Server với connection string từ appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// ===== Cấu hình ASP.NET Identity =====
// Thiết lập Identity cho xác thực và phân quyền
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Yêu cầu cho mật khẩu
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;

    // Yêu cầu cho email
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ===== Dependency Injection - Đăng ký Services =====
// Đăng ký IToDoService
builder.Services.AddScoped<IToDoService, ToDoService>();

// Đăng ký IToDoFilterService
builder.Services.AddScoped<IToDoFilterService, ToDoFilterService>();

// Đăng ký IValidationService cho kiểm tra dữ liệu đầu vào
builder.Services.AddScoped<IValidationService, ValidationService>();

// Đăng ký IExportService cho xuất dữ liệu
builder.Services.AddScoped<IExportService, ExportService>();

// Đăng ký INotificationService cho hệ thống thông báo
builder.Services.AddScoped<INotificationService, NotificationService>();

// Đăng ký ICategoryService cho hệ thống danh mục
builder.Services.AddScoped<ICategoryService, CategoryService>();
// Đăng ký IAuditService cho lưu lịch sử thay đổi
builder.Services.AddScoped<IAuditService, AuditService>();

// ===== Cấu hình Controllers và Views =====
builder.Services.AddControllersWithViews();

// ===== Cấu hình Logging =====
builder.Services.AddLogging();

var app = builder.Build();

// ===== Cấu hình HTTP request pipeline =====
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // Bảo mật HSTS (HTTP Strict Transport Security)
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ===== Cấu hình xác thực và phân quyền =====
app.UseAuthentication();
app.UseAuthorization();

// ===== Cấu hình các route =====
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
