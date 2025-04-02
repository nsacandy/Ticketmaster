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

namespace Ticketmaster.Tests.ControllerTests;
public class TestEmployeeController
{
    private readonly EmployeeManagementController _controller;
    private readonly TicketmasterContext _context;
    private readonly DefaultHttpContext _httpContext;
    private readonly Dictionary<string, byte[]> _sessionStorage;

    public TestEmployeeController()
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
