using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketmaster.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTasks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedTo",
                table: "TaskItem",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAEEx4qcUjf8CykDL4VkOrj+7jiO21KkivkYrYXze+1yth1p19Dt2I4kAmoH3nA/6RYQ==");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItem_AssignedTo",
                table: "TaskItem",
                column: "AssignedTo");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskItem_Employee_AssignedTo",
                table: "TaskItem",
                column: "AssignedTo",
                principalTable: "Employee",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskItem_Employee_AssignedTo",
                table: "TaskItem");

            migrationBuilder.DropIndex(
                name: "IX_TaskItem_AssignedTo",
                table: "TaskItem");

            migrationBuilder.DropColumn(
                name: "AssignedTo",
                table: "TaskItem");

            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAEAhyn1Y3U4qD1TLISIlX+8cIbAZJy8qU//OQlRgNhDl4gWBHBoTBIvniZg/YptkwGA==");
        }
    }
}
