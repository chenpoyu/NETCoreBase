using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace NETCoreBase.Tests.Integration
{
    /// <summary>
    /// Starts a SQL Server container for the integration test session.
    /// Shared across all tests in the collection via IAsyncLifetime.
    /// </summary>
    public class SqlServerFixture : IAsyncLifetime
    {
        private readonly MsSqlContainer _container;

        public string ConnectionString => _container.GetConnectionString();

        public SqlServerFixture()
        {
            _container = new MsSqlBuilder()
                .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
                .WithPassword("Test@1234567!")
                .Build();
        }

        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            await CreateSchemaAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync();
        }

        private async Task CreateSchemaAsync()
        {
            // Inline schema — matches NETCoreBase.Database/Schema.sql but pre-initialised for tests
            const string createDb = @"
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'NETCoreBase')
    CREATE DATABASE [NETCoreBase];
";
            const string usageAndTables = @"
USE [NETCoreBase];

IF OBJECT_ID('[dbo].[Banners]', 'U') IS NULL
CREATE TABLE [dbo].[Banners](
    [Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(),
    [Image] [nvarchar](max) NULL,
    [Enable] [char](1) NOT NULL DEFAULT 'U',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [CreateUser] [nvarchar](20) NULL,
    [UpdateUser] [nvarchar](20) NULL,
    [Version] [bigint] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Banners] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF OBJECT_ID('[dbo].[Features]', 'U') IS NULL
CREATE TABLE [dbo].[Features](
    [Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(),
    [ParentId] [uniqueidentifier] NULL,
    [Seq] [int] NOT NULL DEFAULT 1,
    [Name] [nvarchar](30) NOT NULL,
    [Path] [nvarchar](100) NULL,
    [Status] [char](1) NOT NULL DEFAULT 'A',
    [CreateUser] [nvarchar](20) NOT NULL DEFAULT 'system',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateUser] [nvarchar](20) NULL,
    [UpdateDate] [datetimeoffset](7) NULL,
    [Version] [bigint] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Features] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF OBJECT_ID('[dbo].[Roles]', 'U') IS NULL
CREATE TABLE [dbo].[Roles](
    [Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(),
    [Name] [nvarchar](256) NULL,
    [NormalizedName] [nvarchar](256) NULL,
    [Status] [char](1) NOT NULL DEFAULT 'A',
    [CreateUser] [nvarchar](20) NOT NULL DEFAULT 'system',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateUser] [nvarchar](20) NULL,
    [UpdateDate] [datetimeoffset](7) NULL,
    [Version] [bigint] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [RoleNameIndex] UNIQUE NONCLUSTERED ([NormalizedName] ASC)
);

IF OBJECT_ID('[dbo].[UserLogins]', 'U') IS NULL
CREATE TABLE [dbo].[UserLogins](
    [Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(),
    [UserName] [nvarchar](256) NOT NULL,
    [PasswordHash] [nvarchar](max) NULL,
    [Status] [char](1) NOT NULL DEFAULT 'F',
    [UserId] [uniqueidentifier] NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF OBJECT_ID('[dbo].[Users]', 'U') IS NULL
CREATE TABLE [dbo].[Users](
    [Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(),
    [UserName] [nvarchar](256) NOT NULL,
    [PasswordHash] [nvarchar](max) NULL,
    [NormalizedUserName] [nvarchar](256) NULL,
    [Email] [nvarchar](256) NULL,
    [EmailConfirmed] [char](1) NOT NULL DEFAULT 'D',
    [PhoneNumber] [nvarchar](10) NULL,
    [PhoneNumberConfirmed] [char](1) NOT NULL DEFAULT 'D',
    [TwoFactorEnabled] [char](1) NOT NULL DEFAULT 'N',
    [AccessFailedCount] [int] NOT NULL DEFAULT 0,
    [RequireChangeMima] [char](1) NOT NULL DEFAULT 'N',
    [LastChangeMimaDate] [datetimeoffset](7) NULL,
    [LastLogin] [datetimeoffset](7) NULL,
    [Lockout] [datetimeoffset](7) NULL,
    [Status] [char](1) NOT NULL DEFAULT 'A',
    [CreateUser] [nvarchar](20) NOT NULL DEFAULT 'system',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateUser] [nvarchar](20) NULL,
    [UpdateDate] [datetimeoffset](7) NULL,
    [Version] [bigint] NOT NULL DEFAULT 0,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([Id] ASC)
);

IF OBJECT_ID('[dbo].[UserRoles]', 'U') IS NULL
CREATE TABLE [dbo].[UserRoles](
    [UserId] [uniqueidentifier] NOT NULL,
    [RoleId] [uniqueidentifier] NOT NULL,
    [CreateUser] [nvarchar](20) NOT NULL DEFAULT 'system',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateUser] [nvarchar](20) NULL,
    [UpdateDate] [datetimeoffset](7) NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
);

IF OBJECT_ID('[dbo].[FeaturePermissions]', 'U') IS NULL
CREATE TABLE [dbo].[FeaturePermissions](
    [FeatureId] [uniqueidentifier] NOT NULL,
    [Permission] [nvarchar](20) NOT NULL,
    [CreateUser] [nvarchar](20) NOT NULL DEFAULT 'system',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateUser] [nvarchar](20) NULL,
    [UpdateDate] [datetimeoffset](7) NULL,
    CONSTRAINT [PK_FeaturePermissions] PRIMARY KEY CLUSTERED ([FeatureId] ASC, [Permission] ASC),
    CONSTRAINT [FK_FeaturePermissions_Features_FeatureId] FOREIGN KEY ([FeatureId]) REFERENCES [dbo].[Features] ([Id]) ON DELETE CASCADE
);

IF OBJECT_ID('[dbo].[RoleFeatures]', 'U') IS NULL
CREATE TABLE [dbo].[RoleFeatures](
    [RoleId] [uniqueidentifier] NOT NULL,
    [FeatureId] [uniqueidentifier] NOT NULL,
    [CreateUser] [nvarchar](20) NOT NULL DEFAULT 'system',
    [CreateDate] [datetimeoffset](7) NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    [UpdateUser] [nvarchar](20) NULL,
    [UpdateDate] [datetimeoffset](7) NULL,
    CONSTRAINT [PK_RoleFeatures] PRIMARY KEY CLUSTERED ([RoleId] ASC, [FeatureId] ASC),
    CONSTRAINT [FK_RoleFeatures_Features_FeatureId] FOREIGN KEY ([FeatureId]) REFERENCES [dbo].[Features] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_RoleFeatures_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE
);
";
            await using var masterConn = new SqlConnection(_container.GetConnectionString());
            await masterConn.OpenAsync();
            await using (var cmd = masterConn.CreateCommand())
            {
                cmd.CommandText = createDb;
                await cmd.ExecuteNonQueryAsync();
            }
            await masterConn.CloseAsync();

            // Now connect to NETCoreBase database
            var builder = new SqlConnectionStringBuilder(_container.GetConnectionString())
            {
                InitialCatalog = "NETCoreBase"
            };
            await using var dbConn = new SqlConnection(builder.ConnectionString);
            await dbConn.OpenAsync();
            await using var cmd2 = dbConn.CreateCommand();
            cmd2.CommandText = usageAndTables;
            await cmd2.ExecuteNonQueryAsync();
        }
    }
}
