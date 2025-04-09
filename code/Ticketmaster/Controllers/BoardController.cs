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
            var boards = await _context.Board
                .Include(b => b.Tasks)
                .Include(b => b.ParentProject)
                .ToListAsync();

            ViewData["Title"] = "All Boards";
            return View(boards);
        }

        [HttpGet("Board/Project/{projectId}")]
        public async Task<IActionResult> ProjectBoard(int projectId)
        {
            var boards = await _context.Board
                .Where(b => b.ParentProjectId == projectId)
                .Include(b => b.Tasks)
                .Include(b => b.ParentProject)
                .ToListAsync();

            var projectName = boards.FirstOrDefault()?.ParentProject?.ProjectName ?? "Project";
            ViewData["Title"] = $"Board - {projectName}";

            return View("Index", boards);
        }

        // GET: Boards/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .Include(b => b.ParentProject)
                .FirstOrDefaultAsync(m => m.Title == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // GET: Boards/Create
        public IActionResult Create()
        {
            ViewData["ParentProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName");
            return View();
        }

        // POST: Boards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,ParentProjectId,Position")] Board board)
        {
            if (ModelState.IsValid)
            {
                _context.Add(board);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ParentProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", board.ParentProjectId);
            return View(board);
        }

        // GET: Boards/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board.FindAsync(id);
            if (board == null)
            {
                return NotFound();
            }
            ViewData["ParentProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", board.ParentProjectId);
            return View(board);
        }

        // POST: Boards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Title,ParentProjectId,Position")] Board board)
        {
            if (id != board.Title)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(board);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoardExists(board.Title))
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
            ViewData["ParentProjectId"] = new SelectList(_context.Project, "ProjectId", "ProjectName", board.ParentProjectId);
            return View(board);
        }

        // GET: Boards/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var board = await _context.Board
                .Include(b => b.ParentProject)
                .FirstOrDefaultAsync(m => m.Title == id);
            if (board == null)
            {
                return NotFound();
            }

            return View(board);
        }

        // POST: Boards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var board = await _context.Board.FindAsync(id);
            if (board != null)
            {
                _context.Board.Remove(board);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoardExists(string id)
        {
            return _context.Board.Any(e => e.Title == id);
        }
    }
}
