# Ứng dụng Quản lý Công việc (To Do List)

## 📋 Mô tả

Ứng dụng web **Quản lý Công việc** xây dựng bằng ASP.NET Core 10 với các tính năng quản lý công việc toàn diện:
- ✅ Đăng ký/Đăng nhập người dùng
- ✅ CRUD đầy đủ cho công việc
- ✅ Lọc nâng cao (theo trạng thái, ưu tiên, ngày hạn chót)
- ✅ Tìm kiếm theo từ khóa
- ✅ Thống kê công việc
- ✅ Giao diện Bootstrap 5 đẹp mắt

---

## 🛠️ Công nghệ sử dụng

### Backend
- **ASP.NET Core 10**: Framework web .NET mới nhất
- **Entity Framework Core**: ORM cho truy cập cơ sở dữ liệu
- **ASP.NET Identity**: Hệ thống xác thực và quản lý người dùng
- **SQL Server**: Cơ sở dữ liệu quan hệ

### Frontend
- **Razor Views**: Template engine của ASP.NET
- **Bootstrap 5**: Framework CSS responsive
- **Font Awesome 6**: Thư viện icons
- **jQuery**: Thư viện JavaScript
- **jQuery Validation**: Validation cho phía client

---

## 📦 Cấu trúc Dự án

```
ToDoList/
├── Controllers/              # Các controller xử lý logic
│   ├── HomeController.cs     # Trang chủ
│   ├── AuthController.cs     # Đăng nhập/Đăng ký
│   └── ToDoController.cs     # Quản lý công việc
│
├── Models/                   # Các model và entity
│   ├── ApplicationUser.cs    # Entity người dùng
│   ├── ToDoItem.cs          # Entity công việc
│   ├── Auth/
│   │   └── AuthViewModels.cs # ViewModel cho auth
│   └── ViewModels/
│       └── ToDoViewModels.cs # ViewModel cho todo
│
├── Views/                    # Các view (giao diện)
│   ├── Home/                 # Trang chủ
│   ├── Auth/                 # Đăng nhập/Đăng ký
│   ├── ToDo/                 # Quản lý công việc
│   └── Shared/               # Layout chung
│
├── Services/                 # Microservices layer
│   ├── IToDoService.cs      # Interface service
│   ├── ToDoService.cs       # Implement CRUD
│   ├── IToDoFilterService.cs # Interface filter
│   └── ToDoFilterService.cs  # Implement filter
│
├── Data/                     # Database context
│   └── ApplicationDbContext.cs
│
├── wwwroot/                  # Static files
│   ├── css/site.css         # CSS tùy chỉnh
│   ├── js/site.js           # JavaScript tùy chỉnh
│   └── lib/                 # Thư viện bên ngoài
│
├── Migrations/               # EF Core migrations
├── Program.cs               # Cấu hình ứng dụng
├── appsettings.json         # Cấu hình
└── todolist.csproj          # File dự án

```

---

## 🚀 Hướng dẫn Cài đặt

### 1. Yêu cầu hệ thống
- **.NET 10 SDK** hoặc cao hơn
- **SQL Server** (DESKTOP-25FPKFN\SQLEXPRESS)
- **Visual Studio Code** hoặc Visual Studio

### 2. Cấu hình Connection String
Cập nhật `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-25FPKFN\\SQLEXPRESS;Database=todolist;Trusted_Connection=true;Encrypt=true;TrustServerCertificate=true;"
  }
}
```

### 3. Khôi phục Dependencies
```bash
dotnet restore
```

### 4. Tạo Database
```bash
# Tạo migration
dotnet ef migrations add InitialCreate

# Cập nhật database
dotnet ef database update
```

### 5. Chạy Ứng dụng
```bash
dotnet run

# Ứng dụng sẽ chạy tại:
# https://localhost:7001 (HTTPS)
# http://localhost:5000 (HTTP)
```

---

## 📚 Hướng dẫn Sử dụng

### 1. Đăng ký Tài khoản
- Truy cập trang đăng ký
- Nhập email, tên, mật khẩu
- Bấm "Đăng ký"

### 2. Đăng nhập
- Sử dụng email và mật khẩu đã đăng ký
- Tích "Ghi nhớ tôi" để tự động đăng nhập lần sau

### 3. Tạo Công việc Mới
- Bấm nút "+ Công việc mới"
- Nhập tiêu đề (bắt buộc)
- Nhập mô tả chi tiết
- Chọn trạng thái (Chưa làm, Đang làm, Hoàn thành)
- Chọn mức độ ưu tiên (Thấp, Trung bình, Cao)
- Chọn ngày hạn chót (tùy chọn)

### 4. Lọc Công việc
- **Tìm kiếm**: Gõ từ khóa để tìm trong tiêu đề/mô tả
- **Trạng thái**: Lọc theo Chưa làm, Đang làm, Hoàn thành
- **Ưu tiên**: Lọc theo Thấp, Trung bình, Cao
- **Ngày hạn chót**: Chọn khoảng ngày
- **Sắp xếp**: Theo ngày tạo, hạn chót, ưu tiên, tiêu đề

### 5. Cập nhật Công việc
- Bấm nút ✏️ để sửa
- Thay đổi thông tin
- Bấm "Lưu thay đổi"

### 6. Hoàn Thành Công việc
- Bấm nút ✓ để đánh dấu hoàn thành
- Hoặc vào chế độ chỉnh sửa và thay đổi trạng thái

### 7. Xóa Công việc
- Bấm nút 🗑️
- Xác nhận xóa

---

## 🏗️ Kiến trúc Ứng dụng

### Microservices Layer
Ứng dụng sử dụng pattern Microservices với các services độc lập:

#### 1. **ToDoService** - Quản lý CRUD
```csharp
// Interfaces
IToDoService
- CreateAsync()      // Tạo mới
- GetAllByUserIdAsync() // Lấy danh sách
- GetByIdAsync()     // Lấy theo ID
- UpdateAsync()      // Cập nhật
- DeleteAsync()      // Xóa đơn
- DeleteMultipleAsync() // Xóa nhiều
```

#### 2. **ToDoFilterService** - Lọc nâng cao
```csharp
// Interfaces
IToDoFilterService
- FilterToDoItems()  // Lọc dữ liệu
- GetOverdueItems()  // Công việc quá hạn
- GetDueItemsSoon()  // Công việc sắp hết hạn
- CountByStatus()    // Thống kê theo trạng thái
- CountByPriority()  // Thống kê theo ưu tiên
```

### Dependency Injection
Tất cả services được đăng ký trong `Program.cs`:
```csharp
builder.Services.AddScoped<IToDoService, ToDoService>();
builder.Services.AddScoped<IToDoFilterService, ToDoFilterService>();
```

---

## 💾 Cơ sở Dữ liệu

### Schema
```sql
-- Bảng người dùng (AspNetUsers)
Id, Email, FullName, CreatedAt, ...

-- Bảng công việc (ToDoItems)
Id, Title, Description, Status, Priority, 
DueDate, CreatedAt, UpdatedAt, UserId
```

### Enum Values
**ToDoStatus**:
- 0 = Pending (Chưa làm)
- 1 = InProgress (Đang làm)
- 2 = Completed (Hoàn thành)

**Priority**:
- 0 = Low (Thấp)
- 1 = Medium (Trung bình)
- 2 = High (Cao)

---

## 🔐 Bảo mật

1. **Authentication**: Sử dụng ASP.NET Identity
2. **Authorization**: Yêu cầu [Authorize] attribute
3. **Ownership Check**: Kiểm tra quyền sở hữu công việc
4. **CSRF Protection**: Token anti-forgery
5. **Password Security**: Mã hóa mật khẩu

---

## ✨ Tính năng Nổi bật

### 1. Dashboard thống kê
- Tổng số công việc
- Số công việc quá hạn
- Số công việc sắp hết hạn (7 ngày)
- Số công việc hoàn thành

### 2. Lọc nâng cao
- Tìm kiếm full-text
- Lọc theo trạng thái
- Lọc theo ưu tiên
- Lọc theo khoảng ngày hạn chót
- Sắp xếp linh hoạt

### 3. Giao diện thân thiện
- Responsive design (mobile, tablet, desktop)
- Bootstrap 5 UI
- Font Awesome icons
- Animations mượt mà

### 4. Validation
- Validation server-side
- Validation client-side (jQuery)
- Error messages rõ ràng

---

## 📝 Clean Code Principles

### Single Responsibility
- Mỗi class có một trách nhiệm duy nhất
- Controllers xử lý HTTP, Services xử lý logic
- Filters xử lý lọc dữ liệu

### Open/Closed
- Sử dụng interfaces cho extensibility
- Dễ thêm tính năng mới

### Liskov Substitution
- Interfaces kế thừa đúng

### Interface Segregation
- Interfaces nhỏ, cụ thể

### Dependency Inversion
- Inject dependencies qua constructor
- Không hardcode dependencies

---

## 📖 Comment Tiếng Việt

Tất cả code đều có comment bằng tiếng Việt:
- XML documentation comments
- Inline comments giải thích logic phức tạp
- Comment cho properties, methods

---

## 🐛 Xử lý Lỗi

- Global exception handling
- Logging chi tiết
- User-friendly error messages
- Input validation toàn diện

---

## 🔧 Maintenance

### Update Entity
```bash
dotnet ef migrations add NewMigration
dotnet ef database update
```

### Troubleshooting
1. **Connection String Error**: Kiểm tra SQL Server đang chạy
2. **Migration Error**: Xóa migration cuối cùng: `dotnet ef migrations remove`
3. **Build Error**: Chạy `dotnet clean` rồi `dotnet build`

---

## 📄 License

Dự án này là cho mục đích học tập.

---

## 👨‍💻 Thông tin Developer

**Ứng dụng**: To Do List
**Framework**: ASP.NET Core 10
**Database**: SQL Server
**UI**: Bootstrap 5
**Ngôn ngữ**: C#, HTML, CSS, JavaScript

---

## 📞 Hỗ trợ

Nếu gặp vấn đề, vui lòng:
1. Kiểm tra Connection String trong `appsettings.json`
2. Đảm bảo SQL Server đang chạy
3. Chạy lại migration nếu cần
4. Xem logs để diagnose lỗi

---

**Cảm ơn bạn đã sử dụng Ứng dụng Quản lý Công việc!** 🚀
