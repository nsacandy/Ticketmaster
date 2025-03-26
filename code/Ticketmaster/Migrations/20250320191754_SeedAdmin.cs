using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketmaster.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAEOfJutc11x46Shi1uOq6zdozoRZUrzN6wgA0uuTG6gBR3xTV7iDuo3tDZWwj/t/r5A==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAELKC5cm4ZxIaVm1ZYZkV2iNRjc8Fji9hPJRKWg5+cU8krHyxxkuunzWDvA1KAv0Jfg==");
        }
    }
}
