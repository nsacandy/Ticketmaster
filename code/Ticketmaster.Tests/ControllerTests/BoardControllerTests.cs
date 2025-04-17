using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Controllers;
using Ticketmaster.Data;
using Ticketmaster.Models;

namespace Ticketmaster.Tests.ControllerTests

{
    public class BoardControllerTests
    {
        private readonly TicketmasterContext _context;
        private readonly BoardController _controller;


        public BoardControllerTests()
        {
            var options = new DbContextOptionsBuilder<TicketmasterContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new TicketmasterContext(options);
            _context.Database.EnsureCreated();

            _controller = new BoardController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewWithBoard()
        {
            // Arrange
            var project = new Project
            {
                ProjectId = 1,
                ProjectName = "Demo Project",
                ProjectDescription = "Test Desc",
                InvolvedGroups = "1,2"
            };

            var board = new Board
            {
                BoardId = 1,
                ParentProjectId = 1
            };

            _context.Project.Add(project); // ✅ Needed for .Include(b => b.ParentProject)
            _context.Board.Add(board);

            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.Index();

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.IsAssignableFrom<Board>(view.Model);
        }


        [Fact]
        public async Task ProjectBoard_ReturnsBoardWithEmployees()
        {
            // Arrange
            var project = new Project
            {
                ProjectId = 1, // <- FIXED: match the ID you’ll query
                ProjectName = "Test",
                InvolvedGroups = "3",
                ProjectDescription = "Test project",
            };

            var employee = new Employee
            {
                Id = 10,
                FirstName = "Test",
                LastName = "Simpson",
                Email = "bart@simpson.com",
                PhoneNum = "555555",
                Pword = "TestPAss",
                ERole = "standard"
            };

            var group = new Group
            {
                GroupId = 3,
                GroupName = "Testname",
                EmployeeIds = "10", // <- FIXED: match the employee ID
                ManagerId = null
            };

            var board = new Board
            {
                BoardId = 2,
                ParentProjectId = 1 // <- FIXED: match the project you’ll query
            };

            _context.Project.Add(project);
            _context.Employee.Add(employee);
            _context.Groups.Add(group);
            _context.Board.Add(board);

            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ProjectBoard(1);

            // Assert
            var view = Assert.IsType<ViewResult>(result);
            Assert.IsType<Board>(view.Model);
            Assert.NotNull(_controller.ViewBag.AssignedEmployees);
        }

        [Fact]
        public async Task Create_Post_ValidStage_RedirectsToProjectBoard()
        {
            var stage = new Stage { StageTitle = "Dev", ParentBoardId = 1 };

            var result = await _controller.Create(stage);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProjectBoard", redirect.ActionName);
            Assert.Equal(1, _context.Stage.Count());
        }

        [Fact]
        public async Task Edit_Post_ValidStage_UpdatesAndRedirects()
        {
            var stage = new Stage { StageTitle = "QA", ParentBoardId = 1 };
            _context.Stage.Add(stage);
            await _context.SaveChangesAsync();

            stage.StageTitle = "Updated QA";

            var result = await _controller.Edit(stage);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProjectBoard", redirect.ActionName);
        }

        [Fact]
        public async Task Delete_RemovesStageAndRedirects()
        {
            var stage = new Stage { StageId = 1, StageTitle = "To Remove", ParentBoardId = 1 };
            _context.Stage.Add(stage);
            await _context.SaveChangesAsync();

            var result = await _controller.Delete(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(_context.Stage);
        }

        [Fact]
        public async Task AddTask_ValidStage_AddsTaskAndRedirects()
        {
            var stage = new Stage { StageId = 1, StageTitle = "To Do", ParentBoardId = 1 };
            _context.Stage.Add(stage);
            await _context.SaveChangesAsync();

            var result = await _controller.AddTask(1, "New Task", "Some Description");

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Single(_context.TaskItem);
        }

        [Fact]
        public async Task DeleteTask_RemovesTaskAndRedirects()
        {
            var stage = new Stage { StageId = 1, ParentBoardId = 1, StageTitle = "Test title"};
            var task = new TaskItem { TaskItemId = 1, Title = "Test", StageId = 1, Stage = stage };

            _context.Stage.Add(stage);
            _context.TaskItem.Add(task);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteTask(1);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Empty(_context.TaskItem);
        }

        [Fact]
        public async Task MoveTask_ChangesStageIdAndRedirects()
        {
            var board = new Board { BoardId = 1, ParentProjectId = 1 };

            var stage1 = new Stage { StageId = 1, StageTitle = "Start", ParentBoardId = 1 };
            var stage2 = new Stage { StageId = 2, StageTitle = "Done", ParentBoardId = 1 };

            var task = new TaskItem
            {
                TaskItemId = 1,
                Title = "Test Title",
                StageId = 1
            };

            _context.Board.Add(board); // 🔥 Needed so .Include(s => s.ParentBoard) works
            _context.Stage.AddRange(stage1, stage2);
            _context.TaskItem.Add(task);

            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.MoveTask(1, 2);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ProjectBoard", redirect.ActionName);
            Assert.Equal(2, _context.TaskItem.First().StageId);
        }

        [Fact]
        public async Task AssignTask_ValidRequest_AssignsEmployee()
        {
            var task = new TaskItem
            {
                TaskItemId = 1,
                Title = "Test Task"
            };
            var employee = new Employee { Id = 42 , Email = "Lisa@simpson.com", FirstName = "Lisa", LastName = "Simpson", ERole = "admin", Pword = "lisa", PhoneNum = "5555555"};

            _context.TaskItem.Add(task);
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            var request = new AssignTaskRequest { TaskId = 1, EmployeeId = 42 };
            var result = await _controller.AssignTask(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(42, _context.TaskItem.First().AssignedTo);
        }
    }
}
