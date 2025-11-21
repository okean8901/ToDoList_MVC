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
        /// DbSet dùng để truy cập bảng Categories
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// DbSet dùng để truy cập bảng AuditLogs
        /// </summary>
        public DbSet<Models.AuditLog> AuditLogs { get; set; }

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

            // Cấu hình quan hệ giữa Category và ToDoItem
            builder.Entity<ToDoItem>()
                .HasOne(t => t.Category)
                .WithMany(c => c.ToDoItems)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);

            // Cấu hình quan hệ giữa ApplicationUser và Category
            builder.Entity<Category>()
                .HasOne(c => c.User)
                .WithMany(u => u.Categories)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Tạo index để tăng tốc độ truy vấn
            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.Status, t.Priority });

            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.DueDate });

            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.CategoryId });

            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.Order });

            builder.Entity<ToDoItem>()
                .HasIndex(t => new { t.UserId, t.IsStarred });

            builder.Entity<Category>()
                .HasIndex(c => new { c.UserId, c.Name });

            // Cấu hình AuditLog
            builder.Entity<Models.AuditLog>()
                .HasOne(a => a.ToDoItem)
                .WithMany()
                .HasForeignKey(a => a.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Models.AuditLog>()
                .HasIndex(a => new { a.ToDoItemId, a.UserId });
        }
    }
}
