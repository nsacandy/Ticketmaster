using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class BoardTests
{
    //private Project _project = new Project
    //{
    //    ProjectId = 1,
    //    ProjectName = "Main Project",
    //    ProjectDescription = "This is the main project for all tasks."
    //};


    //[Fact]
    //public void Can_Create_Board_With_Valid_Data()
    //{
    //    // Arrange
    //    var board = new Board
    //    {
    //        Title = "Not Started",
    //        ParentProject = _project,
    //        ParentProjectId = _project.ProjectId,
    //        Position = 1
    //    };

    //    // Act & Assert
    //    Assert.Equal("Not Started", board.Title);
    //    Assert.Equal(1, board.Position);
    //    Assert.Equal(_project, board.ParentProject);
    //    Assert.Equal(1, board.ParentProjectId);
    //}

    //[Fact]
    //public void New_Board_Has_Null_Tasks_By_Default()
    //{
    //    var board = new Board();
    //    Assert.Null(board.Tasks);
    //}

    //[Fact]
    //public void Board_Can_Associate_With_Project()
    //{
    //    var board = new Board
    //    {
    //        Title = "QA",
    //        Position = 2,
    //        ParentProject = _project,
    //        ParentProjectId = _project.ProjectId
    //    };

    //    Assert.NotNull(board.ParentProject);
    //    Assert.Equal("Main Project", board.ParentProject.ProjectName);
    //}

    //[Fact]
    //public void Can_Modify_Board_Title_And_Position()
    //{
    //    var board = new Board
    //    {
    //        Title = "In Progress",
    //        Position = 2
    //    };

    //    board.Title = "Review";
    //    board.Position = 3;

    //    Assert.Equal("Review", board.Title);
    //    Assert.Equal(3, board.Position);
    //}

    //[Fact]
    //public void Setting_ParentProjectId_Does_Not_Affect_Navigation_Property()
    //{
    //    var board = new Board
    //    {
    //        Title = "Test",
    //        Position = 1,
    //        ParentProjectId = 99 // doesn't update ParentProject automatically
    //    };

    //    Assert.Null(board.ParentProject);
    //    Assert.Equal(99, board.ParentProjectId);
    //}

}