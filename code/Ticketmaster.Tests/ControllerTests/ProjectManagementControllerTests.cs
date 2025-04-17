using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Controllers;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Ticketmaster.Tests.ControllerTests
{
    public class ProjectManagementControllerTests
    {
        private readonly TicketmasterContext _context;
        private readonly ProjectManagementController _controller;

        public ProjectManagementControllerTests()
        {
            var options = new DbContextOptionsBuilder<TicketmasterContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new TicketmasterContext(options);
            _context.Database.EnsureCreated();

            _controller = new ProjectManagementController(_context);
        }

        [Fact]
        public async Task Index_ReturnsViewWithModel()
        {
            var claims = new List<Claim>
            {
                new Claim("Id", "1"),
                new Claim(ClaimTypes.Role, "admin")
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = principal
                }
            };

            var result = await _controller.Index();

            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<ProjectManagementViewModel>(viewResult.Model);
        }


        [Fact]
        public async Task CreateProject_ReturnsOk_WhenValid()
        {
            var lead = new Employee
            {
                FirstName = "Test",
                LastName = "User",
                Email = "test@example.com",
                Pword = "securepass",
                PhoneNum = "1234567890",
                ERole = "standard"
            };

            _context.Employee.Add(lead);
            await _context.SaveChangesAsync();

            var request = new CreateProjectRequest
            {
                ProjectName = "Project X",
                ProjectDescription = "Test Description",
                ProjectLeadId = lead.Id,
                InvolvedGroups = new List<int> { 123 }
            };

            var result = await _controller.CreateProject(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Project created successfully", okResult.Value.ToString());
        }


        [Fact]
        public async Task CreateProject_ReturnsBadRequest_WhenInvalid()
        {
            var result = await _controller.CreateProject(new CreateProjectRequest());
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("All fields are required", badRequest.Value.ToString());
        }

        [Fact]
        public async Task EditProject_UpdatesSuccessfully()
        {
            var options = new DbContextOptionsBuilder<TicketmasterContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new TicketmasterContext(options);

            var employee = new Employee
            {
                Id = 1,
                FirstName = "Lead",
                LastName = "Test",
                Email = "lead@example.com",
                Pword = "pass123",
                PhoneNum = "555-0000",
                ERole = "standard"
            };
            context.Employee.Add(employee);

            var project = new Project
            {
                ProjectId = 1,
                ProjectName = "Old",
                ProjectDescription = "Desc",
                ProjectLeadId = 1,
                InvolvedGroups = "1"
            };
            context.Project.Add(project);

            await context.SaveChangesAsync();

            var controller = new ProjectManagementController(context);

            var update = new EditProjectRequest
            {
                ProjectId = 1,
                ProjectName = "Updated",
                ProjectDescription = "Updated Desc",
                ProjectLeadId = 1,
                InvolvedGroups = new List<int> { 1 }
            };

            var result = await controller.EditProject(update);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Project updated successfully", okResult.Value.ToString());
        }

        [Fact]
        public async Task EditProject_ReturnsNotFound_IfProjectMissing()
        {
            var result = await _controller.EditProject(new EditProjectRequest { ProjectId = 999 });
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Project not found", notFound.Value.ToString());
        }

        [Fact]
        public async Task DeleteProject_DeletesSuccessfully()
        {
            var project = new Project
            {
                ProjectId = 2,
                ProjectName = "ToDelete",
                ProjectDescription = "Desc",
                ProjectLeadId = 1,
                InvolvedGroups = "1"
            };

            _context.Project.Add(project);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteProject(new DeleteProjectRequest { ProjectId = 2 });
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Project deleted successfully", okResult.Value.ToString());
        }


        [Fact]
        public async Task DeleteProject_ReturnsNotFound_IfMissing()
        {
            var result = await _controller.DeleteProject(new DeleteProjectRequest { ProjectId = 999 });
            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains("Project not found", notFound.Value.ToString());
        }
    }
    
    }