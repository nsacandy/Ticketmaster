using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Ticketmaster.Controllers;
using Ticketmaster.Data;
using Microsoft.AspNetCore.Mvc.Routing;
using Ticketmaster.Models;
using Ticketmaster.Utilities;
using static Ticketmaster.Controllers.EmployeeManagementController;

namespace Ticketmaster.Tests.ControllerTests;
public class EmployeeManagementControllerTests
{
    private readonly EmployeeManagementController _controller;
    private readonly TicketmasterContext _context;
    private readonly DefaultHttpContext _httpContext;
    private readonly Dictionary<string, byte[]> _sessionStorage;

    public EmployeeManagementControllerTests()
    {
        _httpContext = new DefaultHttpContext();
        _sessionStorage = new Dictionary<string, byte[]>();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDataProtection();
        services.AddSingleton<ITempDataDictionaryFactory, TempDataDictionaryFactory>();
        services.AddSingleton<ITempDataProvider, CookieTempDataProvider>();
        services.AddControllersWithViews();

        var provider = services.BuildServiceProvider();
        _httpContext.RequestServices = provider;

        _httpContext.Session = new DummySession(_sessionStorage);

        var tempDataFactory = provider.GetRequiredService<ITempDataDictionaryFactory>();
        var tempData = tempDataFactory.GetTempData(_httpContext);

        var options = new DbContextOptionsBuilder<TicketmasterContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new TicketmasterContext(options);
        _context.Database.EnsureCreated();

        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock.Setup(u => u.Action(It.IsAny<UrlActionContext>()))
            .Returns("/EmployeeManagement/Index");

        _controller = new EmployeeManagementController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            },
            TempData = tempData,
            Url = urlHelperMock.Object
        };
        _controller.DisableTransactionsForTesting = true;

    }

    [Fact]
    public async Task StageEmployeeAdd_AddsEmployeeToSessionAndRedirects()
    {
        var result = await _controller.StageEmployeeAdd(101, "Bob", "Builder", "bob@build.com", "builder", "555-1234", "standard");

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.True(_controller.TempData.ContainsKey("Success"));

        var stagedChanges = _httpContext.Session.GetObjectFromJson<List<EmployeeManagementController.EmployeeChange>>("StagedChanges");
        Assert.Single(stagedChanges);
        Assert.Equal("Bob", stagedChanges[0].Employee.FirstName);
    }

    [Fact]
    public async Task StageEmployeeDelete_StagedSuccessfully()
    {
        var employee = new Employee { Id = 10, FirstName = "Alice", Email = "alice@example.com", Pword = "pass", ERole = "admin", LastName = "wonderland", PhoneNum = "999999999"};
        _context.Employee.Add(employee);
        await _context.SaveChangesAsync();

        var result = await _controller.StageEmployeeDelete(employee);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var stagedChanges = _httpContext.Session.GetObjectFromJson<List<EmployeeManagementController.EmployeeChange>>("StagedChanges");
        Assert.Single(stagedChanges);
        Assert.Equal("Delete", stagedChanges[0].Action);
        Assert.Equal("Alice", stagedChanges[0].Employee.FirstName);
    }

    [Fact]
    public async Task StageEmployeeEdit_StagesEditSuccessfully()
    {
        var employee = new Employee { Id = 20, FirstName = "John", Email = "john@example.com", PhoneNum = "123456789",Pword = "secret", ERole = "admin", LastName = "Doe"};
        _context.Employee.Add(employee);
        await _context.SaveChangesAsync();

        employee.FirstName = "John Updated";
        var result = await _controller.StageEmployeeEdit(employee);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var stagedChanges = _httpContext.Session.GetObjectFromJson<List<EmployeeManagementController.EmployeeChange>>("StagedChanges");
        Assert.Single(stagedChanges);
        Assert.Equal("Edit", stagedChanges[0].Action);
        Assert.Equal("John Updated", stagedChanges[0].Employee.FirstName);
    }

    [Fact]
    public void DiscardChanges_RemovesStagedChanges()
    {
        _httpContext.Session.SetObjectAsJson("StagedChanges", new List<EmployeeManagementController.EmployeeChange>
        {
            new() { Action = "Add", Employee = new Employee { Id = 5, FirstName = "Test" } }
        });

        var result = _controller.DiscardChanges();

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var resultSession = _httpContext.Session.GetObjectFromJson<List<EmployeeManagementController.EmployeeChange>>("StagedChanges");
        Assert.Null(resultSession);
    }

    [Fact]
    public void RevertEmployeeChange_RemovesSpecificChange()
    {
        _httpContext.Session.SetObjectAsJson("StagedChanges", new List<EmployeeManagementController.EmployeeChange>
        {
            new() { Action = "Edit", Employee = new Employee { Id = 123, FirstName = "Foo" } },
            new() { Action = "Delete", Employee = new Employee { Id = 456, FirstName = "Bar" } }
        });

        var result = _controller.RevertEmployeeChange(123);

        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);

        var remaining = _httpContext.Session.GetObjectFromJson<List<EmployeeManagementController.EmployeeChange>>("StagedChanges");
        Assert.Single(remaining);
        Assert.Equal(456, remaining[0].Employee.Id);
    }

    [Fact]
    public async Task IndexCreatesNewViewModel()
    {
        var result = await _controller.Index();

        Assert.NotNull(_controller.GetEmployeeManagementViewModel);
    }
    [Fact]
    public void CommitChanges_CommitsStagedAddChange_ReturnsRedirectWithSuccessMessage()
    {
        // Arrange
        var newEmployee = new Employee
        {
            Id = 0,
            FirstName = "Test",
            LastName = "User",
            Email = "test@example.com",
            PhoneNum = "1234567890",
            Pword = "plaintext",  // Should get hashed
            ERole = "standard"
        };

        var change = new EmployeeChange
        {
            Action = "Add",
            Employee = newEmployee
        };

        var changesList = new List<EmployeeChange> { change };
        _httpContext.Session.SetObjectAsJson("StagedChanges", changesList);

        // Act
        var result = _controller.CommitChanges();

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("All changes committed successfully!", _controller.TempData["Message"]);

        var addedEmployee = _context.Employee.FirstOrDefault(e => e.Email == "test@example.com");
        Assert.NotNull(addedEmployee);
        Assert.NotEqual("plaintext", addedEmployee!.Pword); // Ensure password was hashed

        // Session should be cleared
        bool stillHasKey = _httpContext.Session.TryGetValue("StagedChanges", out var _);
        Assert.False(stillHasKey);
    }

    [Fact]
    public async Task StageEmployeeDelete_EmployeeInGroup_ReturnsRedirectWithError()
    {

        var _controller = CreateController();
        // Arrange
        var employee = new Employee
        {
            Id = 10,
            FirstName = "Groupie",
            LastName = "McTest",
            Email = "groupie@test.com",
            Pword = "pass",
            PhoneNum = "1234567890",
            ERole = "standard"
        };

        var group = new Group
        {
            GroupId = 1,
            GroupName = "TestGroup",
            EmployeeIds = "10,11",
            ManagerId = 42 // not the same employee
        };

        _context.Employee.Add(employee);
        _context.Groups.Add(group);
        await _context.SaveChangesAsync();


        // Act
        var result = await _controller.StageEmployeeDelete(employee);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirect.ActionName);
        Assert.Equal("This employee cannot be deleted because they are part of a group.", _controller.TempData["Error"]);
    }

    private EmployeeManagementController CreateController()
    {
        var controller = new EmployeeManagementController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = _httpContext,
                ActionDescriptor = new ControllerActionDescriptor()
            },
            TempData = _controller.TempData,
            Url = _controller.Url,
            DisableTransactionsForTesting = true
        };

        return controller;
    }


    private class DummySession : ISession
    {
        private readonly Dictionary<string, byte[]> _storage;

        public DummySession(Dictionary<string, byte[]> storage) => _storage = storage;

        public IEnumerable<string> Keys => _storage.Keys;
        public string Id => Guid.NewGuid().ToString();
        public bool IsAvailable => true;
        public void Clear() => _storage.Clear();
        public Task CommitAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task LoadAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
        public void Remove(string key) => _storage.Remove(key);
        public void Set(string key, byte[] value) => _storage[key] = value;
        public bool TryGetValue(string key, out byte[] value) => _storage.TryGetValue(key, out value);
    }
}
