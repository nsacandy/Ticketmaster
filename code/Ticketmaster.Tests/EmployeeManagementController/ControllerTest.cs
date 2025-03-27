using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ticketmaster.Data;
using Ticketmaster.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Ticketmaster.Tests.EmployeeManagementController
{

    public class EmployeeManagementIntegrationTests
    {
        private readonly WebApplicationFactory<Program> _factory;

        public EmployeeManagementIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var server = new WebApplicationFactory<>()
        }

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

        [Fact]
        public async Task Get_EmployeeManagement_Delete()
        {
            // Arrange
        }
    }
}