using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Ticketmaster.Controllers
{

    /// <summary>
    /// Controller responsible for managing employee groups within the system,
    /// including creation, editing, and deletion of groups.
    /// </summary>
    /// <remarks>
    /// Groups are associated with managers and employees. Only users with roles
    /// "admin" or "standard" are authorized to access these endpoints.
    /// </remarks>
    /// <author>Nate Schaab</author>
    /// <email>nschaab@my.westga.edu</email>
    [Authorize(Roles = "admin,standard")]
    public class GroupManagementController : Controller
    {
        private readonly TicketmasterContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupManagementController"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public GroupManagementController(TicketmasterContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Loads the group management view with a list of employees and their group assignments.
        /// </summary>
        /// <returns>The group management view.</returns>
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employee.ToListAsync();
            var groups = await _context.Groups
                .Include(g => g.Manager)
                .ThenInclude(m => m.Employee)
                .ToListAsync();

            // Pass logged-in user's ID and role to the view
            ViewBag.LoggedInUserId = User.FindFirst("Id")?.Value;
            ViewBag.IsAdmin = User.IsInRole("admin");

            var viewModel = new GroupManagementViewModel
            {
                Employees = employees,
                Groups = groups
            };

            return View(viewModel);
        }

        /// <summary>
        /// Creates a new group with the specified name, manager, and list of employees.
        /// </summary>
        /// <param name="request">The group creation request containing group name, manager ID, and employee IDs.</param>
        /// <returns>A success message or a BadRequest result if validation fails.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
        {
            if (string.IsNullOrEmpty(request.GroupName) || request.ManagerId == 0 || request.EmployeeIds == null || !request.EmployeeIds.Any())
            {
                return BadRequest(new { message = "Group name, manager, and at least one employee are required." });
            }

            var employeeIds = request.EmployeeIds.Distinct().ToList();
            employeeIds.Remove(request.ManagerId);

            if (!await _context.Manager.AnyAsync(m => m.ManagerId == request.ManagerId))
            {
                _context.Manager.Add(new Manager { ManagerId = request.ManagerId });
                await _context.SaveChangesAsync();
            }

            var newGroup = new Group
            {
                GroupName = request.GroupName,
                ManagerId = request.ManagerId,
                EmployeeIds = string.Join(",", employeeIds)
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            return Ok(new Dictionary<String, String>() { {"message", "Group created successfully!" }});
        }

        /// <summary>
        /// Edits the specified group with updated information including name, manager, and employee list.
        /// </summary>
        /// <param name="request">The request containing updated group details.</param>
        /// <returns>A success message or a NotFound result if the group does not exist.</returns>
        [HttpPost]
        public async Task<IActionResult> EditGroup([FromBody] EditGroupRequest request)
        {
            var group = await _context.Groups.FindAsync(request.GroupId);
            if (group == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(request.GroupName) && request.GroupName != group.GroupName)
            {
                group.GroupName = request.GroupName;
            }

            if (request.ManagerId != 0 && request.ManagerId != group.ManagerId)
            {
                var managerExists = await _context.Manager.AnyAsync(m => m.ManagerId == request.ManagerId);
                if (!managerExists)
                {
                    _context.Manager.Add(new Manager { ManagerId = request.ManagerId });
                    await _context.SaveChangesAsync();
                }
                group.ManagerId = request.ManagerId;
            }

            if (request.EmployeeIds != null)
            {
                request.EmployeeIds.Remove(request.ManagerId);
                group.EmployeeIds = string.Join(",", request.EmployeeIds);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Group updated successfully!" });
        }

        /// <summary>
        /// Deletes a group by ID, unless the group is currently used in a project.
        /// </summary>
        /// <param name="request">The deletion request containing the group ID.</param>
        /// <returns>
        /// A success message if the group is deleted, or a BadRequest if the group is in use by a project.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            var group = await _context.Groups.FindAsync(request.GroupId);
            if (group == null)
            {
                return NotFound(new Dictionary<String, String>(){{ "message" , "Group not found." }});
            }

            var groupIdStr = request.GroupId.ToString();

            var projects = await _context.Project.ToListAsync();

            bool isUsedInProject = projects.Any(p =>
                !string.IsNullOrEmpty(p.InvolvedGroups) &&
                p.InvolvedGroups.Split(',').Contains(groupIdStr));

            if (isUsedInProject)
            {
                return BadRequest(new Dictionary<String, String>(){{ "message", "This group cannot be deleted because it is associated with a project." }});
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(new Dictionary<String, String>(){ {"message" , "Group deleted successfully!" }});
        }
    }

    public class CreateGroupRequest
    {
        public string GroupName { get; set; }
        public int ManagerId { get; set; }
        public List<int> EmployeeIds { get; set; }
    }

    public class EditGroupRequest
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public int ManagerId { get; set; }
        public List<int> EmployeeIds { get; set; }
    }
    public class DeleteGroupRequest
    {
        public int GroupId { get; set; }
    }
}