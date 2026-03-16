/*
 * 001_CreateTables.sql
 * Creates the AASHTOware database and core tables for the Project Dashboard.
 * Target: SQL Server 2016+
 */

-- Create database if it does not exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'AASHTOware')
    CREATE DATABASE [AASHTOware];
GO

USE [AASHTOware];
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Projects')
BEGIN
    CREATE TABLE [dbo].[Projects]
    (
        [ProjectId]    INT             IDENTITY(1,1) NOT NULL,
        [Name]         NVARCHAR(200)   NOT NULL,
        [Status]       NVARCHAR(50)    NOT NULL,
        [StartDate]    DATE            NOT NULL,
        [EndDate]      DATE            NULL,
        [TotalBudget]  DECIMAL(18,2)   NOT NULL,
        [Location]     NVARCHAR(500)   NULL,
        [CreatedDate]  DATETIME2       NOT NULL  DEFAULT GETUTCDATE(),
        [ModifiedDate] DATETIME2       NOT NULL  DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([ProjectId])
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProjectItems')
BEGIN
    CREATE TABLE [dbo].[ProjectItems]
    (
        [ProjectItemId] INT             IDENTITY(1,1) NOT NULL,
        [ProjectId]     INT             NOT NULL,
        [ItemNumber]    NVARCHAR(50)    NOT NULL,
        [Description]   NVARCHAR(500)   NOT NULL,
        [Quantity]      DECIMAL(18,4)   NOT NULL,
        [Unit]          NVARCHAR(20)    NOT NULL,
        [UnitPrice]     DECIMAL(18,4)   NOT NULL,
        [Amount]        DECIMAL(18,2)   NOT NULL,
        [CreatedDate]   DATETIME2       NOT NULL  DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_ProjectItems] PRIMARY KEY CLUSTERED ([ProjectItemId]),
        CONSTRAINT [FK_ProjectItems_Projects] FOREIGN KEY ([ProjectId])
            REFERENCES [dbo].[Projects] ([ProjectId])
            ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'FundingSources')
BEGIN
    CREATE TABLE [dbo].[FundingSources]
    (
        [FundingSourceId]   INT             IDENTITY(1,1) NOT NULL,
        [ProjectId]         INT             NOT NULL,
        [SourceName]        NVARCHAR(200)   NOT NULL,
        [FundingIdentifier] NVARCHAR(100)   NULL,
        [AllocatedAmount]   DECIMAL(18,2)   NOT NULL,
        [CreatedDate]       DATETIME2       NOT NULL  DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_FundingSources] PRIMARY KEY CLUSTERED ([FundingSourceId]),
        CONSTRAINT [FK_FundingSources_Projects] FOREIGN KEY ([ProjectId])
            REFERENCES [dbo].[Projects] ([ProjectId])
            ON DELETE CASCADE
    );
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ValidationIssues')
BEGIN
    CREATE TABLE [dbo].[ValidationIssues]
    (
        [ValidationIssueId] INT             IDENTITY(1,1) NOT NULL,
        [ProjectId]         INT             NOT NULL,
        [Severity]          NVARCHAR(20)    NOT NULL,
        [Description]       NVARCHAR(1000)  NOT NULL,
        [AffectedField]     NVARCHAR(200)   NULL,
        [DetectedDate]      DATETIME2       NOT NULL  DEFAULT GETUTCDATE(),

        CONSTRAINT [PK_ValidationIssues] PRIMARY KEY CLUSTERED ([ValidationIssueId]),
        CONSTRAINT [FK_ValidationIssues_Projects] FOREIGN KEY ([ProjectId])
            REFERENCES [dbo].[Projects] ([ProjectId])
            ON DELETE CASCADE
    );
END
GO

-- =============================================================================
-- Indexes
-- =============================================================================

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectItems_ProjectId' AND object_id = OBJECT_ID('dbo.ProjectItems'))
    CREATE NONCLUSTERED INDEX [IX_ProjectItems_ProjectId]
        ON [dbo].[ProjectItems] ([ProjectId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProjectItems_Amount' AND object_id = OBJECT_ID('dbo.ProjectItems'))
    CREATE NONCLUSTERED INDEX [IX_ProjectItems_Amount]
        ON [dbo].[ProjectItems] ([ProjectId], [Amount] DESC);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_FundingSources_ProjectId' AND object_id = OBJECT_ID('dbo.FundingSources'))
    CREATE NONCLUSTERED INDEX [IX_FundingSources_ProjectId]
        ON [dbo].[FundingSources] ([ProjectId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ValidationIssues_ProjectId' AND object_id = OBJECT_ID('dbo.ValidationIssues'))
    CREATE NONCLUSTERED INDEX [IX_ValidationIssues_ProjectId]
        ON [dbo].[ValidationIssues] ([ProjectId]);
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ValidationIssues_Severity' AND object_id = OBJECT_ID('dbo.ValidationIssues'))
    CREATE NONCLUSTERED INDEX [IX_ValidationIssues_Severity]
        ON [dbo].[ValidationIssues] ([ProjectId], [Severity]);
GO
