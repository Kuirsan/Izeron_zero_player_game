CREATE TABLE [dbo].[Quests]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Title] NVARCHAR(50) NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [isProfessionQuest] BIT NOT NULL, 
    [isBaseQuest] BIT NOT NULL
)
