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
    /// <summary>
    /// Handles user authentication actions such as login and logout.
    /// Allows anonymous access and manages login logic using claims-based identity.
    /// </summary>
    /// <remarks>
    /// On successful login, issues a cookie-based authentication ticket.
    /// If credentials are invalid, displays an error and returns to the login view.
    /// </remarks>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly TicketmasterContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginController"/> class.
        /// </summary>
        /// <param name="context">The application's database context.</param>
        public LoginController(TicketmasterContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        /// <returns>The login view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Handles login requests by validating the provided email and password.
        /// On success, authenticates the user and redirects to the Home page.
        /// On failure, returns to the login page with an error message.
        /// </summary>
        /// <param name="email">The user's email address.</param>
        /// <param name="password">The user's password.</param>
        /// <returns>
        /// A redirect to the Home page on success, or the login view with an error on failure.
        /// </returns>
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.Email == email);
            if (employee == null || !VerifyPassword(password, employee.Pword))
            {
                ViewData["LoginError"] = "Invalid email or password.";
                return View("Index"); // Stay on login page
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, employee.Email),
                new Claim("Email", employee.Email),
                new Claim(ClaimTypes.Role, employee.ERole)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties { IsPersistent = true };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Verifies the user's provided password against the stored hashed password.
        /// </summary>
        /// <param name="password">The plain-text password entered by the user.</param>
        /// <param name="hashedPassword">The hashed password from the database.</param>
        /// <returns><c>true</c> if the password is valid; otherwise, <c>false</c>.</returns>
        private bool VerifyPassword(String password, string hashedPassword)
        {
           
            return EmployeePasswordHasher.VerifyPassword(hashedPassword, password);
        }

        /// <summary>
        /// Signs the current user out and redirects to the login page.
        /// </summary>
        /// <returns>A redirect to the login page.</returns>
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            return RedirectToAction("Index");
        }
    }
}
