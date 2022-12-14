
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 12/14/2022 03:02:08
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
    [GameResultId] int IDENTITY(1,1) NOT NULL,
    [PlayerId] int  NOT NULL,
    [GameId] int  NOT NULL,
    [Score] int  NOT NULL,
    [Placement] int  NOT NULL
);
GO

-- Creating table 'games'
CREATE TABLE [dbo].[games] (
    [GameId] int IDENTITY(1,1) NOT NULL,
    [Date] datetime  NOT NULL,
    [WinnerId] int  NULL
);
GO

-- Creating table 'players'
CREATE TABLE [dbo].[players] (
    [PlayerId] int IDENTITY(1,1) NOT NULL,
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

-- Creating primary key on [GameResultId], [PlayerId], [GameId] in table 'gameResults'
ALTER TABLE [dbo].[gameResults]
ADD CONSTRAINT [PK_gameResults]
    PRIMARY KEY CLUSTERED ([GameResultId], [PlayerId], [GameId] ASC);
GO

-- Creating primary key on [GameId] in table 'games'
ALTER TABLE [dbo].[games]
ADD CONSTRAINT [PK_games]
    PRIMARY KEY CLUSTERED ([GameId] ASC);
GO

-- Creating primary key on [PlayerId] in table 'players'
ALTER TABLE [dbo].[players]
ADD CONSTRAINT [PK_players]
    PRIMARY KEY CLUSTERED ([PlayerId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------