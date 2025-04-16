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
                value: "AQAAAAIAAYagAAAAEAhyn1Y3U4qD1TLISIlX+8cIbAZJy8qU//OQlRgNhDl4gWBHBoTBIvniZg/YptkwGA==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Employee",
                keyColumn: "Id",
                keyValue: 1,
                column: "Pword",
                value: "AQAAAAIAAYagAAAAENjHEpOgSW4plLWvF2f76tsPNQ+jRE1GURqVsw1gHBb/8IQJ7MqDMOhbpHAztrmN6g==");
        }
    }
}
