
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 09/22/2022 13:10:57
-- Generated from EDMX file: C:\Users\aiwass\Desktop\ProyectoV1\ProyectoV1\Scrabble99Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Scrabble99];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[Scrabble99ModelStoreContainer].[friendships]', 'U') IS NOT NULL
    DROP TABLE [Scrabble99ModelStoreContainer].[friendships];
GO
IF OBJECT_ID(N'[Scrabble99ModelStoreContainer].[gameResults]', 'U') IS NOT NULL
    DROP TABLE [Scrabble99ModelStoreContainer].[gameResults];
GO
IF OBJECT_ID(N'[Scrabble99ModelStoreContainer].[games]', 'U') IS NOT NULL
    DROP TABLE [Scrabble99ModelStoreContainer].[games];
GO
IF OBJECT_ID(N'[Scrabble99ModelStoreContainer].[players]', 'U') IS NOT NULL
    DROP TABLE [Scrabble99ModelStoreContainer].[players];
GO
IF OBJECT_ID(N'[Scrabble99ModelStoreContainer].[settings]', 'U') IS NOT NULL
    DROP TABLE [Scrabble99ModelStoreContainer].[settings];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'friendships'
CREATE TABLE [dbo].[friendships] (
    [idFriendship] int  NOT NULL,
    [player1Id] int  NOT NULL,
    [player2Id] int  NOT NULL
);
GO

-- Creating table 'gameResults'
CREATE TABLE [dbo].[gameResults] (
    [idGameResults] int  NOT NULL,
    [playerId] int  NOT NULL,
    [gameId] int  NOT NULL,
    [points] int  NOT NULL,
    [position] int  NOT NULL
);
GO

-- Creating table 'games'
CREATE TABLE [dbo].[games] (
    [idGame] int  NOT NULL,
    [duration] int  NOT NULL,
    [date] datetime  NOT NULL,
    [idWinner] int  NOT NULL
);
GO

-- Creating table 'players'
CREATE TABLE [dbo].[players] (
    [idUsuario] int  NOT NULL,
    [username] varchar(50)  NOT NULL,
    [password] varchar(50)  NOT NULL
);
GO

-- Creating table 'settings'
CREATE TABLE [dbo].[settings] (
    [idSettings] int  NOT NULL,
    [playerId] int  NOT NULL,
    [lenguege] varchar(5)  NULL,
    [videoResolutionWidth] int  NULL,
    [videoResolutionHeigth] int  NULL,
    [videoStyle] nchar(10)  NOT NULL,
    [audioMaster] int  NULL,
    [audioMusic] int  NULL,
    [audioEffects] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [idFriendship], [player1Id], [player2Id] in table 'friendships'
ALTER TABLE [dbo].[friendships]
ADD CONSTRAINT [PK_friendships]
    PRIMARY KEY CLUSTERED ([idFriendship], [player1Id], [player2Id] ASC);
GO

-- Creating primary key on [idGameResults], [playerId], [gameId], [points], [position] in table 'gameResults'
ALTER TABLE [dbo].[gameResults]
ADD CONSTRAINT [PK_gameResults]
    PRIMARY KEY CLUSTERED ([idGameResults], [playerId], [gameId], [points], [position] ASC);
GO

-- Creating primary key on [idGame], [duration], [date], [idWinner] in table 'games'
ALTER TABLE [dbo].[games]
ADD CONSTRAINT [PK_games]
    PRIMARY KEY CLUSTERED ([idGame], [duration], [date], [idWinner] ASC);
GO

-- Creating primary key on [idUsuario], [username], [password] in table 'players'
ALTER TABLE [dbo].[players]
ADD CONSTRAINT [PK_players]
    PRIMARY KEY CLUSTERED ([idUsuario], [username], [password] ASC);
GO

-- Creating primary key on [idSettings], [playerId], [videoStyle] in table 'settings'
ALTER TABLE [dbo].[settings]
ADD CONSTRAINT [PK_settings]
    PRIMARY KEY CLUSTERED ([idSettings], [playerId], [videoStyle] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------