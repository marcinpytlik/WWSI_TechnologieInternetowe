# Lab 07 — Authentication, JWT i Authorization

## Cel

W tym laboratorium dodajemy uwierzytelnianie i autoryzację do aplikacji z poprzedniego laba.

Po wykonaniu laba potrafisz:
- wyjaśnić różnicę między authentication i authorization,
- zalogować użytkownika,
- wygenerować JWT,
- przesłać token w nagłówku Authorization,
- chronić endpointy przez `RequireAuthorization()`,
- używać claims i roles,
- rozróżnić 401 Unauthorized i 403 Forbidden,
- obsłużyć token po stronie frontendu,
- zabezpieczyć operacje modyfikujące dane.

## Architektura

```text
Browser
  |
  | POST /api/auth/login
  v
ASP.NET Core
  |
  | JWT
  v
Browser stores token
  |
  | Authorization: Bearer <token>
  v
Protected API
  |
  +--> User policy
  +--> Admin policy
```

## Uwaga dydaktyczna

W tym labie nie używamy jeszcze ASP.NET Core Identity.

Użytkownicy są zdefiniowani w prostym serwisie in-memory, żeby skupić się na:
- JWT,
- claims,
- roles,
- authorization policies,
- 401 i 403.

## Konta testowe

```text
student / Student123!
admin   / Admin123!
```

To wyłącznie dane laboratoryjne.

## Uruchomienie

```powershell
cd labs/07-auth-jwt
Copy-Item .env.example .env
docker compose up --build -d
```

Frontend:

```text
http://localhost:8080
```

Swagger:

```text
http://localhost:8081/swagger
```

## Krok 1 — Login

Wyślij:

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "student",
  "password": "Student123!"
}
```

Odpowiedź zawiera:
- token,
- datę wygaśnięcia,
- username,
- role.

## Krok 2 — Bearer token

Token przesyłamy w nagłówku:

```http
Authorization: Bearer eyJ...
```

## Krok 3 — Endpoint chroniony

Sprawdź bez tokenu:

```text
GET /api/profile
```

Oczekiwany wynik:

```text
401 Unauthorized
```

Po zalogowaniu endpoint zwraca dane z claims.

## Krok 4 — Role

Endpoint:

```text
DELETE /api/topics/{id}
```

jest dostępny tylko dla roli:

```text
Admin
```

Student powinien otrzymać:

```text
403 Forbidden
```

Admin:

```text
204 No Content
```

## Krok 5 — 401 vs 403

Zapamiętaj:

- 401 — klient nie jest uwierzytelniony,
- 403 — klient jest uwierzytelniony, ale nie ma uprawnień.

## Krok 6 — Frontend

Frontend:
- loguje użytkownika,
- przechowuje JWT w pamięci strony,
- dodaje nagłówek Authorization,
- pokazuje aktualnego użytkownika i rolę,
- ukrywa operację DELETE dla zwykłego użytkownika.

## Krok 7 — Claims

Token zawiera:
- Name,
- Role.

Endpoint `/api/profile` odczytuje je z `HttpContext.User`.

## Krok 8 — Policy

W `Program.cs` znajdź:

```csharp
options.AddPolicy("AdminOnly", policy =>
    policy.RequireRole("Admin"));
```

Następnie:

```csharp
.RequireAuthorization("AdminOnly")
```

## Krok 9 — DevTools

W Network znajdź request do chronionego endpointu.

Sprawdź:

```text
Authorization: Bearer ...
```

## Weryfikacja

Lab jest zaliczony, jeśli potrafisz:
- zalogować się,
- odebrać JWT,
- użyć tokenu,
- wyjaśnić claims,
- wyjaśnić role,
- rozróżnić 401 i 403,
- wykonać chroniony request,
- użyć policy.

## Zadania dodatkowe

1. Dodaj rolę `Instructor`.
2. Dodaj policy `CanEditTopics`.
3. Ustaw krótszy czas życia tokenu i sprawdź wygaśnięcie.
4. Dodaj endpoint `/api/admin/stats`.
5. Dodaj claim `course=TI`.

## Cleanup

```powershell
docker compose down
```

## Co dalej?

W Lab 08 zajmiemy się pełniej Dockerem i Docker Compose:
- build,
- networks,
- healthchecks,
- environment variables,
- dependency management,
- troubleshooting.
