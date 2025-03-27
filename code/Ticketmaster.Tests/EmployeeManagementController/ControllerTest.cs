using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;


public class EmployeeManagementController
{

    //public void EmployeeManagementIntegrationTests(WebApplicationFactory)
    //{
    //    var builder = WebApplication.CreateBuilder(args);
    //    builder.Services.AddDbContext<TicketmasterContext>(options =>
    //        options.UseSqlServer(builder.Configuration.GetConnectionString("TicketmasterContext") ?? throw new InvalidOperationException("Connection string 'TicketmasterContext' not found.")));

    //    // Add services to the container.
    //    builder.Services.AddControllersWithViews();
    //    builder.Services.AddDistributedMemoryCache();
    //    builder.Services.AddSession(options =>
    //    {
    //        options.IdleTimeout = TimeSpan.FromMinutes(30);
    //        options.Cookie.HttpOnly = true;
    //        options.Cookie.IsEssential = true;
    //    });

    //    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    //        .AddCookie(options =>
    //        {
    //            options.LoginPath = "/Login/Index";
    //            options.AccessDeniedPath = "/Home/AccessDenied";
    //        });

    //}

        [Fact]
        public async Task Get_EmployeeManagement()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/EmployeeManagement");
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("EmployeeManagement", responseString);
        }

        [Fact]
        public async Task Get_EmployeeManagement_Create()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/EmployeeManagement/Create");
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Create", responseString);
        }

        [Fact]
        public async Task Post_EmployeeManagement_Create()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.PostAsync("/EmployeeManagement/Create", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Test Employee")
            }));
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Employee", responseString);
        }

        [Fact]
        public async Task Get_EmployeeManagement_Edit()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.GetAsync("/EmployeeManagement/Edit/1");
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Edit", responseString);
        }

        [Fact]
        public async Task Post_EmployeeManagement_Edit()
        {
            // Arrange
            var client = _factory.CreateClient();
            // Act
            var response = await client.PostAsync("/EmployeeManagement/Edit/1", new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "Test Employee Updated")
            }));
            // Assert
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Test Employee Updated", responseString);
        }

//        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
//        var response = await client.PostAsync("/EmployeeManagement/CommitChanges", null);
//        response.EnsureSuccessStatusCode();
//    }
//}
    }