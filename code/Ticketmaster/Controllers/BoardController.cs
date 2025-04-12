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
                .Include(b => b.Stages)
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
                .Include(b => b.Stages)
                .Include(b => b.ParentProject)
                .ToListAsync();

            var projectName = boards.FirstOrDefault()?.ParentProject?.ProjectName ?? "Project";
            ViewData["Title"] = $"Board - {projectName}";

            return View("Index", boards);
        }


        private bool BoardExists(int id)
        {
            return _context.Board.Any(e => e.BoardId == id);
        }
    }
}
