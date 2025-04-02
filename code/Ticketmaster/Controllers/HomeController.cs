using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Ticketmaster.Models;

namespace Ticketmaster.Controllers
{
    /// <summary>
    /// Controls the main pages of the application such as Index, Privacy, and error handling.
    /// This controller is accessible to users with "admin" or "standard" roles, 
    /// except for AccessDenied which is open to all.
    /// </summary>
    /// <author>Nicolas Sacandy</author>
    /// <email>nsacand2@my.westga.edu</email>
    [Authorize(Roles = "admin,standard")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="logger">An <see cref="ILogger"/> used for logging diagnostic messages.</param>
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Returns the main landing page of the application.
        /// </summary>
        /// <returns>The Index view.</returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Returns the application's privacy policy page.
        /// </summary>
        /// <returns>The Privacy view.</returns>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Returns the error page with detailed request diagnostics.
        /// This method disables caching to ensure that errors are always fresh.
        /// </summary>
        /// <returns>An error view with the current request's ID.</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Displays the access denied page for unauthorized users.
        /// This action is publicly accessible.
        /// </summary>
        /// <returns>The AccessDenied view.</returns>
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
