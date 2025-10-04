-- 02_indexes.sql
USE DemoDB;
GO

-- Unikalność zapewniona przez UNIQUE w definicji, ale dodajmy pomocnicze indeksy do sortowania/filtrów
CREATE INDEX IX_Users_CreatedAt ON dbo.Users(CreatedAt);
GO
CREATE INDEX IX_Tasks_User_Done ON dbo.Tasks(UserId, IsDone) INCLUDE (CreatedAt, Title);
GO
