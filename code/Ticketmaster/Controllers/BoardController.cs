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
    }
}
