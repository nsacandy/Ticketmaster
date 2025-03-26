using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Ticketmaster.Controllers
{
    [Authorize(Roles = "admin,standard")]

    public class ProjectManagementController : Controller
    {
        private readonly TicketmasterContext _context;

        public ProjectManagementController(TicketmasterContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups.Include(g => g.Manager).ThenInclude(m => m.Employee).ToListAsync();
            var projects = await _context.Project.ToListAsync();

            var viewModel = new ProjectManagementViewModel
            {
                Groups = groups,
                Project = projects
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
        {
            try
            {
                Console.WriteLine($"Received CreateProject request: " +
                    $"Name={request?.ProjectName}, " +
                    $"Description={request?.ProjectDescription}, " +
                    $"LeadId={request?.ProjectLeadId}, " +
                    $"Groups={(request?.InvolvedGroups != null ? string.Join(",", request.InvolvedGroups) : "NULL")}");

                Console.WriteLine($"Received request - Name: {request?.ProjectName}, Desc: {request?.ProjectDescription}, LeadId: {request?.ProjectLeadId}, Groups: {string.Join(",", request?.InvolvedGroups ?? new List<int>())}");

                if (request == null)
                {
                    return BadRequest(new { message = "Request data is null." });
                }

                if (string.IsNullOrEmpty(request.ProjectName) || string.IsNullOrEmpty(request.ProjectDescription) || request.ProjectLeadId == 0 || request.InvolvedGroups == null || !request.InvolvedGroups.Any())
                {
                    return BadRequest(new { message = "All fields are required." });
                }

                var leadExists = await _context.Employee.AnyAsync(e => e.Id == request.ProjectLeadId);
                if (!leadExists)
                {
                    return BadRequest(new { message = "Invalid Project Lead." });
                }

                var newProject = new Project
                {
                    ProjectName = request.ProjectName,
                    ProjectDescription = request.ProjectDescription,
                    ProjectLeadId = request.ProjectLeadId,
                    InvolvedGroups = string.Join(",", request.InvolvedGroups)
                };

                _context.Project.Add(newProject);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Project created successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}\nSTACK TRACE: {ex.StackTrace}");
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditProject([FromBody] EditProjectRequest request)
        {
            try
            {
                var project = await _context.Project.FindAsync(request.ProjectId);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found." });
                }

                if (!string.IsNullOrEmpty(request.ProjectName) && request.ProjectName != project.ProjectName)
                {
                    project.ProjectName = request.ProjectName;
                }

                if (!string.IsNullOrEmpty(request.ProjectDescription) && request.ProjectDescription != project.ProjectDescription)
                {
                    project.ProjectDescription = request.ProjectDescription;
                }

                if (request.ProjectLeadId != 0 && request.ProjectLeadId != project.ProjectLeadId)
                {
                    var leadExists = await _context.Employee.AnyAsync(e => e.Id == request.ProjectLeadId);
                    if (!leadExists)
                    {
                        return BadRequest(new { message = "Invalid Project Lead." });
                    }
                    project.ProjectLeadId = request.ProjectLeadId;
                }

                if (request.InvolvedGroups != null && request.InvolvedGroups.Count > 0)
                {
                    project.InvolvedGroups = string.Join(",", request.InvolvedGroups);
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Project updated successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal server error occurred.", details = ex.Message });
            }
        }
    }

    public class CreateProjectRequest
    {
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int ProjectLeadId { get; set; }
        public List<int> InvolvedGroups { get; set; }
    }

    public class EditProjectRequest
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }
        public int ProjectLeadId { get; set; }
        public List<int> InvolvedGroups { get; set; }
    }

}
