# To Do List — Tổng quan và Hướng dẫn cài đặt

## 1) Tổng quan dự án
Ứng dụng Quản lý Công việc (To Do List) xây dựng bằng ASP.NET Core 10, cung cấp các chức năng chính:

- Đăng ký/Đăng nhập người dùng (ASP.NET Identity).
- CRUD công việc: tạo, sửa, xóa, xem chi tiết.
- Lọc nâng cao (trạng thái, ưu tiên, ngày hạn, danh mục, tìm kiếm).
- Hệ thống Category cho phân loại công việc theo người dùng.
- Audit Log: lưu lịch sử thay đổi (Create / Update / Delete) cho từng công việc.
- Thống kê cơ bản và biểu đồ (Chart.js).
- Calendar view (FullCalendar) để xem công việc theo ngày.
- Sắp xếp thủ công (Order) và Drag & Drop hỗ trợ cập nhật thứ tự.
- Đánh dấu công việc quan trọng (IsStarred).
- Export CSV, thông báo và validation.

Các service chính:
- `IToDoService` / `ToDoService` — CRUD và các thao tác với `ToDoItem`.
- `IToDoFilterService` — logic lọc và thống kê cơ bản.
- `IAuditService` / `AuditService` — ghi và truy vấn lịch sử thay đổi.

Migrations gần đây:
- `AddAuditLog` — thêm bảng `AuditLogs`.
- `AddOrderIsStarred` — thêm cột `Order` (int?) và `IsStarred` (bool) vào `ToDoItems`.


## 2) Cách setup & chạy dự án (Windows PowerShell)

Yêu cầu trước khi cài:
- .NET 10 SDK (hoặc mới hơn).
- SQL Server (instance: `DESKTOP-25FPKFN\\SQLEXPRESS`).
- (Tùy chọn) Visual Studio / VS Code.

Các bước:

1) Lấy mã nguồn và vào thư mục dự án:

```powershell
cd D:\codec#\ToDoList\todolist
```

2) Cập nhật `appsettings.json` (connection string). Ví dụ:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-25FPKFN\\\\SQLEXPRESS;Database=todolist;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3) Khôi phục dependencies:

```powershell
dotnet restore
```

4) Tạo và áp dụng migrations (nếu cần). Thư mục `Migrations/` trong repo đã chứa các migration đã tạo sẵn. Nếu bạn muốn tạo migration mới sau khi thay đổi model:

```powershell
# Tạo migration mới (tên tuỳ ý)
dotnet ef migrations add YourMigrationName

# Áp dụng mọi migration lên database
dotnet ef database update
```

5) Chạy ứng dụng:

```powershell
dotnet run
```

Mặc định địa chỉ truy cập (có thể khác trên máy bạn):
- HTTPS: `https://localhost:7001`
- HTTP: `http://localhost:5000`


### Một vài lưu ý hữu ích
- Nếu database đã có migrations trong `Migrations/`, chỉ cần chạy `dotnet ef database update` để sync schema.
- Để rollback migration cuối cùng:

```powershell
dotnet ef migrations remove
dotnet ef database update
```

- Nếu gặp lỗi kết nối, kiểm tra:
  - SQL Server đang chạy và instance name đúng.
  - Chuỗi kết nối trong `appsettings.json`.


---

Nếu bạn muốn, tôi có thể thêm hướng dẫn ngắn về UI drag&drop hoặc ví dụ payload cho các endpoint như:
- `GET /todo/audit/{id}`
- `POST /todo/api/update-order` (payload: JSON array of ids)
- `POST /todo/api/toggle-star/{id}`

