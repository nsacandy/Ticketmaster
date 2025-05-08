using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;

namespace Ticketmaster.Controllers
{
    /// <summary>
    /// Controller for managing boards, stages, and tasks in the Ticketmaster application.
    /// </summary>
    public class BoardController : Controller
    {
        private readonly TicketmasterContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BoardController"/> class.
        /// </summary>
        /// <param name="context">Database context for accessing board-related data.</param>
        public BoardController(TicketmasterContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the default board view with all related stages and tasks.
        /// </summary>
        /// <returns>The board view with loaded data.</returns>
        public async Task<IActionResult> Index()
        {
            var board = await _context.Board
                .Include(b => b.Stages)
                .ThenInclude(s => s.Tasks)
                .Include(b => b.ParentProject)
                .FirstOrDefaultAsync();
            ViewData["Title"] = "All Boards";
            return View(board);
        }

        /// <summary>
        /// Displays the board associated with a specific project, including related stages, tasks, and assigned employees.
        /// </summary>
        /// <param name="projectId">The ID of the project whose board should be displayed.</param>
        /// <returns>The board view filtered by project.</returns>
        [HttpGet("Board/Project/{projectId}")]
        public async Task<IActionResult> ProjectBoard(int projectId)
        {
            var board = await _context.Board
                .Include(b => b.Stages).ThenInclude(s => s.Tasks)
                .Include(b => b.ParentProject)
                .FirstOrDefaultAsync(b => b.ParentProjectId == projectId);

            ViewData["Title"] = $"Board - {board?.ParentProject?.ProjectName ?? "Project"}";

            var project = await _context.Project.FindAsync(projectId);
            var employeeList = new List<Employee>();

            if (project != null && !string.IsNullOrEmpty(project.InvolvedGroups))
            {
                var groupIds = project.InvolvedGroups.Split(',').Select(int.Parse).ToList();
                var groups = await _context.Groups.ToListAsync();

                foreach (var groupId in groupIds)
                {
                    var group = groups.FirstOrDefault(g => g.GroupId == groupId);
                    if (group != null && !string.IsNullOrWhiteSpace(group.EmployeeIds))
                    {
                        var ids = group.EmployeeIds.Split(',').Select(int.Parse).ToList();
                        var employees = await _context.Employee.Where(e => ids.Contains(e.Id)).ToListAsync();
                        employeeList.AddRange(employees);
                    }

                    if (group?.ManagerId != null && !employeeList.Any(e => e.Id == group.ManagerId))
                    {
                        var manager = await _context.Employee.FindAsync(group.ManagerId);
                        if (manager != null)
                            employeeList.Add(manager);
                    }
                }
            }

            ViewBag.AssignedEmployees = employeeList.Distinct().ToList();

            var userId = User.FindFirst("Id")?.Value;
            bool isAdmin = User.IsInRole("admin");
            bool isProjectLead = userId != null && project?.ProjectLeadId.ToString() == userId;

            ViewBag.IsAdmin = isAdmin;
            ViewBag.IsProjectLead = isProjectLead;

            return View("Index", board);
        }

        /// <summary>
        /// Checks if a board with the given ID exists.
        /// </summary>
        /// <param name="id">Board ID to check.</param>
        /// <returns>True if the board exists; otherwise, false.</returns>
        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.BoardId == id);
        }

        /// <summary>
        /// Displays the form to create a new stage for a given project.
        /// </summary>
        /// <param name="projectId">The ID of the parent project.</param>
        /// <returns>The stage creation view.</returns>
        [HttpGet]
        public IActionResult Create(int projectId)
        {
            var stage = new Stage { ParentBoardId = projectId };
            return View(stage);
        }

        /// <summary>
        /// Handles the submission of a new stage.
        /// </summary>
        /// <param name="stage">The stage to add.</param>
        /// <returns>Redirects to the project board view if successful; otherwise, returns the form with validation errors.</returns>
        [HttpPost]
        public async Task<IActionResult> Create(Stage stage)
        {
            if (!ModelState.IsValid)
            {
                return View(stage);
            }

            // Retrieve all stages for the same board, ordered by Position descending
            var existingStages = await _context.Stage
                .Where(s => s.ParentBoardId == stage.ParentBoardId)
                .OrderByDescending(s => s.Position)
                .ToListAsync();

            // Perform cascading position shift to prevent conflicts
            foreach (var existingStage in existingStages)
            {
                if (existingStage.Position >= stage.Position)
                {
                    existingStage.Position += 1;
                    _context.Stage.Update(existingStage);
                }
            }

            _context.Stage.Add(stage);
            await _context.SaveChangesAsync();
            return RedirectToAction("ProjectBoard", "Board", new { projectId = stage.ParentBoardId });
        }

        /// <summary>
        /// Displays the form to edit a stage.
        /// </summary>
        /// <param name="id">The ID of the stage to edit.</param>
        /// <returns>The edit form for the selected stage, or a 404 if not found.</returns>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var stage = await _context.Stage.FindAsync(id);
            return stage == null ? NotFound() : View(stage);
        }

        /// <summary>
        /// Handles the submission of edited stage details.
        /// </summary>
        /// <param name="stage">The updated stage object.</param>
        /// <returns>Redirects to the board view if successful; otherwise, redisplays the form.</returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Stage stage)
        {
            if (ModelState.IsValid)
            {
                _context.Stage.Update(stage);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProjectBoard", "Board", new { projectId = stage.ParentBoardId });
            }
            return View(stage);
        }

        /// <summary>
        /// Deletes a stage by ID.
        /// </summary>
        /// <param name="id">The ID of the stage to delete.</param>
        /// <returns>Redirects to the associated project board.</returns>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var stage = await _context.Stage.FindAsync(id);
            if (stage != null)
            {
                _context.Stage.Remove(stage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("ProjectBoard", new { projectId = stage?.ParentBoardId });
        }

        /// <summary>
        /// Adds a new task to the specified stage.
        /// </summary>
        /// <param name="stageId">The ID of the stage to add the task to.</param>
        /// <param name="title">The title of the task.</param>
        /// <param name="description">The optional description of the task.</param>
        /// <returns>Redirects to the project board view.</returns>
        [HttpPost]
        public async Task<IActionResult> AddTask(int stageId, string title, string? description)
        {
            var stage = await _context.Stage.Include(s => s.Tasks).FirstOrDefaultAsync(s => s.StageId == stageId);
            if (stage == null || string.IsNullOrWhiteSpace(title))
            {
                return BadRequest("Invalid stage or task title.");
            }

            var task = new TaskItem
            {
                Title = title,
                Description = description,
                StageId = stageId,
                CreatedAt = DateTime.UtcNow,
                IsComplete = false
            };

            _context.TaskItem.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProjectBoard", new { projectId = stage.ParentBoardId });
        }

        /// <summary>
        /// Deletes a task by its ID.
        /// </summary>
        /// <param name="taskId">The ID of the task to delete.</param>
        /// <returns>Redirects to the associated project board.</returns>
        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var task = await _context.TaskItem.Include(t => t.Stage).FirstOrDefaultAsync(t => t.TaskItemId == taskId);
            if (task == null)
            {
                return NotFound();
            }

            int? projectId = task.Stage?.ParentBoardId;

            _context.TaskItem.Remove(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProjectBoard", new { projectId });
        }

        /// <summary>
        /// Moves a task to a new stage.
        /// </summary>
        /// <param name="taskId">The ID of the task to move.</param>
        /// <param name="targetStageId">The ID of the new stage.</param>
        /// <returns>Redirects to the updated project board.</returns>
        [HttpPost]
        public async Task<IActionResult> MoveTask(int taskId, int targetStageId)
        {
            var task = await _context.TaskItem.Include(t => t.Stage).FirstOrDefaultAsync(t => t.TaskItemId == taskId);
            if (task == null)
            {
                return NotFound();
            }

            var targetStage = await _context.Stage.Include(s => s.ParentBoard).FirstOrDefaultAsync(s => s.StageId == targetStageId);
            if (targetStage == null)
            {
                return BadRequest("Target stage not found.");
            }

            task.StageId = targetStageId;
            await _context.SaveChangesAsync();

            return RedirectToAction("ProjectBoard", new { projectId = targetStage.ParentBoardId });
        }

        /// <summary>
        /// Assigns a task to an employee using a JSON request payload.
        /// </summary>
        /// <param name="request">Contains the task ID and the employee ID to assign.</param>
        /// <returns>A success message if the assignment is valid.</returns>
        [HttpPost]
        public async Task<IActionResult> AssignTask([FromBody] AssignTaskRequest request)
        {
            if (request == null || request.TaskId == 0 || request.EmployeeId == 0)
                return BadRequest(new { message = "Invalid data" });

            var task = await _context.TaskItem
                .Include(t => t.Stage)
                .ThenInclude(s => s.ParentBoard)
                .ThenInclude(b => b.ParentProject)
                .FirstOrDefaultAsync(t => t.TaskItemId == request.TaskId);

            if (task == null)
                return NotFound(new { message = "Task not found" });

            var userId = User.FindFirst("Id")?.Value;
            var isAdmin = User.IsInRole("admin");
            var isProjectLead = userId != null && task.Stage.ParentBoard.ParentProject.ProjectLeadId.ToString() == userId;

            // ⛔ Restriction: Non-admin, non-leads can only assign to themselves
            if (!isAdmin && !isProjectLead && request.EmployeeId.ToString() != userId)
            {
                return StatusCode(403, new { message = "Access denied. You can only assign tasks to yourself." });
            }

            task.AssignedTo = request.EmployeeId;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Task assigned successfully!" });
        }

    }

    /// <summary>
    /// Represents a request to assign a task to an employee.
    /// </summary>
    public class AssignTaskRequest
    {
        /// <summary>
        /// The ID of the task to assign.
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// The ID of the employee to assign the task to.
        /// </summary>
        public int EmployeeId { get; set; }
    }
}
