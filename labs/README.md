# Laboratoria

Kurs składa się z 10 laboratoriów odpowiadających 10 blokom po 4 godziny.

| Lab | Temat | Główny rezultat |
|---|---|---|
| [01](01-http/) | HTTP i DNS | request/response, statusy, nagłówki, pierwszy kontener |
| [02](02-html-css-js/) | HTML/CSS/JS | DOM, zdarzenia, fetch i JSON |
| [03](03-aspnetcore-api/) | ASP.NET Core API | endpointy, DI, middleware, Swagger |
| [04](04-rest-api/) | REST API | DTO, CRUD, Problem Details, paginacja |
| [05](05-efcore-sqlserver/) | EF Core + SQL Server | trwała baza danych i migracje |
| [06](06-frontend-api/) | Frontend + API | pełny przepływ klient–serwer |
| [07](07-auth-jwt/) | JWT | authentication, authorization, claims i role |
| [08](08-docker-compose/) | Docker Compose | sieci, volumes, healthchecki i troubleshooting |
| [09](09-nginx-security-performance/) | Nginx + security | reverse proxy, HTTPS, cache i rate limiting |
| [10](10-final-project/) | Projekt końcowy | kompletna aplikacja kontenerowa |

## Progresja

```text
01 HTTP
   ↓
02 Browser + JavaScript
   ↓
03 ASP.NET Core
   ↓
04 REST API
   ↓
05 EF Core + SQL Server
   ↓
06 Full-stack
   ↓
07 JWT
   ↓
08 Docker Compose
   ↓
09 Nginx + HTTPS
   ↓
10 Final Project
```

## Standard

Każdy lab zawiera instrukcję pozwalającą wykonać ćwiczenie od początku do końca.

W zależności od tematu znajdziesz w nim:
- `README.md`,
- kod aplikacji,
- `Dockerfile`,
- `docker-compose.yml`,
- konfigurację Nginx,
- pliki `.http`,
- `.env.example`.

## Sekrety

Pliki `.env`, klucze prywatne i certyfikaty generowane lokalnie nie są commitowane do repo.

## Kolejność

Najlepiej wykonywać laboratoria kolejno. Każdy następny blok zakłada znajomość pojęć wprowadzonych wcześniej.
