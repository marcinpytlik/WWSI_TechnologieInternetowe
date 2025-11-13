/* Lab11_Kanban_MSSQLEdition.sql
   Kanban – schema + seed + constraints
*/
SET NOCOUNT ON;

IF OBJECT_ID('dbo.Tasks', 'U') IS NOT NULL DROP TABLE dbo.Tasks;
IF OBJECT_ID('dbo.Columns', 'U') IS NOT NULL DROP TABLE dbo.Columns;

CREATE TABLE dbo.Columns (
  Id   INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(50) NOT NULL,
  Ord  INT NOT NULL
);

CREATE TABLE dbo.Tasks (
  Id    INT IDENTITY(1,1) PRIMARY KEY,
  Title NVARCHAR(200) NOT NULL,
  ColId INT NOT NULL CONSTRAINT FK_Tasks_Columns FOREIGN KEY REFERENCES dbo.Columns(Id) ON DELETE CASCADE,
  Ord   INT NOT NULL
);

CREATE UNIQUE INDEX UX_Tasks_Col_Ord ON dbo.Tasks(ColId, Ord);

-- Seed
INSERT dbo.Columns(Name, Ord) VALUES (N'Todo',1),(N'Doing',2),(N'Done',3);
INSERT dbo.Tasks(Title, ColId, Ord) VALUES (N'Setup project',1,1),(N'Write docs',1,2),(N'Release',3,1);

-- Move example
-- UPDATE dbo.Tasks SET ColId = 2, Ord = 999 WHERE Id = 1;
-- SELECT c.Name, t.Id, t.Title, t.Ord FROM dbo.Tasks t JOIN dbo.Columns c ON c.Id=t.ColId ORDER BY c.Ord, t.Ord;
