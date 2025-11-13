# MSSQLEdition — wymagania i wskazówki 

W tej edycji **wymagane jest użycie Microsoft SQL Server** (np. SQL Server 2022 Developer). Zmieniamy typy danych na `NVARCHAR`, daty na `DATETIME2(0)` lub `DATETIME2(3)`, klucze główne jako `INT IDENTITY(1,1)`, jawne **CHECK** i indeksy. Operacje INSERT powinny zwracać klucz przy pomocy `OUTPUT INSERTED.Id` (lub `SCOPE_IDENTITY()`).

## Wspólne wytyczne
- **Połączenie:**
  - .NET `SqlClient`: `Server=localhost;Database=TI_Lab;Trusted_Connection=True;TrustServerCertificate=True;`
  - Node `mssql`: `Server=localhost;Database=TI_Lab;User Id=...;Password=...;TrustServerCertificate=true;`
- **Typy:** `NVARCHAR(…)` (z sensownymi limitami, np. tytuły 200), liczby `INT`, pieniądze `DECIMAL(12,2)`, daty `DATETIME2(0/3)`.
- **Standardy:** każda tabela ma **PK (IDENTITY)**, klucze obce **FK** z odpowiednią akcją (`ON DELETE CASCADE/NO ACTION`), **CHECK** (zakresy), **DEFAULT** (np. daty). Dodaj indeksy pod zapytania i sortowanie.
- **Transakcje:** checkout/borrow/approve powinny używać `BEGIN TRAN; … COMMIT;` z poziomem izolacji przynajmniej `READ COMMITTED` (domyślnie).
- **Zwracanie kluczy:** w `INSERT` używaj `OUTPUT INSERTED.Id` lub po `INSERT` wywołuj `SELECT CAST(SCOPE_IDENTITY() AS INT)`.

---

## Lab01 — Wypożyczalnia (MSSQLEdition)

### Schema (T‑SQL)
```sql
CREATE TABLE dbo.Members (
  Id       INT IDENTITY(1,1) PRIMARY KEY,
  Name     NVARCHAR(100) NOT NULL,
  Email    NVARCHAR(200) NOT NULL UNIQUE
);

CREATE TABLE dbo.Books (
  Id       INT IDENTITY(1,1) PRIMARY KEY,
  Title    NVARCHAR(200) NOT NULL,
  Author   NVARCHAR(120) NOT NULL,
  Copies   INT NOT NULL CONSTRAINT CK_Books_Copies CHECK (Copies >= 0)
);

CREATE TABLE dbo.Loans (
  Id         INT IDENTITY(1,1) PRIMARY KEY,
  MemberId   INT NOT NULL CONSTRAINT FK_Loans_Members FOREIGN KEY REFERENCES dbo.Members(Id) ON DELETE CASCADE,
  BookId     INT NOT NULL CONSTRAINT FK_Loans_Books   FOREIGN KEY REFERENCES dbo.Books(Id)   ON DELETE CASCADE,
  LoanDate   DATETIME2(0) NOT NULL CONSTRAINT DF_Loans_LoanDate DEFAULT (SYSUTCDATETIME()),
  DueDate    DATETIME2(0) NOT NULL,
  ReturnDate DATETIME2(0) NULL
);

CREATE INDEX IX_Loans_Member ON dbo.Loans(MemberId);
CREATE INDEX IX_Loans_Book   ON dbo.Loans(BookId) INCLUDE(ReturnDate);
```

### Logika dostępności (przykład)
```sql
-- aktywne wypożyczenia dla danej książki
SELECT ActiveCount = COUNT(*)
FROM dbo.Loans
WHERE BookId = @BookId AND ReturnDate IS NULL;
```
**Akceptacja:** przy `ActiveCount >= Copies` `INSERT` wypożyczenia ma zwrócić błąd aplikacyjny (HTTP 409) — całość w transakcji.

---

## Lab02 — Sklep (MSSQLEdition)

### Schema (T‑SQL)
```sql
CREATE TABLE dbo.Products (
  Id    INT IDENTITY(1,1) PRIMARY KEY,
  Name  NVARCHAR(120) NOT NULL,
  Price DECIMAL(12,2) NOT NULL CONSTRAINT CK_Products_Price CHECK (Price >= 0)
);

CREATE TABLE dbo.Orders (
  Id         INT IDENTITY(1,1) PRIMARY KEY,
  CreatedAt  DATETIME2(0) NOT NULL CONSTRAINT DF_Orders_CreatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE TABLE dbo.OrderItems (
  Id         INT IDENTITY(1,1) PRIMARY KEY,
  OrderId    INT NOT NULL CONSTRAINT FK_OrderItems_Orders   FOREIGN KEY REFERENCES dbo.Orders(Id)   ON DELETE CASCADE,
  ProductId  INT NOT NULL CONSTRAINT FK_OrderItems_Products FOREIGN KEY REFERENCES dbo.Products(Id),
  Qty        INT NOT NULL CONSTRAINT CK_OrderItems_Qty CHECK (Qty > 0),
  Price      DECIMAL(12,2) NOT NULL
);

CREATE INDEX IX_OrderItems_Order ON dbo.OrderItems(OrderId) INCLUDE(Qty,Price);
```

### Checkout (transakcja)
- Pobierz snapshot produktów (`Price`) do pozycji, sumuj `Total` po stronie aplikacji, wszystko w `BEGIN TRAN … COMMIT`.
- **Akceptacja:** po `checkout` koszyk pusty, w DB istnieje `Orders` z powiązanymi `OrderItems` i poprawnym sumarycznym kosztem.

---

## Lab03 — Blog (MSSQLEdition)

### Schema (T‑SQL)
```sql
CREATE TABLE dbo.Posts (
  Id         INT IDENTITY(1,1) PRIMARY KEY,
  Title      NVARCHAR(200) NOT NULL,
  Body       NVARCHAR(MAX) NOT NULL,
  CreatedAt  DATETIME2(0) NOT NULL CONSTRAINT DF_Posts_CreatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE TABLE dbo.Comments (
  Id         INT IDENTITY(1,1) PRIMARY KEY,
  PostId     INT NOT NULL CONSTRAINT FK_Comments_Posts FOREIGN KEY REFERENCES dbo.Posts(Id) ON DELETE CASCADE,
  Author     NVARCHAR(100) NOT NULL,
  Body       NVARCHAR(1000) NOT NULL,
  CreatedAt  DATETIME2(0) NOT NULL CONSTRAINT DF_Comments_CreatedAt DEFAULT (SYSUTCDATETIME()),
  Approved   BIT NOT NULL CONSTRAINT DF_Comments_Approved DEFAULT (0)
);

CREATE INDEX IX_Comments_Post ON dbo.Comments(PostId) INCLUDE(Approved, CreatedAt);
```
**Akceptacja:** endpoint publiczny zwraca wyłącznie `Approved = 1`. Operacja „approve” aktualizuje wiersz i natychmiast wpływa na widok.

---

## Lab04 — Filmy i oceny (MSSQLEdition)

### Schema (T‑SQL)
```sql
CREATE TABLE dbo.Movies (
  Id    INT IDENTITY(1,1) PRIMARY KEY,
  Title NVARCHAR(200) NOT NULL,
  [Year] INT NOT NULL
);

CREATE TABLE dbo.Ratings (
  Id       INT IDENTITY(1,1) PRIMARY KEY,
  MovieId  INT NOT NULL CONSTRAINT FK_Ratings_Movies FOREIGN KEY REFERENCES dbo.Movies(Id) ON DELETE CASCADE,
  Score    INT NOT NULL CONSTRAINT CK_Ratings_Score CHECK (Score BETWEEN 1 AND 5)
);

-- widok rankingowy (opcjonalnie)
CREATE OR ALTER VIEW dbo.vMoviesRanking AS
SELECT m.Id, m.Title, m.[Year],
       CAST(AVG(CAST(r.Score AS DECIMAL(5,2))) AS DECIMAL(5,2)) AS AvgScore,
       COUNT(r.Id) AS Votes
FROM dbo.Movies m
LEFT JOIN dbo.Ratings r ON r.MovieId = m.Id
GROUP BY m.Id, m.Title, m.[Year];
```
**Akceptacja:** lista filmów zwraca pola `AvgScore` i `Votes` zgodne z widokiem/rzeczywistością; walidacja zakresu `Score`.

---

## Lab05 — Kanban (MSSQLEdition)

### Schema (T‑SQL)
```sql
CREATE TABLE dbo.Columns (
  Id   INT IDENTITY(1,1) PRIMARY KEY,
  Name NVARCHAR(50) NOT NULL,
  Ord  INT NOT NULL
);

CREATE TABLE dbo.Tasks (
  Id     INT IDENTITY(1,1) PRIMARY KEY,
  Title  NVARCHAR(200) NOT NULL,
  ColId  INT NOT NULL CONSTRAINT FK_Tasks_Columns FOREIGN KEY REFERENCES dbo.Columns(Id) ON DELETE CASCADE,
  Ord    INT NOT NULL
);

CREATE UNIQUE INDEX UX_Tasks_Col_Ord ON dbo.Tasks(ColId, Ord);
```
**Akceptacja:** dodanie ustawia `Ord = ISNULL(MAX(Ord)+1,1)` w danej kolumnie; przeniesienie aktualizuje `ColId, Ord` i zachowuje spójność unikalności.

---

## Lab06 — Notatki i tagi (MSSQLEdition)

### Schema (T‑SQL)
```sql
CREATE TABLE dbo.Notes (
  Id         INT IDENTITY(1,1) PRIMARY KEY,
  Title      NVARCHAR(200) NOT NULL,
  Body       NVARCHAR(MAX) NOT NULL,
  CreatedAt  DATETIME2(0) NOT NULL CONSTRAINT DF_Notes_CreatedAt DEFAULT (SYSUTCDATETIME())
);

CREATE TABLE dbo.Tags (
  Id    INT IDENTITY(1,1) PRIMARY KEY,
  Name  NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE dbo.NoteTags (
  NoteId INT NOT NULL CONSTRAINT FK_NoteTags_Notes FOREIGN KEY REFERENCES dbo.Notes(Id) ON DELETE CASCADE,
  TagId  INT NOT NULL CONSTRAINT FK_NoteTags_Tags  FOREIGN KEY REFERENCES dbo.Tags(Id)  ON DELETE CASCADE,
  CONSTRAINT PK_NoteTags PRIMARY KEY (NoteId, TagId)
);

-- opcjonalnie pełnotekstowe: wymaga Full-Text Search (FTS)
-- CREATE FULLTEXT CATALOG ftc AS DEFAULT;
-- CREATE FULLTEXT INDEX ON dbo.Notes(Title LANGUAGE 1045, Body LANGUAGE 1045) KEY INDEX PK__Notes__Id;
```
**Akceptacja:** unikalność (NoteId, TagId) wymuszana kluczem złożonym; wyszukiwanie minimum po `LIKE`, opcjonalnie po FTS.

---

## Kryteria dodatkowe MSSQLEdition
- Każdy `INSERT` zwraca nowy klucz (JSON z `Id`).
- W raportach wymagaj **indeksów pokrywających** (`INCLUDE`) tam, gdzie to naturalne (lista, sortowanie).
- Dla operacji wielokrokowych użyj transakcji; w razie błędu `ROLLBACK`.
- README powinien zawierać skrypty tworzące schemat + przykładowe dane oraz sekcję „Testy ręczne” (przykładowe zapytania T‑SQL).

