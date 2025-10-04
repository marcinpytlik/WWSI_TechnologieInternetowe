-- 04_procs.sql
USE DemoDB;
GO

-- Tworzenie zadania + wpis do AuditLog w jednej transakcji
CREATE OR ALTER PROCEDURE dbo.usp_Tasks_Create
  @UserId INT,
  @Title NVARCHAR(200)
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRAN;
      INSERT INTO dbo.Tasks(UserId, Title) VALUES(@UserId, @Title);
      DECLARE @NewId INT = SCOPE_IDENTITY();
      INSERT INTO dbo.AuditLog(UserId, Action, RefId) VALUES(@UserId, N'TASK_CREATED', @NewId);
    COMMIT;
    SELECT @NewId AS TaskId;
  END TRY
  BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
  END CATCH
END
GO

-- Oznaczenie zadania jako zakończone + wpis do AuditLog
CREATE OR ALTER PROCEDURE dbo.usp_Tasks_SetDone
  @TaskId INT
AS
BEGIN
  SET NOCOUNT ON;
  BEGIN TRY
    BEGIN TRAN;
      UPDATE dbo.Tasks SET IsDone = 1 WHERE TaskId = @TaskId;
      INSERT INTO dbo.AuditLog(UserId, Action, RefId)
      SELECT UserId, N'TASK_DONE', @TaskId FROM dbo.Tasks WHERE TaskId = @TaskId;
    COMMIT;
  END TRY
  BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK;
    THROW;
  END CATCH
END
GO

-- MERGE z tabeli staging do Users + TRUNCATE staging
CREATE OR ALTER PROCEDURE dbo.usp_Users_MergeFromStaging
AS
BEGIN
  SET NOCOUNT ON;
  MERGE dbo.Users AS T
  USING (SELECT DISTINCT Username, Email FROM dbo.Users_Staging) AS S
  ON T.Email = S.Email
  WHEN MATCHED THEN UPDATE SET T.Username = S.Username
  WHEN NOT MATCHED THEN INSERT(Username, Email) VALUES(S.Username, S.Email);
  TRUNCATE TABLE dbo.Users_Staging;
END
GO
