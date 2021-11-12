CREATE TABLE [dbo].[RefreshTokens] (
    [Id]              INT            IDENTITY (1, 1) NOT NULL,
    [Token]           NVARCHAR (MAX) NULL,
    [Expires]         DATETIME2 (7)  NOT NULL,
    [Created]         DATETIME2 (7)  NOT NULL,
    [CreatedByIp]     NVARCHAR (MAX) NULL,
    [Revoked]         DATETIME2 (7)  NULL,
    [RevokedByIp]     NVARCHAR (MAX) NULL,
    [ReplacedByToken] NVARCHAR (MAX) NULL,
    [AccountId]       BIGINT         NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_RefreshTokens_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [dbo].[Accounts] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_RefreshTokens_AccountId]
    ON [dbo].[RefreshTokens]([AccountId] ASC);

