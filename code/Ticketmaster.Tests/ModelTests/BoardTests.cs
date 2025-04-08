using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class BoardTests
{
    private Project _project = new Project
    {
        ProjectId = 1,
        ProjectName = "Main Project",
        ProjectDescription = "This is the main project for all tasks."
    };

    [Fact]
    public void Can_Create_Board_With_Valid_Data()
    {
        // Arrange
        var board = new Board
        {
            Title = "Not Started",
            ParentProject = _project,
            Position = 1
        };
        // Act & Assert
        Assert.Equal(1, board.Position);
        Assert.Equal("Not Started", board.Title);
        //Assert.Equal("This is the main board for all tasks.", board.Description);
    }
}