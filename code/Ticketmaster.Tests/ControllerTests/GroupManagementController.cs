using global::Ticketmaster.Controllers;
using global::Ticketmaster.Data;
using global::Ticketmaster.Models;
using global::Ticketmaster.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Build.Framework;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Ticketmaster.Tests.ControllerTests
{
    public class GroupManagementControllerTests
    {
        private readonly TicketmasterContext _context;
        private readonly GroupManagementController _controller;

        public GroupManagementControllerTests()
        {
            var options = new DbContextOptionsBuilder<TicketmasterContext>()
                .UseInMemoryDatabase(databaseName: "GroupMgmtDb")
                .Options;

            _context = new TicketmasterContext(options);
            _context.Database.EnsureCreated();

            _controller = new GroupManagementController(_context);

            // Simulate a logged-in user
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim("Id", "1"),
                new Claim(ClaimTypes.Role, "admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Index_ReturnsViewWithEmployeesAndGroups()
        {
             _context.Groups.RemoveRange(_context.Groups);
             _context.Employee.RemoveRange(_context.Employee);
            await _context.SaveChangesAsync();            // Arrange
            _context.Employee.Add(new Employee { Id = 22, FirstName = "Test", LastName = "User", Pword = "test", ERole = "standard", Email = "test@ticketmaster.com", PhoneNum = "5555555"});
            _context.Groups.Add(new Group { GroupId = 25, GroupName = "Test Group" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<GroupManagementViewModel>(viewResult.Model);

            Assert.Single(model.Employees);
            Assert.Single(model.Groups);
        }

        [Fact]
        public async Task CreateGroup_ReturnsOk_WhenValid()
        {
            // Arrange
            var request = new CreateGroupRequest
            {
                GroupName = "New Group",
                ManagerId = 2,
                EmployeeIds = new List<int> { 2, 3 }
            };

            _context.Employee.AddRange(
                new Employee
                {
                    Id = 2, FirstName = "bart", Pword = "bart", Email = "bart@simpson.com", PhoneNum = "5555555",
                    ERole = "standard", LastName = "simpson"
                },
                new Employee
                {
                    Id = 3, FirstName = "lisa", Pword = "lisa", Email = "lisa@simpson@gmail.com", ERole = "admin",
                    LastName = "simpson", PhoneNum = "6666666"
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.CreateGroup(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var resultValue = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.Equal("Group created successfully!", resultValue["message"]);
        }

        [Fact]
        public async Task EditGroup_UpdatesGroup()
        {
            // Arrange
            var group = new Group { GroupId = 10, GroupName = "Original", ManagerId = 2, EmployeeIds = "2,3" };
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var request = new EditGroupRequest
            {
                GroupId = 10,
                GroupName = "Updated Name",
                ManagerId = 3,
                EmployeeIds = new List<int> { 3, 4 }
            };

            // Act
            var result = await _controller.EditGroup(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updated = await _context.Groups.FindAsync(10);
            Assert.Equal("Updated Name", updated.GroupName);
            Assert.Equal("4", updated.EmployeeIds); // Manager ID should be removed
        }

        [Fact]
        public async Task DeleteGroup_ReturnsBadRequest_WhenGroupUsedInProject()
        {
            // Arrange
            var group = new Group { GroupId = 1, GroupName = "ToDelete", ManagerId = 5 };
            _context.Groups.Add(group);
            _context.Project.Add(new Project
            {
                ProjectName = "Related Project",
                InvolvedGroups = "1",
                ProjectDescription = "This project is related to the group to be deleted."
            });
            await _context.SaveChangesAsync();

            var request = new DeleteGroupRequest { GroupId = 1 };

            // Act
            var result = await _controller.DeleteGroup(request);

            // Assert
            var badResult = Assert.IsType<BadRequestObjectResult>(result); // ✅ First check the action result type
            var value = Assert.IsType<Dictionary<string, string>>(badResult.Value); // ✅ Then extract and assert the value
            Assert.Equal("This group cannot be deleted because it is associated with a project.", value["message"]);
        }

        [Fact]
        public async Task DeleteGroup_Succeeds_WhenNotUsed()
        {
            // Arrange
            var group = new Group { GroupId = 2, GroupName = "Unused" };
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            var request = new DeleteGroupRequest { GroupId = 2 };

            // Act
            var result = await _controller.DeleteGroup(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result); // <== this is the line I referred to
            var value = Assert.IsType<Dictionary<string, string>>(okResult.Value);
            Assert.Equal("Group deleted successfully!", value["message"]);
        }
    }
}