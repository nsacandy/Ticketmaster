using Ticketmaster.Models;

namespace Ticketmaster.Tests.ModelTests;

public class TaskItemTests
{
    private readonly Stage _stage = new Stage
    {
        StageTitle = "To Do",
        Position = 0,
        ParentBoardId = 1
    };

    [Fact]
    public void Can_Create_TaskItem_With_Valid_Data()
    {
        var task = new TaskItem
        {
            Title = "Fix login bug",
            Description = "Error occurs when user enters wrong password.",
            Stage = _stage,
            StageId = 1,
            IsComplete = false
        };

        Assert.Equal("Fix login bug", task.Title);
        Assert.Equal("Error occurs when user enters wrong password.", task.Description);
        Assert.Equal(1, task.StageId);
        Assert.Equal(_stage, task.Stage);
        Assert.False(task.IsComplete);
        Assert.True((DateTime.UtcNow - task.CreatedAt).TotalSeconds < 5); // basic time sanity check
    }

    [Fact]
    public void Task_Title_Can_Be_Changed()
    {
        var task = new TaskItem
        {
            Title = "Initial Title"
        };

        task.Title = "Updated Title";

        Assert.Equal("Updated Title", task.Title);
    }

    [Fact]
    public void Task_Description_Defaults_To_Null()
    {
        var task = new TaskItem
        {
            Title = "Test Task"
        };

        Assert.Null(task.Description);
    }

    [Fact]
    public void Can_Associate_Task_With_Stage()
    {
        var task = new TaskItem
        {
            Title = "Sync API",
            Stage = _stage,
            StageId = 1
        };

        Assert.NotNull(task.Stage);
        Assert.Equal("To Do", task.Stage.StageTitle);
    }

    [Fact]
    public void Can_Set_StageId_Without_Stage_Object()
    {
        var task = new TaskItem
        {
            Title = "Deploy to production",
            StageId = 2
        };

        Assert.Equal(2, task.StageId);
        Assert.Null(task.Stage);
    }

    [Fact]
    public void IsComplete_Defaults_To_False()
    {
        var task = new TaskItem
        {
            Title = "Check default complete status"
        };

        Assert.False(task.IsComplete);
    }

    [Fact]
    public void CreatedAt_Defaults_To_CurrentUtcTime()
    {
        var task = new TaskItem
        {
            Title = "Timestamp test"
        };

        var difference = DateTime.UtcNow - task.CreatedAt;
        Assert.True(difference.TotalSeconds < 5); // Just sanity check, time range
    }
}