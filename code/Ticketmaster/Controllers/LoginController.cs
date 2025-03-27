using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Ticketmaster.Data;
using Microsoft.AspNetCore.Authentication;
using Ticketmaster.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ticketmaster.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace Ticketmaster.Controllers
{
    [AllowAnonymous]
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
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null)
            {
               return RedirectToAction("AccessDenied", "Home");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Email),
                new Claim("Email", employee.Email),
                new Claim(ClaimTypes.Role, employee.ERole) // Store the role
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            if (claimsIdentity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            else return RedirectToAction("AccessDenied", "Home");
        }

        private bool VerifyPassword(String password, string hashedPassword)
        {
           
            return EmployeePasswordHasher.VerifyPassword(hashedPassword, password);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index");
        }
    }
}
