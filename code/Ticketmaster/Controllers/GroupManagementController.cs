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
    [Authorize(Roles = "admin,standard")]

    public class GroupManagementController : Controller
    {
        private readonly TicketmasterContext _context;

        public GroupManagementController(TicketmasterContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employee.ToListAsync();
            var groups = await _context.Groups
                .Include(g => g.Manager)
                .ThenInclude(m => m.Employee)
                .ToListAsync();

            var viewModel = new GroupManagementViewModel
            {
                Employees = employees,
                Groups = groups
            };

            return View(viewModel);
        }

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

            return Ok(new { message = "Group created successfully!" });
        }

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

            if (request.EmployeeIds != null && request.EmployeeIds.Count > 0)
            {
                request.EmployeeIds.Remove(request.ManagerId);
                group.EmployeeIds = string.Join(",", request.EmployeeIds);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Group updated successfully!" });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup([FromBody] DeleteGroupRequest request)
        {
            var group = await _context.Groups.FindAsync(request.GroupId);
            if (group == null)
            {
                return NotFound(new { message = "Group not found." });
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Group deleted successfully!" });
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
