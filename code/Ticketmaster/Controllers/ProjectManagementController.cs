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

        public async Task<IActionResult> Index()
        {
            var groups = await _context.Groups.ToListAsync();
            var projects = await _context.Project.ToListAsync();

            var viewModel = new ProjectManagementViewModel
            {
                Groups = groups,
                Projects = projects
            };

            return View(viewModel);
        }

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
                InvolvedGroups = selectedGroupIds 
            };

            _context.Project.Add(newProject);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Project created successfully!";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> EditProject([FromBody] Project updatedProject)
        {
            var existingProject = await _context.Project.FindAsync(updatedProject.ProjectId);
            if (existingProject == null) return NotFound();

            existingProject.ProjectName = updatedProject.ProjectName;
            existingProject.ProjectDescription = updatedProject.ProjectDescription;

            await _context.SaveChangesAsync();
            return Ok();
        }

    }
}
