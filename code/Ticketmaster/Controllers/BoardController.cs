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
    public class BoardController : Controller
    {
        private readonly TicketmasterContext _context;

        public BoardController(TicketmasterContext context)
        {
            _context = context;
        }

        // GET: Boards
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

        [HttpGet("Board/Project/{projectId}")]
        public async Task<IActionResult> ProjectBoard(int projectId)
        {
            var board = await _context.Board
                .Include(b => b.Stages).ThenInclude(s => s.Tasks)
                .Include(b => b.ParentProject)
                .FirstOrDefaultAsync(b => b.ParentProjectId == projectId);

            ViewData["Title"] = $"Board - {board?.ParentProject?.ProjectName ?? "Project"}";
            return View("Index", board);
        }


        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.BoardId == id);
        }

        [HttpGet]
        public IActionResult Create(int projectId)
        {
            var stage = new Stage { ParentBoardId = projectId };
            return View(stage);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Stage stage)
        {
            foreach (var error in ModelState)
            {
                Console.WriteLine($"Key: {error.Key}");
                foreach (var subError in error.Value.Errors)
                {
                    Console.WriteLine($"  Error: {subError.ErrorMessage}");
                }
            }
            if (ModelState.IsValid)
            {
                _context.Stage.Add(stage);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProjectBoard", "Board", new { projectId = stage.ParentBoardId });
            }
            return View(stage);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var stage = await _context.Stage.FindAsync(id);
            return stage == null ? NotFound() : View(stage);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Stage stage)
        {
            if (ModelState.IsValid)
            {
                _context.Stage.Update(stage);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProjectBoard", "Board",new { projectId = stage.ParentBoardId});
            }
            return View(stage);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var stage = await _context.Stage.FindAsync(id);
            if (stage != null)
            {
                _context.Stage.Remove(stage);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Board", new { projectId = stage?.ParentBoardId });
        }
    }
}
