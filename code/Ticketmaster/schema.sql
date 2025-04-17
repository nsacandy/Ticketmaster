IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
CREATE TABLE [Employee] (
    [Id] int NOT NULL IDENTITY,
    [FirstName] nvarchar(max) NOT NULL,
    [LastName] nvarchar(max) NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Pword] nvarchar(max) NOT NULL,
    [PhoneNum] nvarchar(max) NOT NULL,
    [ERole] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Employee] PRIMARY KEY ([Id])
);

CREATE TABLE [Project] (
    [ProjectId] int NOT NULL IDENTITY,
    [ProjectName] nvarchar(100) NOT NULL,
    [ProjectDescription] nvarchar(max) NOT NULL,
    [InvolvedGroups] nvarchar(max) NOT NULL,
    [ProjectLeadId] int NOT NULL,
    CONSTRAINT [PK_Project] PRIMARY KEY ([ProjectId])
);

CREATE TABLE [Manager] (
    [ManagerId] int NOT NULL,
    CONSTRAINT [PK_Manager] PRIMARY KEY ([ManagerId]),
    CONSTRAINT [FK_Manager_Employee_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [Employee] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Board] (
    [BoardId] int NOT NULL IDENTITY,
    [ParentProjectId] int NOT NULL,
    CONSTRAINT [PK_Board] PRIMARY KEY ([BoardId]),
    CONSTRAINT [FK_Board_Project_ParentProjectId] FOREIGN KEY ([ParentProjectId]) REFERENCES [Project] ([ProjectId]) ON DELETE CASCADE
);

CREATE TABLE [Groups] (
    [GroupId] int NOT NULL IDENTITY,
    [GroupName] nvarchar(100) NOT NULL,
    [ManagerId] int NULL,
    [EmployeeIds] nvarchar(max) NULL,
    CONSTRAINT [PK_Groups] PRIMARY KEY ([GroupId]),
    CONSTRAINT [FK_Groups_Manager_ManagerId] FOREIGN KEY ([ManagerId]) REFERENCES [Manager] ([ManagerId])
);

CREATE TABLE [Stage] (
    [StageId] int NOT NULL IDENTITY,
    [StageTitle] nvarchar(100) NOT NULL,
    [ParentBoardId] int NOT NULL,
    [Position] int NOT NULL,
    CONSTRAINT [PK_Stage] PRIMARY KEY ([StageId]),
    CONSTRAINT [FK_Stage_Board_ParentBoardId] FOREIGN KEY ([ParentBoardId]) REFERENCES [Board] ([BoardId]) ON DELETE CASCADE
);

CREATE TABLE [TaskItem] (
    [TaskItemId] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [Description] nvarchar(max) NULL,
    [StageId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [IsComplete] bit NOT NULL,
    CONSTRAINT [PK_TaskItem] PRIMARY KEY ([TaskItemId]),
    CONSTRAINT [FK_TaskItem_Stage_StageId] FOREIGN KEY ([StageId]) REFERENCES [Stage] ([StageId]) ON DELETE CASCADE
);

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ERole', N'Email', N'FirstName', N'LastName', N'PhoneNum', N'Pword') AND [object_id] = OBJECT_ID(N'[Employee]'))
    SET IDENTITY_INSERT [Employee] ON;
INSERT INTO [Employee] ([Id], [ERole], [Email], [FirstName], [LastName], [PhoneNum], [Pword])
VALUES (1, N'admin', N'admin@ticketmaster.com', N'Admin', N'User', N'123-456-7890', N'AQAAAAIAAYagAAAAENjHEpOgSW4plLWvF2f76tsPNQ+jRE1GURqVsw1gHBb/8IQJ7MqDMOhbpHAztrmN6g==');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'ERole', N'Email', N'FirstName', N'LastName', N'PhoneNum', N'Pword') AND [object_id] = OBJECT_ID(N'[Employee]'))
    SET IDENTITY_INSERT [Employee] OFF;

CREATE INDEX [IX_Board_ParentProjectId] ON [Board] ([ParentProjectId]);

CREATE INDEX [IX_Groups_ManagerId] ON [Groups] ([ManagerId]);

CREATE UNIQUE INDEX [IX_Stage_ParentBoardId_StageTitle] ON [Stage] ([ParentBoardId], [StageTitle]);

CREATE INDEX [IX_TaskItem_StageId] ON [TaskItem] ([StageId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250412232753_BoardStageSetup', N'9.0.4');

UPDATE [Employee] SET [Pword] = N'AQAAAAIAAYagAAAAEAhyn1Y3U4qD1TLISIlX+8cIbAZJy8qU//OQlRgNhDl4gWBHBoTBIvniZg/YptkwGA=='
WHERE [Id] = 1;
SELECT @@ROWCOUNT;


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250416171044_InitialCreate', N'9.0.4');

COMMIT;
GO

