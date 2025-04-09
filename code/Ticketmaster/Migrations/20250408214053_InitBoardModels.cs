using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ticketmaster.Migrations
{
    /// <inheritdoc />
    public partial class InitBoardModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pword = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNum = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ERole = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employee", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProjectDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InvolvedGroups = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProjectLeadId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                });

            migrationBuilder.CreateTable(
                name: "Manager",
                columns: table => new
                {
                    ManagerId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Manager", x => x.ManagerId);
                    table.ForeignKey(
                        name: "FK_Manager_Employee_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Employee",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Board",
                columns: table => new
                {
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentProjectId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Board", x => new { x.Title, x.ParentProjectId });
                    table.ForeignKey(
                        name: "FK_Board_Project_ParentProjectId",
                        column: x => x.ParentProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ManagerId = table.Column<int>(type: "int", nullable: true),
                    EmployeeIds = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupId);
                    table.ForeignKey(
                        name: "FK_Groups_Manager_ManagerId",
                        column: x => x.ManagerId,
                        principalTable: "Manager",
                        principalColumn: "ManagerId");
                });

            migrationBuilder.CreateTable(
                name: "BoardTask",
                columns: table => new
                {
                    TaskTitle = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ParentBoardId = table.Column<int>(type: "int", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentBoardTitle = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ParentBoardParentProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardTask", x => new { x.ParentBoardId, x.TaskTitle });
                    table.ForeignKey(
                        name: "FK_BoardTask_Board_ParentBoardTitle_ParentBoardParentProjectId",
                        columns: x => new { x.ParentBoardTitle, x.ParentBoardParentProjectId },
                        principalTable: "Board",
                        principalColumns: new[] { "Title", "ParentProjectId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Employee",
                columns: new[] { "Id", "ERole", "Email", "FirstName", "LastName", "PhoneNum", "Pword" },
                values: new object[] { 1, "admin", "admin@ticketmaster.com", "Admin", "User", "123-456-7890", "AQAAAAIAAYagAAAAEDcDC4XjTw9FQA74cKwM/zxETLtgCJ20mT3HKdvyelI5GIxiE0rIKfcRPYQhdEnV/A==" });

            migrationBuilder.CreateIndex(
                name: "IX_Board_ParentProjectId",
                table: "Board",
                column: "ParentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardTask_ParentBoardTitle_ParentBoardParentProjectId",
                table: "BoardTask",
                columns: new[] { "ParentBoardTitle", "ParentBoardParentProjectId" });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ManagerId",
                table: "Groups",
                column: "ManagerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BoardTask");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Board");

            migrationBuilder.DropTable(
                name: "Manager");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "Employee");
        }
    }
}
