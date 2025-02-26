using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Ticketmaster.Controllers
{
    public class ProjectManagementController : Controller
    {
        private readonly TicketmasterContext _context;

        public ProjectManagementController(TicketmasterContext context)
        {
            _context = context;
        }

        // GET: ProjectManagement
        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups.ToListAsync();
            var projects = await _context.Projects.ToListAsync();

            var viewModel = new ProjectManagementViewModel
            {
                Groups = groups,
                Projects = projects
            };

            return View(viewModel);
        }

        // POST: CreateProject
        [HttpPost]
        public async Task<IActionResult> CreateProject(string projectName, string projectDescription, string selectedGroupIds)
        {
            if (string.IsNullOrEmpty(projectName) || string.IsNullOrEmpty(projectDescription) || string.IsNullOrEmpty(selectedGroupIds))
            {
                TempData["Error"] = "Please provide a project name, description, and select at least one group.";
                return RedirectToAction(nameof(Index));
            }

            var newProject = new Project
            {
                ProjectName = projectName,
                ProjectDescription = projectDescription,
                InvolvedGroups = selectedGroupIds // Store as comma-separated values
            };

            _context.Projects.Add(newProject);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Project created successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
