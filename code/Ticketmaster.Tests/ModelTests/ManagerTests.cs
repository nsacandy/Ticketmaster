using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class ManagerTests
{
    [Fact]
    public void Can_Create_Manager_With_Valid_Data()
    {
        // Arrange
        var employee = new Employee { Id = 99, FirstName = "Sam" };
        var manager = new Manager
        {
            ManagerId = 99,
            Employee = employee
        };

        // Act & Assert
        Assert.Equal(99, manager.ManagerId);
        Assert.NotNull(manager.Employee);
        Assert.Equal("Sam", manager.Employee.FirstName);
    }

    [Fact]
    public void ManagerId_Should_Have_Key_And_ForeignKey_Attributes()
    {
        // Arrange
        var property = typeof(Manager).GetProperty(nameof(Manager.ManagerId));
        var keyAttr = property.GetCustomAttribute<KeyAttribute>();
        var fkAttr = property.GetCustomAttribute<ForeignKeyAttribute>();

        // Assert
        Assert.NotNull(keyAttr);
        Assert.NotNull(fkAttr);
        Assert.Equal("Employee", fkAttr.Name);
    }

    [Fact]
    public void Employee_Navigation_Can_Be_Null()
    {
        // Arrange
        var manager = new Manager
        {
            ManagerId = 101,
            Employee = null
        };

        // Assert
        Assert.Equal(101, manager.ManagerId);
        Assert.Null(manager.Employee);
    }
}