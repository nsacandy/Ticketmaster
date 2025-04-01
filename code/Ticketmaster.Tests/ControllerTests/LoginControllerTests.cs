using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ticketmaster.Controllers;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using Xunit;

namespace Ticketmaster.Tests.ControllerTests;

public class LoginControllerTests : IDisposable
{
    private readonly LoginController _loginController;
    private readonly TicketmasterContext _context;

    public LoginControllerTests()
    {
        var httpContext = new DefaultHttpContext();
        var authServiceMock = new Mock<IAuthenticationService>();

        var services = new ServiceCollection();
        services.AddSingleton(authServiceMock.Object);
        services.AddLogging();
        services.AddDataProtection();
        services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>(); // ✅ Fix: Add a valid TempData provider
        services.AddControllersWithViews(); // ✅ Registers TempDataSerializer and other dependencies
        // Build service provider and attach to HttpContext
        var serviceProvider = services.BuildServiceProvider();
        httpContext.RequestServices = serviceProvider;

        var tempDataFactory = serviceProvider.GetRequiredService<ITempDataDictionaryFactory>();
        var tempData = tempDataFactory.GetTempData(httpContext);

        var options = new DbContextOptionsBuilder<TicketmasterContext>()
            .UseInMemoryDatabase(databaseName: "Ticketmaster")
            .Options;

        _context = new TicketmasterContext(options);
        _context.Database.EnsureCreated();

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("/Home/Index"); // fake URL or just non-null string

        _loginController = new LoginController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            },
            TempData = tempData,
            Url = urlHelperMock.Object
        };

        _loginController.TempData = tempDataFactory.GetTempData(httpContext);
        
        _context.Employee.RemoveRange(_context.Employee);
        _context.SaveChanges();


        var password = EmployeePasswordHasher.HashPassword("hank");
        _context.Employee.Add(new Employee
        {
            Email = "hank@hill.com",
            Pword = password,
            FirstName = "Hank",
            LastName = "Hill",
            ERole = "admin",
            PhoneNum = "123-456-7890",
            Id = 2
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task SuccessfulLoginGoesToHomePage()
    {
        var result = await _loginController.Login("hank@hill.com", "hank");

        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    [Fact]
    public async Task TestLogoutReturnsToIndex()
    {
        await _loginController.Login("hank@hill.com", "hank");

        var result = await _loginController.Logout();
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
    }

    [Fact]
    public async Task TestGetIndex()
    {
        var result = _loginController.Index();
        var redirectResult = Assert.IsType<ViewResult>(result);
    }

    [Fact]
    public async Task FailLoginStaysOnLogin()
    {
        var result = await _loginController.Login("hank@hill.com", "Spank");

        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal("Index", viewResult.ViewName);
        Assert.True(_loginController.ViewData.ContainsKey("LoginError"));
        Assert.Equal("Invalid email or password.", _loginController.ViewData["LoginError"]);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Clean up DB after test run
        _context.Dispose();
    }

}