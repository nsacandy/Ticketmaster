using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ticketmaster.Data;
using Microsoft.AspNetCore.Authentication;
using Ticketmaster.Models;

namespace Ticketmaster.Controllers
{
    public class LoginController : Controller
    {
        private readonly TicketmasterContext _context;

        public LoginController(TicketmasterContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == email && e.Pword == password);
            if (employee == null || !VerifyPassword(employee, password))
            {
                TempData["Error"] = "Invalid credentials.";
                return RedirectToAction("Index");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Email),
                new Claim("Email", employee.Email)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home"); // Redirect to homepage
        }

        private bool VerifyPassword(Employee employee, string password)
        {
            return employee.Pword == password;
        }
    }
}
