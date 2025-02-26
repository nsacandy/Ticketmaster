using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;

namespace Ticketmaster.Controllers
{
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
            var groups = await _context.Groups.Include(g => g.Manager).ToListAsync();

            var viewModel = new GroupManagementViewModel
            {
                Employees = employees,
                Groups = groups
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup(string groupName, int managerId, string selectedEmployeeIds)
        {
            if (string.IsNullOrEmpty(groupName) || managerId == 0 || string.IsNullOrEmpty(selectedEmployeeIds))
            {
                TempData["Error"] = "Please provide a group name, select a manager, and select at least one employee.";
                return RedirectToAction(nameof(Index));
            }

            var employeeIds = selectedEmployeeIds.Split(',').Select(int.Parse).ToList();
            employeeIds.Remove(managerId);

            var managerExists = await _context.Set<Manager>().AnyAsync(m => m.ManagerId == managerId);

            if (!managerExists)
            {
                var newManager = new Manager { ManagerId = managerId };
                _context.Set<Manager>().Add(newManager);
                await _context.SaveChangesAsync();
            }

            var newGroup = new Group
            {
                GroupName = groupName,
                ManagerId = managerId,
                EmployeeIds = string.Join(",", employeeIds)
            };

            _context.Groups.Add(newGroup);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Group created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> ChangeManager([FromBody] ChangeManagerRequest request)
        {
            var group = await _context.Groups.FindAsync(request.GroupId);
            if (group == null)
            {
                return NotFound();
            }

            int previousManagerId = group.ManagerId ?? 0;

            var managerExists = await _context.Set<Manager>().AnyAsync(m => m.ManagerId == request.NewManagerId);
            if (!managerExists)
            {
                var newManager = new Manager { ManagerId = request.NewManagerId };
                _context.Set<Manager>().Add(newManager);
                await _context.SaveChangesAsync();
            }

            var employeeIds = string.IsNullOrEmpty(group.EmployeeIds)
                ? new List<int>()
                : group.EmployeeIds.Split(',').Select(int.Parse).ToList();

            employeeIds.Remove(request.NewManagerId);

            if (!employeeIds.Contains(previousManagerId))
            {
                employeeIds.Add(previousManagerId);
            }

            group.ManagerId = request.NewManagerId;
            group.EmployeeIds = string.Join(",", employeeIds);

            await _context.SaveChangesAsync();

            return Ok(new { newEmployeeIds = group.EmployeeIds });
        }

        public class ChangeManagerRequest
        {
            public int GroupId { get; set; }
            public int NewManagerId { get; set; }
        }

    }
}
