using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ticketmaster.Tests.EmployeeManagementController

//public class EmployeeManagementIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    //private readonly WebApplicationFactory<Program> _factory;

    //public EmployeeManagementIntegrationTests(WebApplicationFactory<Program> factory)
    //{
    //    _factory = factory.WithWebHostBuilder(builder =>
    //    {
    //        builder.ConfigureServices(services =>
    //        {
    //            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<TicketmasterContext>));
    //            if (descriptor != null) services.Remove(descriptor);
    //            services.AddDbContext<TicketmasterContext>(options => options.UseInMemoryDatabase("TestDb"));
    //        });
    //    });
    }

//    [Fact]
//    public async Task Index_ReturnsSuccessAndCorrectView()
//    {
//        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
//        var response = await client.GetAsync("/EmployeeManagement");
//        response.EnsureSuccessStatusCode();
//    }

//    [Fact]
//    public async Task StageEmployeeAdd_AddsEmployeeToSession()
//    {
//        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
//        var content = new FormUrlEncodedContent(new[]
//        {
//            new KeyValuePair<string, string>("id", "1"),
//            new KeyValuePair<string, string>("firstName", "John"),
//            new KeyValuePair<string, string>("lastName", "Doe"),
//            new KeyValuePair<string, string>("email", "johndoe@example.com"),
//            new KeyValuePair<string, string>("pword", "password123"),
//            new KeyValuePair<string, string>("phoneNum", "1234567890"),
//            new KeyValuePair<string, string>("eRole", "Employee")
//        });
//        var response = await client.PostAsync("/EmployeeManagement/StageEmployeeAdd", content);
//        response.EnsureSuccessStatusCode();
//    }

//    [Fact]
//    public async Task CommitChanges_SavesEmployeeToDatabase()
//    {
//        using var scope = _factory.Services.CreateScope();
//        var context = scope.ServiceProvider.GetRequiredService<TicketmasterContext>();

//        context.Employee.Add(new Employee
//        {
//            Id = 1,
//            FirstName = "John",
//            LastName = "Doe",
//            Email = "johndoe@example.com",
//            Pword = "password123",
//            PhoneNum = "1234567890",
//            ERole = "Employee"
//        });
//        await context.SaveChangesAsync();

//        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
//        var response = await client.PostAsync("/EmployeeManagement/CommitChanges", null);
//        response.EnsureSuccessStatusCode();
//    }
//}
