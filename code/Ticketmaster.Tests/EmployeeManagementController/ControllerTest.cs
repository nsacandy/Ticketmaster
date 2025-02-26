using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ticketmaster.Data;
using Ticketmaster.Models;

namespace Ticketmaster.Tests.EmployeeManagementController
{
    public class ControllerTest
    {
       /* private TicketmasterContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<TicketmasterContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            var context = new TicketmasterContext(options);
            context.Database.EnsureCreated();
            return context;
        }
        [Fact]
        public async Task Index_ReturnsViewWithEmployees()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            context.Employee.AddRange(new List<Employee>
            {
                new Employee { Id = 1, FirstName = "Alice", LastName = "Smith", Email = "alice@example.com", PhoneNum = "1234567890" },
                new Employee { Id = 2, FirstName = "Bob", LastName = "Jones", Email = "bob@example.com", PhoneNum = "0987654321" }
            });
            context.SaveChanges();

            var controller = new Controllers.EmployeeManagementController(context);

            // Act
            var result = await controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<EmployeeManagementViewModel>(viewResult.Model);
            Assert.Equal(2, model.Employees.Count());
        }*/


    }
}
