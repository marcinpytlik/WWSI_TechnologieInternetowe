# Lab 05 — Entity Framework Core + SQL Server w Dockerze

## Cel

W tym laboratorium zastąpisz pamięciowy magazyn danych z Lab 04 prawdziwą bazą SQL Server obsługiwaną przez Entity Framework Core.

Po wykonaniu laba potrafisz:
- skonfigurować SQL Server w Dockerze,
- połączyć ASP.NET Core z SQL Serverem,
- utworzyć `DbContext`,
- mapować encje przez EF Core,
- wykonywać migracje,
- seedować dane,
- wykonywać CRUD przez EF Core,
- używać LINQ do filtrowania, sortowania i paginacji,
- uruchamiać API i bazę przez Docker Compose,
- korzystać z persistent volume,
- rozumieć różnicę między kontenerem a trwałością danych.

## Architektura

```text
Client
  |
  | HTTP :8080
  v
+---------------------------+
| ASP.NET Core Web API      |
| REST / DTO / Validation   |
+-------------+-------------+
              |
              | EF Core
              v
+---------------------------+
| SQL Server 2022           |
| Docker container          |
| persistent volume         |
+---------------------------+
```

## Struktura

```text
labs/05-efcore-sqlserver/
├── README.md
├── docker-compose.yml
├── Dockerfile
├── requests/
│   └── efcore.http
└── src/
    └── CourseApi/
        ├── CourseApi.csproj
        ├── Program.cs
        ├── Data/
        │   ├── CourseDbContext.cs
        │   └── DbSeeder.cs
        ├── Contracts/
        │   ├── CreateTopicRequest.cs
        │   ├── UpdateTopicRequest.cs
        │   ├── TopicResponse.cs
        │   └── PagedResponse.cs
        └── Models/
            └── CourseTopic.cs
```

---

# Krok 1 — Uruchom środowisko

Przejdź do:

```powershell
cd labs/05-efcore-sqlserver
```

Uruchom:

```powershell
docker compose up --build -d
```

Sprawdź:

```powershell
docker compose ps
```

Powinny działać dwa kontenery:

```text
ti-lab05-api
ti-lab05-sql
```

---

# Krok 2 — SQL Server w Dockerze

W `docker-compose.yml` znajdź usługę:

```yaml
sqlserver:
```

Zwróć uwagę na:
- obraz SQL Server 2022,
- hasło SA,
- port 1433,
- volume,
- healthcheck.

## Pytania

1. Dlaczego baza działa jako osobny kontener?
2. Po co jest volume?
3. Co się stanie po usunięciu kontenera bez usunięcia volume?
4. Co się stanie po `docker compose down -v`?

---

# Krok 3 — Connection string

API pobiera connection string z konfiguracji:

```text
ConnectionStrings:CourseDb
```

W Docker Compose przekazujemy go jako:

```yaml
ConnectionStrings__CourseDb: Server=sqlserver,1433;Database=CourseDb;...
```

Zwróć uwagę:

```text
Server=sqlserver
```

Nie używamy:

```text
localhost
```

## Pytanie

Dlaczego API łączy się z hostem `sqlserver`, a nie z `localhost`?

---

# Krok 4 — DbContext

Otwórz:

```text
Data/CourseDbContext.cs
```

Znajdź:

```csharp
DbSet<CourseTopic>
```

oraz konfigurację encji w:

```csharp
OnModelCreating(...)
```

Zwróć uwagę na:
- primary key,
- długość kolumny,
- unique index.

---

# Krok 5 — Rejestracja EF Core

W `Program.cs` znajdź:

```csharp
builder.Services.AddDbContext<CourseDbContext>(options =>
    options.UseSqlServer(connectionString));
```

To rejestruje `DbContext` w DI.

---

# Krok 6 — Migracje

W tym labie aplikacja przy starcie wykonuje:

```csharp
await db.Database.MigrateAsync();
```

Dzięki temu schema jest aktualizowana automatycznie.

W produkcji decyzja o automatycznych migracjach wymaga ostrożności, ale w laboratorium upraszcza start środowiska.

## Pytania

1. Co robi migracja?
2. Czy migracja to to samo co seed danych?
3. Dlaczego migracje warto przechowywać w repo?

---

# Krok 7 — Seed danych

Po migracji uruchamiany jest:

```csharp
DbSeeder.SeedAsync(...)
```

Jeżeli tabela jest pusta, dodawane są przykładowe rekordy.

Sprawdź:

```powershell
curl.exe -i http://localhost:8080/api/topics
```

---

# Krok 8 — GET z EF Core

Endpoint używa teraz:

```csharp
db.Topics.AsNoTracking()
```

oraz LINQ.

Zwróć uwagę na:
- `Where`,
- `OrderBy`,
- `Skip`,
- `Take`,
- `Select`.

## Pytanie

Dlaczego `AsNoTracking()` jest dobrym wyborem dla zapytań tylko do odczytu?

---

# Krok 9 — POST do bazy

Dodaj nowy temat:

```powershell
curl.exe -i -X POST -H "Content-Type: application/json" -d "{\"name\":\"Entity Framework Core\"}" http://localhost:8080/api/topics
```

API wykonuje:

```csharp
db.Topics.Add(topic);
await db.SaveChangesAsync();
```

Oczekiwany kod:

```text
201 Created
```

---

# Krok 10 — Persistence

Dodaj nowy temat.

Następnie:

```powershell
docker compose restart api
docker compose restart sqlserver
```

Ponownie:

```powershell
curl.exe http://localhost:8080/api/topics
```

Dane powinny nadal istnieć.

---

# Krok 11 — Usuń kontenery, zachowaj dane

Wykonaj:

```powershell
docker compose down
docker compose up -d
```

Dane nadal powinny istnieć, ponieważ volume nie został usunięty.

---

# Krok 12 — Usuń volume

UWAGA: to usuwa dane laba.

```powershell
docker compose down -v
docker compose up --build -d
```

Baza zostanie utworzona od nowa, a seed uruchomi się ponownie.

---

# Krok 13 — Healthcheck SQL Server

Sprawdź:

```powershell
docker compose ps
```

Usługa API startuje dopiero wtedy, gdy SQL Server jest healthy.

## Pytanie

Dlaczego samo `depends_on` bez healthchecka nie gwarantuje gotowości bazy?

---

# Krok 14 — PUT i DELETE

Aktualizacja:

```powershell
curl.exe -i -X PUT -H "Content-Type: application/json" -d "{\"name\":\"EF Core 10\"}" http://localhost:8080/api/topics/1
```

Usuwanie:

```powershell
curl.exe -i -X DELETE http://localhost:8080/api/topics/1
```

Obie operacje zapisują zmiany w SQL Serverze.

---

# Krok 15 — Logowanie zapytań

Podejrzyj logi:

```powershell
docker compose logs -f api
```

W trybie Development zobaczysz informacje o zapytaniach EF Core.

## Pytania

1. Co oznacza parametryzacja zapytań?
2. Dlaczego EF Core nie powinien budować SQL-a przez konkatenację stringów?

---

# Krok 16 — Swagger

Otwórz:

```text
http://localhost:8080/swagger
```

Przetestuj pełny CRUD.

---

# Weryfikacja

Lab jest zaliczony, jeśli potrafisz:
- uruchomić SQL Server w Dockerze,
- wyjaśnić connection string,
- wskazać `DbContext`,
- wyjaśnić `DbSet`,
- wykonać migrację,
- wykonać seed,
- wykonać CRUD przez EF Core,
- wyjaśnić `AsNoTracking`,
- pokazać trwałość danych po restarcie,
- wyjaśnić volume.

---

# Zadania dodatkowe

## Zadanie A — nowa kolumna

Dodaj do `CourseTopic`:

```text
Description
```

Utwórz migrację i zastosuj ją.

## Zadanie B — CreatedAt

Dodaj:

```csharp
DateTime CreatedAt
```

i ustawiaj wartość przy tworzeniu rekordu.

## Zadanie C — drugi indeks

Dodaj indeks na innym polu i sprawdź migrację.

## Zadanie D — SQL logging

Włącz bardziej szczegółowe logowanie EF Core w Development i sprawdź generowany SQL.

## Zadanie E — ręczne sprawdzenie bazy

Połącz się do SQL Servera narzędziem klienckim i wykonaj:

```sql
SELECT * FROM Topics;
```

---

# Cleanup

Zachowanie danych:

```powershell
docker compose down
```

Usunięcie danych:

```powershell
docker compose down -v
```

---

# Co dalej?

W Lab 06 dołączymy frontend z Lab 02 do naszego prawdziwego API.

Powstanie pierwszy pełny przepływ:

```text
Browser -> JavaScript fetch() -> ASP.NET Core -> EF Core -> SQL Server
```
