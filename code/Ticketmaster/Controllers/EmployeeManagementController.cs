using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using Microsoft.AspNetCore.Http;
using Ticketmaster.Views.EmployeeManagement;

namespace Ticketmaster.Controllers
{
    public class EmployeeManagementController : Controller
    {
        private readonly TicketmasterContext _context;

        /*
         *Creates a new instance of the EmployeeManagementController class
         * Initializes the database as the context
         */
        public EmployeeManagementController(TicketmasterContext context)
        {
            _context = context;
        }

        public class EmployeeChange
        {
            public string Action { get; set; }  // "Add", "Edit", "Delete"
            public Employee Employee { get; set; }
        }

        // GET: EmployeeManagement
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employee.ToListAsync();
            var stagedChanges = HttpContext.Session.GetObjectFromJson<List<Employee>>("StagedChanges") ?? new List<Employee>();

            var viewModel = new EmployeeManagementViewModel
            {
                Employees = employees,
                StagedChanges = stagedChanges
            };

            return View(viewModel);
        }

        // GET: EmployeeManagement/Details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: EmployeeManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeManagement/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNum")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: EmployeeManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        // POST: EmployeeManagement/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNum")] Employee employee)
        {
            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        // GET: EmployeeManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: EmployeeManagement/Delete/5
/*        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }*/

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.Id == id);
        }



        // Apply changes to the database in a transaction
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
                    {
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
                            if (employee != null)
                            {
                                _context.Employee.Remove(employee);
                            }
                        }
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
        public IActionResult StageEmployeeChange(Employee employee)
        {
            var stagedChanges = HttpContext.Session.GetObjectFromJson<List<Employee>>("StagedChanges") ?? new List<Employee>();

            if (!stagedChanges.Any(e => e.Id == employee.Id))
            {
                stagedChanges.Add(employee);
                HttpContext.Session.SetObjectAsJson("StagedChanges", stagedChanges);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ReviewChanges()
        {
            var stagedChanges = HttpContext.Session.GetObjectFromJson<List<EmployeeChange>>("StagedChanges") ?? new List<EmployeeChange>();
            return View(stagedChanges);  // Create a new Razor view to display staged changes
        }


        [HttpPost]
        public IActionResult RevertEmployeeChange(int id)
        {
            var stagedChanges = HttpContext.Session.GetObjectFromJson<List<Employee>>("StagedChanges") ?? new List<Employee>();

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
    }
}
