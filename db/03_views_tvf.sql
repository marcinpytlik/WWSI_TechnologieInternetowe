-- 03_views_tvf.sql
USE DemoDB;
GO

-- TVF do wyszukiwania użytkowników
CREATE OR ALTER FUNCTION dbo.ufn_GetUsersPaged(@q NVARCHAR(200) = NULL)
RETURNS TABLE
AS RETURN
  SELECT Id, Username, Email, CreatedAt
  FROM dbo.Users
  WHERE (@q IS NULL OR Username LIKE '%' + @q + '%' OR Email LIKE '%' + @q + '%');
GO

-- Widok do agregacji dziennych
CREATE OR ALTER VIEW dbo.vNewUsersDaily AS
SELECT CAST(CreatedAt AS date) AS [Day], COUNT(*) AS NewUsers
FROM dbo.Users
GROUP BY CAST(CreatedAt AS date);
GO
