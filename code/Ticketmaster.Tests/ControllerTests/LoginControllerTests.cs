using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ticketmaster.Controllers;
using Ticketmaster.Data;
using Ticketmaster.Models;
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

        httpContext.RequestServices = new ServiceCollection()
            .AddSingleton(authServiceMock.Object)
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<TicketmasterContext>()
            .UseInMemoryDatabase(databaseName: "Ticketmaster")
            .Options;

        _context = new TicketmasterContext(options);
        _context.Database.EnsureCreated();
        _loginController = new LoginController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            }
        };

        
        _context.Employee.Add(new Employee
        {
            Email = "hank@hill.com",
            Pword = "hill",
            FirstName = "Hank",
            LastName = "Hill",
            ERole = "admin",
            PhoneNum = "123-456-7890",
            Id = 1,
        });
        _context.SaveChanges();

        

    }

    [Fact]
    public async Task SuccessfulLogin()
    {
        // Act
        var result = await _loginController.Login("hank@hill.com", "hank");

        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Home", redirectResult.ControllerName);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Clean up DB after test run
        _context.Dispose();
    }

}