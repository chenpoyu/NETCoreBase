-- Migration V2: Add optimistic locking Version column to main tables
ALTER TABLE [dbo].[Users]    ADD [Version] BIGINT NOT NULL DEFAULT 0;
ALTER TABLE [dbo].[Roles]    ADD [Version] BIGINT NOT NULL DEFAULT 0;
ALTER TABLE [dbo].[Features] ADD [Version] BIGINT NOT NULL DEFAULT 0;
ALTER TABLE [dbo].[Banners]  ADD [Version] BIGINT NOT NULL DEFAULT 0;
