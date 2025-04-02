using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class ProjectTests
{
    [Fact]
    public void Can_Create_Project_With_Valid_Data()
    {
        // Arrange
        var project = new Project
        {
            ProjectId = 10,
            ProjectName = "New Ticketing System",
            ProjectDescription = "Redesigning the internal support ticket flow",
            InvolvedGroups = "1,2,3",
            ProjectLeadId = 5
        };

        // Act & Assert
        Assert.Equal(10, project.ProjectId);
        Assert.Equal("New Ticketing System", project.ProjectName);
        Assert.Equal("Redesigning the internal support ticket flow", project.ProjectDescription);
        Assert.Equal("1,2,3", project.InvolvedGroups);
        Assert.Equal(5, project.ProjectLeadId);
    }

    [Fact]
    public void ProjectName_Should_Have_Required_And_StringLength_Attributes()
    {
        // Arrange
        var property = typeof(Project).GetProperty(nameof(Project.ProjectName));
        var requiredAttr = property.GetCustomAttribute<RequiredAttribute>();
        var stringLengthAttr = property.GetCustomAttribute<StringLengthAttribute>();

        // Assert
        Assert.NotNull(requiredAttr);
        Assert.NotNull(stringLengthAttr);
        Assert.Equal(100, stringLengthAttr.MaximumLength);
    }

    [Fact]
    public void ProjectId_Should_Have_Key_Attribute()
    {
        // Arrange
        var property = typeof(Project).GetProperty(nameof(Project.ProjectId));
        var keyAttr = property.GetCustomAttribute<KeyAttribute>();

        // Assert
        Assert.NotNull(keyAttr);
    }

    [Fact]
    public void Project_Class_Should_Have_Table_Attribute()
    {
        // Arrange
        var tableAttr = typeof(Project).GetCustomAttribute<TableAttribute>();

        // Assert
        Assert.NotNull(tableAttr);
        Assert.Equal("Project", tableAttr.Name);
    }

    [Fact]
    public void ProjectDescription_And_InvolvedGroups_Should_Be_Optional()
    {
        // Arrange
        var project = new Project
        {
            ProjectId = 11,
            ProjectName = "Empty Optional Fields",
            ProjectDescription = null,
            InvolvedGroups = null,
            ProjectLeadId = 8
        };

        // Assert
        Assert.Null(project.ProjectDescription);
        Assert.Null(project.InvolvedGroups);
    }
}