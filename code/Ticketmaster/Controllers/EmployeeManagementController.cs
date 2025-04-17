using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;

namespace Ticketmaster.Controllers;


/// <summary>
/// Manages employee records, including staging adds, edits, and deletes
/// before committing changes to the database. Provides administrative access
/// to employee data for editing and auditing.
/// </summary>
/// <author>Nicolas Sacandy</author>
/// <email>nsacand2@my.westga.edu</email>
[Authorize(Roles = "admin")]
public class EmployeeManagementController : Controller
{
    private readonly TicketmasterContext _context;
    private EmployeeManagementViewModel viewModel;
    public bool DisableTransactionsForTesting { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmployeeManagementController"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    public EmployeeManagementController(TicketmasterContext context)
    {
        _context = context;
        viewModel = new EmployeeManagementViewModel();
    }


    /// <summary>
    /// Displays the Employee Management page with the current employees and any staged changes.
    /// </summary>
    /// <returns>The employee management view populated with employee data and staged changes.</returns>
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
    /// Stages the deletion of an employee by storing it in session state.
    /// Prevents deletion if the employee is part of a group or is a manager.
    /// </summary>
    /// <param name="employee">The employee to be marked for deletion.</param>
    /// <returns>A redirection to the index page with updated session state.</returns>
    public async Task<IActionResult> StageEmployeeDelete(Employee employee)
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ??
                            new List<EmployeeChange>();

        var selectedEmployee = await _context.Employee.FindAsync(employee.Id);

        var groupUsage = await _context.Groups.ToListAsync();
        bool isInGroup = groupUsage.Any(g =>
            (g.EmployeeIds?.Split(',') ?? Array.Empty<string>()).Contains(employee.Id.ToString()) ||
            g.ManagerId == employee.Id);

        if (isInGroup)
        {
            TempData["Error"] = "This employee cannot be deleted because they are part of a group.";
            return RedirectToAction(nameof(Index));
        }

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
    /// Stages the addition of a new employee based on provided form inputs.
    /// The employee is stored temporarily in session until committed.
    /// </summary>
    /// <param name="id">The unique employee ID.</param>
    /// <param name="firstName">Employee's first name.</param>
    /// <param name="lastName">Employee's last name.</param>
    /// <param name="email">Employee's email address.</param>
    /// <param name="pword">Employee's plaintext password.</param>
    /// <param name="phoneNum">Employee's phone number.</param>
    /// <param name="eRole">Employee's role (e.g., "admin", "standard").</param>
    /// <returns>A redirection to the index view with session-staged data.</returns>
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
    /// Stages an edit for the provided employee object. If a new password is given,
    /// it is hashed before staging.
    /// </summary>
    /// <param name="employee">The employee to stage for editing.</param>
    /// <returns>Redirection to the index view after staging the change.</returns>
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


    /// <summary>
    /// Commits all staged changes (adds, edits, and deletes) in a database transaction.
    /// All actions are removed from session state after a successful commit.
    /// </summary>
    /// <returns>Redirects to index view with success or error feedback via TempData.</returns>
    [HttpPost]
    public IActionResult CommitChanges()
    {
        var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges");

        if (stagedChanges == null || !stagedChanges.Any())
        {
            TempData["Message"] = "No changes to commit.";
            return RedirectToAction(nameof(Index));
        }

        if (!DisableTransactionsForTesting)
        {
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
        }
        else
        {


            try
            {
                foreach (var change in stagedChanges)
                {
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
                }

                _context.SaveChanges();
                HttpContext.Session.Remove("StagedChanges");

                TempData["Message"] = "All changes committed successfully!";

            }
            catch
            {
                TempData["Error"] = "Error committing changes.";
            }


        }

        return RedirectToAction(nameof(Index));
    }


    /// <summary>
    /// Removes a staged employee change based on the employee ID.
    /// </summary>
    /// <param name="id">The employee ID of the change to remove.</param>
    /// <returns>Redirects to the index page.</returns>
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


    /// <summary>
    /// Discards all staged changes by clearing them from session state.
    /// </summary>
    /// <returns>Redirects to the index view with a message that changes were discarded.</returns>
    [HttpPost]
    public IActionResult DiscardChanges()
    {
        HttpContext.Session.Remove("StagedChanges");
        TempData["Message"] = "All staged changes discarded.";
        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Gets the current view model for employee management.
    /// </summary>
    public EmployeeManagementViewModel GetEmployeeManagementViewModel => this.viewModel;

    /// <summary>
    /// Represents a pending employee change staged for commit.
    /// </summary>
    public class EmployeeChange
    {
        /// <summary>
        /// Type of action: "Add", "Edit", or "Delete".
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// The employee data associated with the change.
        /// </summary>
        public Employee Employee { get; set; }
    }

    /// <summary>
    /// Checks if an employee exists in the database by ID.
    /// </summary>
    /// <param name="id">The employee ID to check.</param>
    /// <returns>True if the employee exists, false otherwise.</returns>
    private bool EmployeeExists(int id)
    {
        return _context.Employee.Any(e => e.Id == id);
    }

}