USE [master]
GO
/*** select name from sys.databases; **/
DECLARE @kill varchar(8000) = '';  
SELECT @kill = @kill + 'kill ' + CONVERT(varchar(5), session_id) + ';'  
FROM sys.dm_exec_sessions
WHERE database_id  = db_id('NETCoreBase')
EXEC(@kill);
GO
/*** select name from sys.databases; **/
DROP DATABASE IF EXISTS [NETCoreBase]
GO
/****** Object:  Database [NETCoreBase]    Script Date: 2021/05/23 00:57:41 ******/
CREATE DATABASE [NETCoreBase]
GO
ALTER DATABASE [NETCoreBase] SET COMPATIBILITY_LEVEL = 140
GO
ALTER DATABASE [NETCoreBase] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [NETCoreBase] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [NETCoreBase] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [NETCoreBase] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [NETCoreBase] SET ARITHABORT OFF 
GO
ALTER DATABASE [NETCoreBase] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [NETCoreBase] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [NETCoreBase] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [NETCoreBase] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [NETCoreBase] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [NETCoreBase] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [NETCoreBase] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [NETCoreBase] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [NETCoreBase] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [NETCoreBase] SET  ENABLE_BROKER 
GO
ALTER DATABASE [NETCoreBase] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [NETCoreBase] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [NETCoreBase] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [NETCoreBase] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [NETCoreBase] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [NETCoreBase] SET READ_COMMITTED_SNAPSHOT ON 
GO
ALTER DATABASE [NETCoreBase] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [NETCoreBase] SET RECOVERY FULL 
GO
ALTER DATABASE [NETCoreBase] SET  MULTI_USER 
GO
ALTER DATABASE [NETCoreBase] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [NETCoreBase] SET DB_CHAINING OFF 
GO
ALTER DATABASE [NETCoreBase] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [NETCoreBase] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [NETCoreBase] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [NETCoreBase] SET QUERY_STORE = OFF
GO
USE [NETCoreBase]
GO
/****** Object:  Table [dbo].[Banners]    Script Date: 2021/05/23 01:16:07 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banners](
	[Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(), -- Id
	[Image] [nvarchar](max) NULL, -- 圖片路徑
	[Enable] [char](1) NOT NULL DEFAULT 'U', -- 狀態（A：上架、U：未上架、D：刪除）
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateDate] [datetimeoffset](7) NOT NULL,
	[CreateUser] [nvarchar](20) NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[Version] [bigint] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_Banners] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Features]    Script Date: 2021/05/23 00:57:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Features](
	[Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(), -- 功能Id
	[ParentId] [uniqueidentifier] NULL, -- 父功能Id
	[Seq] [int] NOT NULL DEFAULT 1, -- 順序
	[Name] [nvarchar](30) NOT NULL, -- 功能名稱
	[Path] [nvarchar](100) NULL, -- 功能路徑
	[Status] [char](1) NOT NULL DEFAULT 'A', -- 狀態（A：啟用、D：刪除、U：不顯示）
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[UpdateDate] [datetimeoffset](7) NULL,
	[Version] [bigint] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_Features] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) WITH (
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY],
 CONSTRAINT [FK_Features_Features_ParentId] FOREIGN KEY ([ParentId]) 
 REFERENCES [dbo].[Features] ([Id])
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[FeaturePermissions]    Script Date: 2021/05/23 01:16:08 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FeaturePermissions](
	[FeatureId] [uniqueidentifier] NOT NULL, -- 功能Id
	[Permission] [nvarchar](20) NOT NULL, -- 權限項目
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[UpdateDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_FeaturePermissions] PRIMARY KEY CLUSTERED 
(
	[FeatureId] ASC,
	[Permission] ASC
) WITH (
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY],
 CONSTRAINT [FK_FeaturePermissions_Features_FeatureId] FOREIGN KEY ([FeatureId]) 
 REFERENCES [dbo].[Features] ([Id]) ON DELETE CASCADE
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2021/05/23 00:57:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(), -- Id
	[Name] [nvarchar](256) NULL, -- 角色名稱（英文）
	[NormalizedName] [nvarchar](256) NULL, -- 角色名稱（中文）
	[Status] [char](1) NOT NULL DEFAULT 'A', -- 狀態（A：啟用、D：刪除）
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[UpdateDate] [datetimeoffset](7) NULL,
	[Version] [bigint] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) WITH (
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY],
 CONSTRAINT [RoleNameIndex] UNIQUE NONCLUSTERED ([NormalizedName] ASC)
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RoleFeatures]    Script Date: 2021/05/23 00:57:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RoleFeatures](
	[RoleId] [uniqueidentifier] NOT NULL, -- 角色Id
	[FeatureId] [uniqueidentifier] NOT NULL, -- 功能Id
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[UpdateDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_RoleFeatures] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC,
	[FeatureId] ASC
) WITH (
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY],
 CONSTRAINT [FK_RoleFeatures_Features_FeatureId] FOREIGN KEY ([FeatureId]) 
 REFERENCES [dbo].[Features] ([Id]) ON DELETE CASCADE,
 CONSTRAINT [FK_RoleFeatures_Roles_RoleId] FOREIGN KEY ([RoleId]) 
 REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
 INDEX [IX_RoleFeatures_FeatureId] NONCLUSTERED ([FeatureId] ASC)
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2021/05/23 00:57:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(), -- Id
	[UserName] [nvarchar](256) NOT NULL, -- 帳號
	[PasswordHash] [nvarchar](max) NULL, -- 密碼（加密）
	[NormalizedUserName] [nvarchar](256) NULL, -- 姓名
	[Email] [nvarchar](256) NULL, -- 電子郵件
	[EmailConfirmed] [char](1) NOT NULL DEFAULT 'D', -- 郵件是否認證（N：否、Y：是、D：不需要）
	[PhoneNumber] [nvarchar](10) NULL, -- 手機號碼
	[PhoneNumberConfirmed] [char](1) NOT NULL DEFAULT 'D', -- 手機是否認證（N：否、Y：是、D：不需要）
	[TwoFactorEnabled] [char](1) NOT NULL DEFAULT 'N', -- 是否二階段驗證（N：否、Y：是）
	[AccessFailedCount] [int] NOT NULL DEFAULT 0, -- 登入錯誤次數
	[RequireChangeMima] [char](1) NOT NULL DEFAULT 'N', -- 是否需要更換密碼（N：否、Y：是）
	[LastChangeMimaDate] [datetimeoffset](7) NULL, -- 上次更換密碼時間
	[LastLogin] [datetimeoffset](7) NULL, -- 上次登入時間
	[Lockout] [datetimeoffset](7) NULL, -- 鎖定時間
	[Status] [char](1) NOT NULL DEFAULT 'A', -- 狀態（A：啟用、D：刪除）
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[UpdateDate] [datetimeoffset](7) NULL,
	[Version] [bigint] NOT NULL DEFAULT 0,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserLogins]    Script Date: 2021/05/23 00:57:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserLogins](
	[Id] [uniqueidentifier] NOT NULL DEFAULT newsequentialid(), -- Id
	[UserName] [nvarchar](256) NOT NULL, -- 帳號
	[PasswordHash] [nvarchar](max) NULL, -- 密碼（加密）
	[Status] [char](1) NOT NULL DEFAULT 'F', -- 狀態（S：成功、F：失敗）
	[UserId] [uniqueidentifier] NULL, -- 使用者 Id
	[CreateDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_UserLogins] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
) WITH (
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2021/05/23 00:57:41 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[UserId] [uniqueidentifier] NOT NULL, -- 使用者 Id
	[RoleId] [uniqueidentifier] NOT NULL, -- 角色 Id
	[CreateUser] [nvarchar](20) NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[UpdateUser] [nvarchar](20) NULL,
	[UpdateDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_UserRoles] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
) WITH (
	PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY],
 CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) 
 REFERENCES [dbo].[Roles] ([Id]) ON DELETE CASCADE,
 CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) 
 REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
) ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [NETCoreBase] SET  READ_WRITE 
GO
