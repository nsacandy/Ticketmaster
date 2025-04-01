using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;

namespace Ticketmaster.Controllers;


/// <summary>
/// Controls the employeeManagement  page.
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
 [Authorize(Roles = "admin")]
public class EmployeeManagementController : Controller
{
    private readonly TicketmasterContext _context;
    private EmployeeManagementViewModel viewModel;


    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeManagementController"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public EmployeeManagementController(TicketmasterContext context)
    {
        _context = context;
        viewModel = new EmployeeManagementViewModel();
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
    /// <param name="employee">The employee to delete</param>
    /// <returns>Page with employee removed</returns>

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
    public async Task<IActionResult> StageEmployeeAdd(int id, string firstName, string lastName, string email,
        string pword, string phoneNum, string eRole)
    {
        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pword) || string.IsNullOrEmpty(phoneNum) ||
            string.IsNullOrEmpty(eRole))
        {
            TempData["Error"] = "Please fill in all required fields.";
            return RedirectToAction(nameof(Index));
        }

        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                            new List<EmployeeChange>();

        var newEmployee = new Employee
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Pword = pword,
            PhoneNum = phoneNum,
            ERole = eRole
        };

        // newEmployee.Pword = this._passwordHasher.HashPassword(newEmployee, newEmployee.Pword);
        if (!stagedChanges.Any(employeeChange => employeeChange.Employee.Id == id))
        {
            var change = new EmployeeChange
            {
                Action = "Add",
                Employee = newEmployee
            };
            stagedChanges.Add(change);
            HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
        }

        TempData["Success"] = "Employee staged successfully!";
        return RedirectToAction("Index");
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
            if (employee.Pword.IsNullOrEmpty())
            {
                employee.Pword = (await _context.Employee.FindAsync(employee.Id)).Pword;
            }
            else
            {
                employee.Pword = EmployeePasswordHasher.HashPassword(employee.Pword);
            }

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
                        change.Employee.Pword = EmployeePasswordHasher.HashPassword(change.Employee.Pword);
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
        try
        {
            var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                                new List<EmployeeChange>();

            stagedChanges.RemoveAll(e => e.Employee.Id == id);

            HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
        }
        catch (NullReferenceException n)
        {
            Console.WriteLine(n.StackTrace);
        }



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

    private bool EmployeeExists(int id)
    {
        return _context.Employee.Any(e => e.Id == id);
    }
}