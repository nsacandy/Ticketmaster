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
    /// <summary>
    /// Controller responsible for managing projects within the application,
    /// including creating, editing, and deleting projects.
    /// </summary>
    /// <remarks>
    /// Only users with the "admin" or "standard" roles are authorized to access these endpoints.
    /// </remarks>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    [Authorize(Roles = "admin,standard")]
    public class ProjectManagementController : Controller
    {
        private readonly TicketmasterContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectManagementController"/> class.
        /// </summary>
        /// <param name="context">The database context used for project operations.</param>
        public ProjectManagementController(TicketmasterContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Loads the project management view with all current groups and projects.
        /// </summary>
        /// <returns>The view displaying projects and their associated groups.</returns>
        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups.Include(g => g.Manager).ThenInclude(m => m.Employee).ToListAsync();
            var projects = await _context.Project.ToListAsync();

            var viewModel = new ProjectManagementViewModel
            {
                Groups = groups,
                Project = projects
            };

            ViewBag.LoggedInUserId = User.FindFirst("Id")?.Value;
            ViewBag.IsAdmin = User.IsInRole("admin");

            return View(viewModel);
        }

        /// <summary>
        /// Creates a new project with the specified details.
        /// </summary>
        /// <param name="request">The project creation request containing name, description, lead, and groups.</param>
        /// <returns>
        /// A success response if the project is created, or a <see cref="BadRequestResult"/> if validation fails.
        /// Returns a 500 status code if an unexpected error occurs.
        /// </returns>
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


        /// <summary>
        /// Edits the details of an existing project.
        /// </summary>
        /// <param name="request">The project update request including project ID and new values.</param>
        /// <returns>
        /// A success response if the update is applied, or a <see cref="NotFoundResult"/> if the project does not exist.
        /// Returns a 500 status code if an unexpected error occurs.
        /// </returns>
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

        /// <summary>
        /// Deletes the specified project.
        /// </summary>
        /// <param name="request">The deletion request containing the project ID.</param>
        /// <returns>
        /// A success message if deleted, or a <see cref="NotFoundResult"/> if the project does not exist.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DeleteProject([FromBody] DeleteProjectRequest request)
        {
            var project = await _context.Project.FindAsync(request.ProjectId);
            if (project == null)
            {
                return NotFound(new { message = "Project not found." });
            }

            _context.Project.Remove(project);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Project deleted successfully!" });
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

    public class DeleteProjectRequest
    {
        public int ProjectId { get; set; }
    }

}