using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class BoardTaskTests
{
    private readonly Board _board = new Board
    {
        Title = "To Do",
        Position = 0,
        ParentProjectId = 1
    };

    [Fact]
    public void Can_Create_BoardTask_With_Valid_Data()
    {
        var task = new Stage
        {
            TaskTitle = "Fix login bug",
            TaskDescription = "Error occurs when user enters wrong password.",
            ParentBoard = _board,
            ParentBoardId = 1
        };

        Assert.Equal("Fix login bug", task.TaskTitle);
        Assert.Equal("Error occurs when user enters wrong password.", task.TaskDescription);
        Assert.Equal(1, task.ParentBoardId);
        Assert.Equal(_board, task.ParentBoard);
    }

    [Fact]
    public void Task_Title_Can_Be_Changed()
    {
        var task = new Stage
        {
            TaskTitle = "Initial Title"
        };

        task.TaskTitle = "Updated Title";

        Assert.Equal("Updated Title", task.TaskTitle);
    }

    [Fact]
    public void Task_Description_Defaults_To_Null()
    {
        var task = new Stage
        {
            TaskTitle = "Test Task"
        };

        Assert.Null(task.TaskDescription);
    }

    [Fact]
    public void Can_Associate_Task_With_Board()
    {
        var task = new Stage
        {
            TaskTitle = "Sync API",
            ParentBoard = _board,
            ParentBoardId = 1
        };

        Assert.NotNull(task.ParentBoard);
        Assert.Equal("To Do", task.ParentBoard.Title);
    }

    [Fact]
    public void Can_Set_ParentBoardId_Without_Board_Object()
    {
        var task = new Stage
        {
            TaskTitle = "Deploy to production",
            ParentBoardId = 2
        };

        Assert.Equal(2, task.ParentBoardId);
        Assert.Null(task.ParentBoard);
    }
}