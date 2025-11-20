using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoList.Models;

namespace ToDoList.Data
{
    /// <summary>
    /// DbContext dùng để quản lý kết nối với cơ sở dữ liệu SQL Server
    /// Kế thừa từ IdentityDbContext để tích hợp ASP.NET Identity
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// DbSet dùng để truy cập bảng ToDoItems
        /// </summary>
        public DbSet<ToDoItem> ToDoItems { get; set; }

        /// <summary>
        /// Cấu hình model khi tạo
        /// </summary>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Cấu hình quan hệ giữa ApplicationUser và ToDoItem
            builder.Entity<ToDoItem>()
                .HasOne(t => t.User)
                .WithMany(u => u.ToDoItems)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tạo index để tăng tốc độ truy vấn
            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.Status, t.Priority });

            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.DueDate });
        }
    }
}
