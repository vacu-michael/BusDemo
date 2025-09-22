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
CREATE TABLE [Applications] (
    [Id] int NOT NULL IDENTITY,
    [AccountNumber] bigint NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY ([Id])
);

CREATE TABLE [Settings] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Value] bit NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY ([Id])
);


-- Seed initial Settings
INSERT INTO [Settings] ([Name], [Value]) VALUES (N'CoreAvailable', 1);
INSERT INTO [Settings] ([Name], [Value]) VALUES (N'ThrowError', 0);
INSERT INTO [Settings] ([Name], [Value]) VALUES (N'Delay', 0);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250919173530_InitialMigration', N'9.0.9');

COMMIT;
GO

