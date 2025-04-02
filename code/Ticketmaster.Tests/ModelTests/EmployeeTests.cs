using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class EmployeeTests
{
    [Fact]
    public void Can_Create_Employee_With_Valid_Data()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "Alice",
            LastName = "Johnson",
            Email = "alice.johnson@example.com",
            Pword = "SuperSecure123!",
            PhoneNum = "555-1234",
            ERole = "Technician"
        };

        // Act & Assert
        Assert.Equal(1, employee.Id);
        Assert.Equal("Alice", employee.FirstName);
        Assert.Equal("Johnson", employee.LastName);
        Assert.Equal("alice.johnson@example.com", employee.Email);
        Assert.Equal("SuperSecure123!", employee.Pword);
        Assert.Equal("555-1234", employee.PhoneNum);
        Assert.Equal("Technician", employee.ERole);
    }

    [Theory]
    [InlineData("Id", "Id")]
    [InlineData("FirstName", "FirstName")]
    [InlineData("LastName", "LastName")]
    [InlineData("Email", "Email")]
    [InlineData("Pword", "Pword")]
    [InlineData("PhoneNum", "PhoneNum")]
    [InlineData("ERole", "ERole")]
    public void Property_Should_Have_Correct_Column_Attribute(string propertyName, string expectedColumnName)
    {
        // Arrange
        var property = typeof(Employee).GetProperty(propertyName);
        var columnAttr = property.GetCustomAttribute<ColumnAttribute>();

        // Assert
        Assert.NotNull(columnAttr);
        Assert.Equal(expectedColumnName, columnAttr.Name);
    }

    [Fact]
    public void All_Properties_Should_Allow_Get_And_Set()
    {
        // Arrange
        var employee = new Employee();

        // Act
        employee.FirstName = "Bob";
        employee.LastName = "Smith";
        employee.Email = "bob.smith@example.com";
        employee.Pword = "Password!";
        employee.PhoneNum = "555-5678";
        employee.ERole = "Manager";

        // Assert
        Assert.Equal("Bob", employee.FirstName);
        Assert.Equal("Smith", employee.LastName);
        Assert.Equal("bob.smith@example.com", employee.Email);
        Assert.Equal("Password!", employee.Pword);
        Assert.Equal("555-5678", employee.PhoneNum);
        Assert.Equal("Manager", employee.ERole);
    }
}