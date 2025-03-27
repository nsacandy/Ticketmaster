using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketmaster.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAEBDoghCkcCqp1GiKc5hxLdWqUSqOLe7fjIP8+PX412w5v8BNv29YErDu6soG+ME/Ww==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAEOfJutc11x46Shi1uOq6zdozoRZUrzN6wgA0uuTG6gBR3xTV7iDuo3tDZWwj/t/r5A==");
        }
    }
}
