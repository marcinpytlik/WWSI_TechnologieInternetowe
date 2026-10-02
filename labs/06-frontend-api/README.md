# Lab 06 — Frontend + ASP.NET Core API + EF Core + SQL Server

## Cel

W tym laboratorium łączymy frontend z prawdziwym backendem i bazą danych.

Po wykonaniu laba potrafisz:
- uruchomić pełny stos przez Docker Compose,
- wykonać `fetch()` do własnego API,
- pobierać dane z ASP.NET Core,
- dodawać, edytować i usuwać dane z poziomu przeglądarki,
- obsłużyć loading / success / error state,
- skonfigurować CORS,
- analizować requesty w DevTools,
- wyjaśnić pełny przepływ Browser -> API -> EF Core -> SQL Server.

## Architektura

```text
Browser
  |
  | HTTP :8080
  v
Nginx frontend
  |
  | fetch()
  v
ASP.NET Core API :8081
  |
  | EF Core
  v
SQL Server 2022
  |
  v
Docker volume
```

## Struktura

```text
labs/06-frontend-api/
├── README.md
├── docker-compose.yml
├── .gitignore
├── .env.example
├── frontend/
│   ├── nginx.conf
│   └── html/
│       ├── index.html
│       ├── styles.css
│       └── app.js
├── api/
│   ├── Dockerfile
│   └── src/CourseApi/
│       ├── CourseApi.csproj
│       ├── Program.cs
│       ├── Data/
│       ├── Models/
│       └── Contracts/
└── requests/
    └── fullstack.http
```

## Przygotowanie

Utwórz lokalny plik `.env` na podstawie `.env.example` i ustaw hasło SQL Server.

## Uruchomienie

```powershell
docker compose up --build -d
docker compose ps
```

Frontend:

```text
http://localhost:8080
```

API:

```text
http://localhost:8081/swagger
```

## Krok 1 — Pobranie danych

Po otwarciu strony JavaScript wykonuje:

```javascript
fetch("http://localhost:8081/api/topics")
```

W DevTools sprawdź request `/api/topics`.

## Krok 2 — CORS

Frontend działa na porcie 8080, a API na 8081.

To różne origins.

API musi zezwolić na:

```text
http://localhost:8080
```

W `Program.cs` znajdź politykę CORS.

## Krok 3 — CREATE

W formularzu wpisz nazwę tematu i kliknij **Dodaj**.

Frontend wysyła:

```http
POST /api/topics
Content-Type: application/json
```

Po sukcesie lista jest odświeżana.

## Krok 4 — UPDATE

Kliknij **Edytuj**, zmień nazwę i zatwierdź.

Frontend wysyła:

```http
PUT /api/topics/{id}
```

## Krok 5 — DELETE

Kliknij **Usuń**.

Frontend wysyła:

```http
DELETE /api/topics/{id}
```

Po kodzie 204 element znika z listy po ponownym pobraniu danych.

## Krok 6 — Loading state

Podczas pobierania danych użytkownik widzi stan:

```text
Ładowanie...
```

## Krok 7 — Error state

Zatrzymaj API:

```powershell
docker compose stop api
```

Odśwież frontend.

Powinien pojawić się komunikat błędu.

Uruchom API:

```powershell
docker compose start api
```

## Krok 8 — DevTools

W zakładce Network przeanalizuj:
- GET,
- POST,
- PUT,
- DELETE,
- request payload,
- response payload,
- status codes,
- CORS headers.

## Weryfikacja

Lab jest zaliczony, jeśli:
- frontend pobiera dane z API,
- działa dodawanie,
- działa edycja,
- działa usuwanie,
- błędy są widoczne w UI,
- potrafisz wskazać requesty w DevTools,
- potrafisz wyjaśnić CORS,
- potrafisz prześledzić pełny przepływ do SQL Servera.

## Zadania dodatkowe

1. Dodaj filtrowanie listy po nazwie.
2. Dodaj przycisk sortowania A-Z / Z-A.
3. Dodaj paginację.
4. Wyświetl liczbę wszystkich rekordów z `X-Total-Count`.
5. Dodaj przycisk „Odśwież”.
6. Zablokuj przyciski podczas requestu.
7. Dodaj potwierdzenie przed DELETE.

## Cleanup

```powershell
docker compose down
```

Usunięcie danych:

```powershell
docker compose down -v
```

## Co dalej?

W Lab 07 dodamy authentication i authorization:

```text
Browser -> JWT -> ASP.NET Core -> protected endpoints
```
