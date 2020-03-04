CREATE TABLE [dbo].[Classes]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [ParentID] INT NOT NULL, 
    [Name] NVARCHAR(50) NULL, 
    [Description] NVARCHAR(MAX) NULL
)
