# Lab 10 — Projekt końcowy

## Cel

W projekcie końcowym student składa w jeden system wszystkie elementy poznane podczas kursu.

Projekt ma działać jako kompletna aplikacja webowa uruchamiana przez Docker Compose.

## Architektura

```text
Browser
   |
   | HTTPS
   v
Nginx
   |
   +--> frontend HTML/CSS/JS
   |
   +--> /api/
          |
          v
     ASP.NET Core Web API
          |
          +--> JWT / Authorization
          |
          +--> EF Core
                  |
                  v
             SQL Server
                  |
                  v
             Docker volume
```

## Wymagania funkcjonalne

Aplikacja musi posiadać:

- listę rekordów,
- dodawanie,
- edycję,
- usuwanie,
- walidację danych,
- komunikaty błędów,
- loading state,
- minimum jeden endpoint publiczny,
- minimum jeden endpoint chroniony,
- minimum jeden endpoint tylko dla roli Admin,
- logowanie użytkownika,
- JWT,
- persistent storage.

## Wymagania techniczne

Projekt musi używać:

- HTML5,
- CSS3,
- JavaScript,
- fetch(),
- ASP.NET Core Web API,
- Entity Framework Core,
- SQL Server,
- Dockerfile,
- Docker Compose,
- Nginx,
- HTTPS,
- security headers,
- healthcheck.

## Proponowany temat

Domyślnym projektem jest:

```text
Course Manager
```

System zarządza tematami kursu.

Student może jednak wybrać inny prosty temat, np.:
- biblioteka,
- filmy,
- zadania,
- notatki,
- produkty,
- wydarzenia.

## Role

Minimum dwie role:

```text
User
Admin
```

User:
- może przeglądać dane,
- może dodawać lub edytować dane.

Admin:
- może dodatkowo usuwać dane.

## Endpointy

Minimum:

```text
POST   /api/auth/login
GET    /api/profile

GET    /api/topics
GET    /api/topics/{id}
POST   /api/topics
PUT    /api/topics/{id}
DELETE /api/topics/{id}

GET    /health
```

## Kody HTTP

Projekt powinien poprawnie używać m.in.:

- 200 OK
- 201 Created
- 204 No Content
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 409 Conflict
- 500 Internal Server Error

## Baza danych

Tabela Topics powinna zawierać minimum:

```text
Id
Name
Description
CreatedAt
```

Wymagania:
- primary key,
- wymagane Name,
- limit długości,
- unique index na Name,
- migracje EF Core,
- seed danych.

## Frontend

Frontend powinien umożliwiać:
- logowanie,
- wylogowanie,
- pobranie danych,
- dodanie rekordu,
- edycję rekordu,
- usunięcie rekordu,
- wyświetlanie błędów,
- informację o stanie ładowania.

## Docker Compose

Całość uruchamiamy jednym poleceniem:

```powershell
docker compose up --build -d
```

Usługi:

```text
nginx
api
sqlserver
```

## Nginx

Nginx:
- serwuje frontend,
- proxy'uje /api/,
- proxy'uje /health,
- terminuję HTTPS,
- dodaje security headers,
- włącza gzip,
- cache'uje zasoby statyczne.

## Security

Minimum:
- JWT,
- role,
- authorization policy,
- HTTPS,
- CSP,
- X-Content-Type-Options,
- X-Frame-Options,
- Referrer-Policy,
- brak sekretów w repo.

## Konfiguracja

Sekrety przechowujemy lokalnie w:

```text
.env
```

Repo zawiera tylko:

```text
.env.example
```

## Healthcheck

SQL Server i API muszą posiadać healthcheck.

Sprawdzenie:

```powershell
docker compose ps
```

oraz:

```powershell
curl.exe -k https://localhost:8443/health
```

## Etapy wykonania

### Etap 1 — uruchom środowisko

```powershell
Copy-Item .env.example .env
docker compose up --build -d
```

### Etap 2 — sprawdź frontend

```text
https://localhost:8443
```

### Etap 3 — sprawdź API

```text
https://localhost:8443/api/topics
```

### Etap 4 — zaloguj użytkownika

```text
POST /api/auth/login
```

### Etap 5 — wykonaj CRUD

Sprawdź:
- CREATE,
- READ,
- UPDATE,
- DELETE.

### Etap 6 — sprawdź role

User powinien otrzymać 403 przy operacji tylko dla Admin.

### Etap 7 — sprawdź persistence

Dodaj dane:

```powershell
docker compose down
docker compose up -d
```

Dane powinny pozostać.

### Etap 8 — sprawdź security headers

```powershell
curl.exe -k -I https://localhost:8443/
```

### Etap 9 — sprawdź logi

```powershell
docker compose logs api
docker compose logs nginx
```

### Etap 10 — dokumentacja

README projektu powinien zawierać:
- opis,
- architekturę,
- sposób uruchomienia,
- endpointy,
- konta testowe bez haseł,
- opis ról,
- sposób cleanup.

## Checklista zaliczeniowa

- [ ] aplikacja startuje przez Docker Compose
- [ ] frontend działa przez HTTPS
- [ ] API działa przez Nginx
- [ ] SQL Server działa w osobnym kontenerze
- [ ] dane są trwałe
- [ ] migracje EF Core działają
- [ ] seed działa
- [ ] GET działa
- [ ] POST działa
- [ ] PUT działa
- [ ] DELETE działa
- [ ] walidacja działa
- [ ] Problem Details działa
- [ ] JWT działa
- [ ] 401 działa
- [ ] 403 działa
- [ ] role działają
- [ ] healthcheck działa
- [ ] security headers są ustawione
- [ ] gzip działa
- [ ] cache statycznych plików działa
- [ ] sekrety nie są w repo
- [ ] README jest kompletne

## Zadania rozszerzające

1. Dodaj paginację.
2. Dodaj wyszukiwanie.
3. Dodaj sortowanie.
4. Dodaj CreatedBy.
5. Dodaj optimistic concurrency.
6. Dodaj drugi zasób i relację 1:N.
7. Dodaj testy integracyjne.
8. Dodaj structured logging.
9. Dodaj correlation ID.
10. Dodaj drugi upstream API za Nginx.

## Cleanup

```powershell
docker compose down
```

Usunięcie danych:

```powershell
docker compose down -v
```

## Efekt końcowy

Po wykonaniu projektu student powinien rozumieć cały przepływ:

```text
DNS / HTTP / HTTPS
        ↓
Browser / JavaScript
        ↓
Nginx
        ↓
ASP.NET Core
        ↓
Authentication / Authorization
        ↓
EF Core
        ↓
SQL Server
        ↓
Docker
```
