
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 05/25/2015 18:36:30
-- Generated from EDMX file: E:\G1mist.CMS\G1mist.CMS\G1mist.CMS.Modal\DB_Model.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [DB_CMS];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_T_Articles_T_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[T_Articles] DROP CONSTRAINT [FK_T_Articles_T_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_T_Categories_T_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[T_Categories] DROP CONSTRAINT [FK_T_Categories_T_Users];
GO
IF OBJECT_ID(N'[dbo].[FK_T_Action_Role_T_Action]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[T_Action_Role] DROP CONSTRAINT [FK_T_Action_Role_T_Action];
GO
IF OBJECT_ID(N'[dbo].[FK_T_Action_Role_T_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[T_Action_Role] DROP CONSTRAINT [FK_T_Action_Role_T_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_T_User_Role_T_Role]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[T_User_Role] DROP CONSTRAINT [FK_T_User_Role_T_Role];
GO
IF OBJECT_ID(N'[dbo].[FK_T_User_Role_T_Users]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[T_User_Role] DROP CONSTRAINT [FK_T_User_Role_T_Users];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[T_Articles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Articles];
GO
IF OBJECT_ID(N'[dbo].[T_Categories]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Categories];
GO
IF OBJECT_ID(N'[dbo].[T_Links]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Links];
GO
IF OBJECT_ID(N'[dbo].[T_Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Users];
GO
IF OBJECT_ID(N'[dbo].[T_Action]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Action];
GO
IF OBJECT_ID(N'[dbo].[T_Action_Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Action_Role];
GO
IF OBJECT_ID(N'[dbo].[T_Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_Role];
GO
IF OBJECT_ID(N'[dbo].[T_User_Role]', 'U') IS NOT NULL
    DROP TABLE [dbo].[T_User_Role];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'T_Articles'
CREATE TABLE [dbo].[T_Articles] (
    [id] int IDENTITY(1,1) NOT NULL,
    [title] varchar(120)  NOT NULL,
    [body] varchar(max)  NOT NULL,
    [abstract] varchar(300)  NULL,
    [from] varchar(120)  NULL,
    [author] varchar(30)  NULL,
    [cateid] int  NULL,
    [uid] int  NULL,
    [createtime] datetime  NOT NULL
);
GO

-- Creating table 'T_Categories'
CREATE TABLE [dbo].[T_Categories] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(120)  NOT NULL,
    [parentid] int  NOT NULL,
    [uid] int  NULL,
    [createtime] datetime  NULL
);
GO

-- Creating table 'T_Links'
CREATE TABLE [dbo].[T_Links] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(120)  NOT NULL,
    [url] varchar(120)  NOT NULL,
    [createtime] datetime  NULL
);
GO

-- Creating table 'T_Users'
CREATE TABLE [dbo].[T_Users] (
    [id] int IDENTITY(1,1) NOT NULL,
    [username] varchar(50)  NOT NULL,
    [password] varchar(32)  NOT NULL,
    [salt] varchar(32)  NOT NULL,
    [createtime] datetime  NOT NULL,
    [lastlogintime] datetime  NULL,
    [lastloginip] varchar(32)  NULL,
    [lastloginarea] varchar(100)  NULL,
    [type] int  NOT NULL
);
GO

-- Creating table 'T_Action'
CREATE TABLE [dbo].[T_Action] (
    [id] int IDENTITY(1,1) NOT NULL,
    [controllername] varchar(100)  NOT NULL,
    [actionname] varchar(100)  NULL
);
GO

-- Creating table 'T_Action_Role'
CREATE TABLE [dbo].[T_Action_Role] (
    [id] int IDENTITY(1,1) NOT NULL,
    [aid] int  NULL,
    [rid] int  NULL
);
GO

-- Creating table 'T_Role'
CREATE TABLE [dbo].[T_Role] (
    [id] int IDENTITY(1,1) NOT NULL,
    [name] varchar(100)  NOT NULL
);
GO

-- Creating table 'T_User_Role'
CREATE TABLE [dbo].[T_User_Role] (
    [id] int IDENTITY(1,1) NOT NULL,
    [uid] int  NULL,
    [rid] int  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'T_Articles'
ALTER TABLE [dbo].[T_Articles]
ADD CONSTRAINT [PK_T_Articles]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_Categories'
ALTER TABLE [dbo].[T_Categories]
ADD CONSTRAINT [PK_T_Categories]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_Links'
ALTER TABLE [dbo].[T_Links]
ADD CONSTRAINT [PK_T_Links]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_Users'
ALTER TABLE [dbo].[T_Users]
ADD CONSTRAINT [PK_T_Users]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_Action'
ALTER TABLE [dbo].[T_Action]
ADD CONSTRAINT [PK_T_Action]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_Action_Role'
ALTER TABLE [dbo].[T_Action_Role]
ADD CONSTRAINT [PK_T_Action_Role]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_Role'
ALTER TABLE [dbo].[T_Role]
ADD CONSTRAINT [PK_T_Role]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- Creating primary key on [id] in table 'T_User_Role'
ALTER TABLE [dbo].[T_User_Role]
ADD CONSTRAINT [PK_T_User_Role]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [uid] in table 'T_Articles'
ALTER TABLE [dbo].[T_Articles]
ADD CONSTRAINT [FK_T_Articles_T_Users]
    FOREIGN KEY ([uid])
    REFERENCES [dbo].[T_Users]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_T_Articles_T_Users'
CREATE INDEX [IX_FK_T_Articles_T_Users]
ON [dbo].[T_Articles]
    ([uid]);
GO

-- Creating foreign key on [uid] in table 'T_Categories'
ALTER TABLE [dbo].[T_Categories]
ADD CONSTRAINT [FK_T_Categories_T_Users]
    FOREIGN KEY ([uid])
    REFERENCES [dbo].[T_Users]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_T_Categories_T_Users'
CREATE INDEX [IX_FK_T_Categories_T_Users]
ON [dbo].[T_Categories]
    ([uid]);
GO

-- Creating foreign key on [aid] in table 'T_Action_Role'
ALTER TABLE [dbo].[T_Action_Role]
ADD CONSTRAINT [FK_T_Action_Role_T_Action]
    FOREIGN KEY ([aid])
    REFERENCES [dbo].[T_Action]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_T_Action_Role_T_Action'
CREATE INDEX [IX_FK_T_Action_Role_T_Action]
ON [dbo].[T_Action_Role]
    ([aid]);
GO

-- Creating foreign key on [rid] in table 'T_Action_Role'
ALTER TABLE [dbo].[T_Action_Role]
ADD CONSTRAINT [FK_T_Action_Role_T_Role]
    FOREIGN KEY ([rid])
    REFERENCES [dbo].[T_Role]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_T_Action_Role_T_Role'
CREATE INDEX [IX_FK_T_Action_Role_T_Role]
ON [dbo].[T_Action_Role]
    ([rid]);
GO

-- Creating foreign key on [rid] in table 'T_User_Role'
ALTER TABLE [dbo].[T_User_Role]
ADD CONSTRAINT [FK_T_User_Role_T_Role]
    FOREIGN KEY ([rid])
    REFERENCES [dbo].[T_Role]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_T_User_Role_T_Role'
CREATE INDEX [IX_FK_T_User_Role_T_Role]
ON [dbo].[T_User_Role]
    ([rid]);
GO

-- Creating foreign key on [uid] in table 'T_User_Role'
ALTER TABLE [dbo].[T_User_Role]
ADD CONSTRAINT [FK_T_User_Role_T_Users]
    FOREIGN KEY ([uid])
    REFERENCES [dbo].[T_Users]
        ([id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_T_User_Role_T_Users'
CREATE INDEX [IX_FK_T_User_Role_T_Users]
ON [dbo].[T_User_Role]
    ([uid]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------