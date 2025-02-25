using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;

namespace Ticketmaster.Controllers;

public class EmployeeManagementController : Controller
{
    private readonly TicketmasterContext _context;
    private EmployeeManagementViewModel viewModel;

    /*
     *Creates a new instance of the EmployeeManagementController class
     * Initializes the database as the context
     */
    public EmployeeManagementController(TicketmasterContext context)
    {
        _context = context;
        var viewModel = new EmployeeManagementViewModel();
    }

    // GET: EmployeeManagement
    public async Task<IActionResult> Index()
    {
        var employees = await _context.Employee.ToListAsync();
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                            new List<EmployeeChange>();

        viewModel = new EmployeeManagementViewModel
        {
            Employees = employees,
            StagedChanges = stagedChanges
        };

        return View(viewModel);
    }


    /// <summary>
    /// Stages the employee delete.
    /// </summary>
    /// <param name="employee">The employee.</param>
    /// <returns></returns>
    public async Task<IActionResult> StageEmployeeDelete(Employee employee)
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                            new List<EmployeeChange>();

        var selectedEmployee = await _context.Employee.FindAsync(employee.Id);
        if (!stagedChanges.Any(employeeChange => employeeChange.Employee.Id == employee.Id))
        {
            var change = new EmployeeChange
            {
                Action = "Delete",
                Employee = selectedEmployee
            };
            stagedChanges.Add(change);
            HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
        }

        return RedirectToAction(nameof(Index));
    }


    /// <summary>
    /// Stages the employee add.
    /// </summary>
    /// <param name="employee">The employee.</param>
    /// <returns></returns>
    public async Task<IActionResult> StageEmployeeAdd([Bind("Id,FirstName,LastName,Email,PhoneNum")] Employee employee)
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                            new List<EmployeeChange>();

        var newEmployee = employee;
        if (!stagedChanges.Any(employeeChange => employeeChange.Employee.Id == employee.Id))
        {
            var change = new EmployeeChange
            {
                Action = "Add",
                Employee = newEmployee
            };
            stagedChanges.Add(change);
            HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
        }

        return RedirectToAction(nameof(Index));
    }


    private bool EmployeeExists(int id)
    {
        return _context.Employee.Any(e => e.Id == id);
    }

    /// <summary>
    /// Stages the employee edit.
    /// </summary>
    /// <param name="employee">The employee.</param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> StageEmployeeEdit(Employee employee)
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                            new List<EmployeeChange>();

        if (!stagedChanges.Any(employeeChange => employeeChange.Employee.Id == employee.Id))
        {
            var change = new EmployeeChange
            {
                Action = "Edit",
                Employee = employee
            };
            stagedChanges.Add(change);
            HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
        }

        HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
        return RedirectToAction(nameof(Index));
    }


    // Apply changes to the database in a transaction
    /// <summary>
    /// Commits the changes.
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public IActionResult CommitChanges()
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges");

        if (stagedChanges == null || !stagedChanges.Any())
        {
            TempData["Message"] = "No changes to commit.";
            return RedirectToAction(nameof(Index));
        }

        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                foreach (var change in stagedChanges)
                    if (change.Action == "Add")
                    {
                        _context.Employee.Add(change.Employee);
                    }
                    else if (change.Action == "Edit")
                    {
                        _context.Employee.Update(change.Employee);
                    }
                    else if (change.Action == "Delete")
                    {
                        var employee = _context.Employee.Find(change.Employee.Id);
                        if (employee != null) _context.Employee.Remove(employee);
                    }

                _context.SaveChanges();
                transaction.Commit();
                HttpContext.Session.Remove("StagedChanges");

                TempData["Message"] = "All changes committed successfully!";
            }
            catch
            {
                transaction.Rollback();
                TempData["Error"] = "Error committing changes. Transaction rolled back.";
            }
        }

        return RedirectToAction(nameof(Index));
    }
    

    [HttpPost]
    public IActionResult RevertEmployeeChange(int id)
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<Employee>>("StagedChanges") ??
                            new List<Employee>();

        stagedChanges.RemoveAll(e => e.Id == id);

        HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public IActionResult DiscardChanges()
    {
        HttpContext.Session.Remove("StagedChanges");
        TempData["Message"] = "All staged changes discarded.";
        return RedirectToAction(nameof(Index));
    }

    /*
     *
     */
    public class EmployeeChange
    {
        public string Action { get; set; } // "Add", "Edit", "Delete"
        public Employee Employee { get; set; }
    }
}