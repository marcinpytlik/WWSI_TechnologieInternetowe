-- 05_seed.sql
USE DemoDB;
GO

-- Seed przykładowych użytkowników (hashy nie ustawiamy – robi to backend)
IF NOT EXISTS (SELECT 1 FROM dbo.Users)
BEGIN
  INSERT INTO dbo.Users(Username, Email)
  VALUES (N'jan.kowalski', N'jan.kowalski@example.com'),
         (N'anna.nowak', N'anna.nowak@example.com'),
         (N'piotr.zielinski', N'piotr.zielinski@example.com');
END
GO

-- Seed przykładowych zadań
IF NOT EXISTS (SELECT 1 FROM dbo.Tasks)
BEGIN
  INSERT INTO dbo.Tasks(UserId, Title) 
  SELECT Id, N'Pierwsze zadanie dla ' + Username FROM dbo.Users;
END
GO
