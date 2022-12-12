
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/11/2022 21:02:39
-- Generated from EDMX file: C:\Users\aiwass\source\repos\scrabble\DataAccess\ScrabbleModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [scrabble];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[friendships]', 'U') IS NOT NULL
    DROP TABLE [dbo].[friendships];
GO
IF OBJECT_ID(N'[dbo].[gameResults]', 'U') IS NOT NULL
    DROP TABLE [dbo].[gameResults];
GO
IF OBJECT_ID(N'[dbo].[games]', 'U') IS NOT NULL
    DROP TABLE [dbo].[games];
GO
IF OBJECT_ID(N'[dbo].[players]', 'U') IS NOT NULL
    DROP TABLE [dbo].[players];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'friendships'
CREATE TABLE [dbo].[friendships] (
    [FriendshipId] int IDENTITY(1,1) NOT NULL,
    [Sender] int  NOT NULL,
    [Receiver] int  NOT NULL,
    [Status] smallint  NOT NULL
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
    [UserId] int IDENTITY(1,1) NOT NULL,
    [Nickname] varchar(50)  NOT NULL,
    [Password] varchar(255)  NOT NULL,
    [Avatar] smallint  NOT NULL,
    [Email] nvarchar(255)  NOT NULL,
    [Registered] datetime  NOT NULL,
    [Games] int  NOT NULL,
    [Wins] int  NOT NULL,
    [VerificationCode] nvarchar(4)  NOT NULL,
    [Verified] bit  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [FriendshipId] in table 'friendships'
ALTER TABLE [dbo].[friendships]
ADD CONSTRAINT [PK_friendships]
    PRIMARY KEY CLUSTERED ([FriendshipId] ASC);
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

-- Creating primary key on [UserId] in table 'players'
ALTER TABLE [dbo].[players]
ADD CONSTRAINT [PK_players]
    PRIMARY KEY CLUSTERED ([UserId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------