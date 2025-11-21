using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace todolist.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderIsStarred : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStarred",
                table: "ToDoItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "ToDoItems",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_UserId_IsStarred",
                table: "ToDoItems",
                columns: new[] { "UserId", "IsStarred" });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoItems_UserId_Order",
                table: "ToDoItems",
                columns: new[] { "UserId", "Order" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ToDoItems_UserId_IsStarred",
                table: "ToDoItems");

            migrationBuilder.DropIndex(
                name: "IX_ToDoItems_UserId_Order",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "IsStarred",
                table: "ToDoItems");

            migrationBuilder.DropColumn(
                name: "Order",
                table: "ToDoItems");
        }
    }
}
