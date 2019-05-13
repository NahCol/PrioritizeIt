CREATE TABLE [dbo].[l_priority_level]
(
	[Id] INT NOT NULL PRIMARY KEY Identity,
	[SortOrder] INT Not Null,
	[Text] varchar(50) NOT NULL, 
    [Active] BIT NOT NULL
)
