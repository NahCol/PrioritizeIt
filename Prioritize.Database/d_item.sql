CREATE TABLE [dbo].[d_item]
(
	[Id] INT NOT NULL PRIMARY KEY Identity,
	[Board] varchar(50) null,
	[List] varchar(50) null,
	[Action] varchar(50) null,
	[CardNumber] VARCHAR(50) null, 
    [PriorityNumber] INT NOT NULL,
	[Requirement] varchar(max) null,
	[PriorityLevelId] int null,
	[StatusId] int not null,
	[CoderId] int null,
	[Link] varchar(max) null, 
    [Active] BIT NOT NULL, 
    [Description] VARCHAR(MAX) NULL, 
    [DueDate] DATETIME NULL, 
    CONSTRAINT [FK_d_items_PriorityLevel] FOREIGN KEY ([PriorityLevelId]) REFERENCES [l_priority_level]([Id]), 
    CONSTRAINT [FK_d_item_Coder] FOREIGN KEY ([CoderId]) REFERENCES [l_coder]([Id]),
	CONSTRAINT [FK_d_item_Status] FOREIGN KEY ([StatusId]) REFERENCES [l_status]([Id])
)
