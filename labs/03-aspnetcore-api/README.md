# Lab 03 — Pierwsze ASP.NET Core Web API

## Cel

W tym laboratorium zbudujesz pierwsze API w ASP.NET Core i uruchomisz je w Dockerze.

Po wykonaniu laba potrafisz:
- utworzyć projekt ASP.NET Core Web API,
- wyjaśnić rolę `Program.cs`,
- rozpoznać middleware pipeline,
- zdefiniować endpointy GET i POST,
- użyć Dependency Injection,
- odczytać konfigurację,
- korzystać z loggingu,
- uruchomić Swagger / OpenAPI,
- testować API przez przeglądarkę, Swagger, `curl` i plik `.http`,
- zbudować obraz Docker i uruchomić API w kontenerze.

## Architektura

```text
Browser / curl / REST Client
            |
            | HTTP :8080
            v
+---------------------------+
| ASP.NET Core Web API      |
|                           |
| Program.cs                |
| Middleware                |
| Endpoints                 |
| DI                        |
| Logging                   |
| Swagger / OpenAPI         |
+---------------------------+
```

W tym labie nie używamy jeszcze bazy danych.

Dane są przechowywane w pamięci procesu, dzięki czemu skupiamy się na samym API.

## Wymagania

- Docker Desktop
- .NET SDK
- Visual Studio Code lub Visual Studio
- wykonany Lab 02 lub znajomość podstaw HTTP i fetch

Sprawdź:

```powershell
dotnet --version
docker --version
docker compose version
```

## Struktura

```text
labs/03-aspnetcore-api/
├── README.md
├── docker-compose.yml
├── Dockerfile
├── requests/
│   └── api.http
└── src/
    └── CourseApi/
        ├── CourseApi.csproj
        ├── Program.cs
        ├── Models/
        │   └── CourseTopic.cs
        └── Services/
            ├── ICourseTopicService.cs
            └── CourseTopicService.cs
```

---

# Krok 1 — Uruchom API lokalnie

Przejdź do:

```powershell
cd labs/03-aspnetcore-api/src/CourseApi
```

Przywróć pakiety:

```powershell
dotnet restore
```

Uruchom:

```powershell
dotnet run
```

Sprawdź adres pokazany w terminalu.

Jeżeli uruchomisz aplikację bez Dockera, port może być inny niż `8080`.

---

# Krok 2 — Program.cs

Otwórz:

```text
src/CourseApi/Program.cs
```

Znajdź:

```csharp
var builder = WebApplication.CreateBuilder(args);
```

oraz:

```csharp
var app = builder.Build();
```

## Pytania

1. Co konfiguruje `builder`?
2. Co reprezentuje `app`?
3. Kiedy konfigurowane są usługi DI?
4. Kiedy budowany jest pipeline HTTP?

---

# Krok 3 — Dependency Injection

Znajdź rejestrację:

```csharp
builder.Services.AddSingleton<ICourseTopicService, CourseTopicService>();
```

To oznacza, że ASP.NET Core może dostarczyć implementację serwisu do endpointu.

Przykład:

```csharp
app.MapGet("/api/topics", (ICourseTopicService service) =>
{
    return Results.Ok(service.GetAll());
});
```

## Pytanie

Dlaczego endpoint nie tworzy serwisu przez:

```csharp
new CourseTopicService()
```

?

---

# Krok 4 — Pierwszy endpoint GET

Uruchom:

```powershell
curl.exe -i http://localhost:8080/api/topics
```

Oczekiwany kod:

```text
200 OK
```

Przykładowa odpowiedź:

```json
[
  {
    "id": 1,
    "name": "HTTP"
  }
]
```

---

# Krok 5 — Endpoint po ID

Sprawdź:

```powershell
curl.exe -i http://localhost:8080/api/topics/1
```

Następnie:

```powershell
curl.exe -i http://localhost:8080/api/topics/999
```

Porównaj:

- `200 OK`
- `404 Not Found`

## Pytanie

Dlaczego brak rekordu nie powinien kończyć się `500 Internal Server Error`?

---

# Krok 6 — POST

Dodaj nowy temat:

```powershell
curl.exe -i ^
  -X POST ^
  -H "Content-Type: application/json" ^
  -d "{\"name\":\"Docker\"}" ^
  http://localhost:8080/api/topics
```

W PowerShell możesz również użyć jednej linii:

```powershell
curl.exe -i -X POST -H "Content-Type: application/json" -d "{\"name\":\"Docker\"}" http://localhost:8080/api/topics
```

Oczekiwany wynik:

```text
201 Created
```

Zwróć uwagę na nagłówek:

```text
Location
```

---

# Krok 7 — Walidacja

Wyślij pustą nazwę:

```powershell
curl.exe -i -X POST -H "Content-Type: application/json" -d "{\"name\":\"\"}" http://localhost:8080/api/topics
```

Oczekiwany wynik:

```text
400 Bad Request
```

## Pytanie

Dlaczego błędne dane wejściowe są błędem klienta, a nie serwera?

---

# Krok 8 — Swagger / OpenAPI

Otwórz:

```text
http://localhost:8080/swagger
```

Przetestuj:
- GET `/api/topics`
- GET `/api/topics/{id}`
- POST `/api/topics`

## Pytanie

Czym różni się Swagger UI od samej specyfikacji OpenAPI?

---

# Krok 9 — Logging

Wykonaj kilka requestów.

Obserwuj logi:

```powershell
docker compose logs -f api
```

Endpointy zapisują informacje przez `ILogger`.

Znajdź w kodzie:

```csharp
logger.LogInformation(...)
```

## Pytanie

Dlaczego logowanie przez `ILogger` jest lepsze niż przypadkowe `Console.WriteLine()`?

---

# Krok 10 — Konfiguracja

API posiada endpoint:

```text
GET /api/info
```

Uruchom:

```powershell
curl.exe -i http://localhost:8080/api/info
```

Endpoint odczytuje nazwę kursu z konfiguracji:

```text
Course:Name
```

W Docker Compose ustawiamy ją przez zmienną środowiskową:

```yaml
environment:
  Course__Name: Technologie Internetowe
```

Podwójny znak podkreślenia:

```text
__
```

odpowiada separatorowi:

```text
:
```

w konfiguracji .NET.

---

# Krok 11 — Middleware

Znajdź w `Program.cs`:

```csharp
app.Use(async (context, next) =>
{
    var started = DateTime.UtcNow;

    await next();

    var elapsed = DateTime.UtcNow - started;

    app.Logger.LogInformation(
        "Request {Method} {Path} -> {StatusCode} in {ElapsedMs} ms",
        context.Request.Method,
        context.Request.Path,
        context.Response.StatusCode,
        elapsed.TotalMilliseconds);
});
```

To własny middleware.

## Pytania

1. Co oznacza `await next()`?
2. Co dzieje się przed `next()`?
3. Co dzieje się po `next()`?
4. Dlaczego middleware pasuje do logowania requestów?

---

# Krok 12 — Docker

Wróć do:

```powershell
cd labs/03-aspnetcore-api
```

Zbuduj i uruchom:

```powershell
docker compose up --build -d
```

Sprawdź:

```powershell
docker compose ps
```

API powinno być dostępne pod:

```text
http://localhost:8080
```

---

# Krok 13 — Obraz Docker

Otwórz:

```text
Dockerfile
```

Zwróć uwagę na dwa etapy:

```text
build
runtime
```

To multi-stage build.

## Pytania

1. Dlaczego SDK nie jest potrzebne w finalnym obrazie?
2. Dlaczego obraz runtime powinien być mniejszy?
3. Co daje `dotnet publish`?

---

# Krok 14 — Health endpoint

Sprawdź:

```powershell
curl.exe -i http://localhost:8080/health
```

Oczekiwany wynik:

```text
200 OK
```

Ten endpoint będzie później wykorzystywany przez Docker i reverse proxy.

---

# Weryfikacja

Lab jest zaliczony, jeśli potrafisz:

- uruchomić API lokalnie i w Dockerze,
- wskazać `Program.cs`,
- wyjaśnić DI,
- wykonać GET i POST,
- rozpoznać 200, 201, 400 i 404,
- uruchomić Swagger,
- znaleźć logi,
- wskazać middleware,
- odczytać konfigurację ze zmiennej środowiskowej,
- zbudować obraz Docker.

---

# Zadania dodatkowe

## Zadanie A — DELETE

Dodaj endpoint:

```text
DELETE /api/topics/{id}
```

Dla istniejącego elementu zwróć:

```text
204 No Content
```

Dla nieistniejącego:

```text
404 Not Found
```

## Zadanie B — PUT

Dodaj:

```text
PUT /api/topics/{id}
```

który aktualizuje nazwę tematu.

## Zadanie C — dodatkowa konfiguracja

Dodaj:

```text
Course:Edition
```

i zwróć ją z `/api/info`.

## Zadanie D — nagłówek response

Dodaj middleware, który ustawia:

```text
X-Course: TI
```

Sprawdź:

```powershell
curl.exe -I http://localhost:8080/api/topics
```

## Zadanie E — request id

W middleware odczytaj:

```csharp
context.TraceIdentifier
```

i dopisz go do logów.

---

# Cleanup

```powershell
docker compose down
```

---

# Co dalej?

W Lab 04 skupimy się na poprawnym projektowaniu REST API:

- DTO,
- walidacji,
- Problem Details,
- filtrowaniu,
- sortowaniu,
- paginacji,
- spójnych kodach HTTP.

Dopiero potem dołożymy bazę danych i Entity Framework Core.
