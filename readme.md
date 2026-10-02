# Technologie Internetowe — ASP.NET Core + Docker (40h)

Repozytorium materiałów do kursu **Technologie Internetowe**.

Kurs jest zorganizowany jako 10 bloków po 4 godziny i oparty o jeden spójny stos technologiczny:

- HTML5
- CSS3
- JavaScript
- HTTP / HTTPS
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Docker
- Docker Compose
- Nginx
- Swagger / OpenAPI
- JWT

## Cel kursu

Celem kursu jest praktyczne zrozumienie technologii internetowych poprzez budowę kompletnej aplikacji webowej.

ASP.NET Core jest platformą wykorzystywaną do pokazania:
- HTTP i REST,
- komunikacji klient–serwer,
- pracy z bazą danych,
- bezpieczeństwa aplikacji webowych,
- konteneryzacji,
- reverse proxy,
- podstaw wdrażania i diagnostyki.

## Architektura końcowa

```text
Browser / JavaScript
        |
        | HTTPS
        v
      Nginx
        |
        v
ASP.NET Core Web API
        |
        v
Entity Framework Core
        |
        v
SQL Server
```

## Plan 40h

| Blok | Temat |
|---|---|
| 01 | HTTP, DNS i model klient–serwer |
| 02 | HTML, CSS i JavaScript |
| 03 | ASP.NET Core Web API |
| 04 | REST i projektowanie API |
| 05 | EF Core + SQL Server w Dockerze |
| 06 | Frontend + własne API |
| 07 | Authentication / Authorization / JWT |
| 08 | Docker i Docker Compose |
| 09 | Nginx, HTTPS, security i performance |
| 10 | Projekt końcowy |

Szczegółowy plan:
- [docs/00-Plan_kursu.md](docs/00-Plan_kursu.md)

Architektura:
- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)

## Laboratoria

Każdy blok ma osobny katalog:

```text
labs/
├── 01-http/
├── 02-html-css-js/
├── 03-aspnetcore-api/
├── 04-rest-api/
├── 05-efcore-sqlserver/
├── 06-frontend-api/
├── 07-auth-jwt/
├── 08-docker-compose/
├── 09-nginx-security-performance/
└── 10-final-project/
```

Mapa laboratoriów:
- [labs/README.md](labs/README.md)

## Materiały pomocnicze

`docs/` zawiera materiały teoretyczne i plan kursu.

`cheat-sheets/` zawiera krótkie materiały referencyjne do HTTP, HTML, CSS, JavaScript i Git.

## Wymagane narzędzia

- Docker Desktop
- .NET SDK 10
- Visual Studio Code lub Visual Studio
- Git
- przeglądarka z DevTools

Opcjonalnie:
- REST Client
- Postman
- curl

Nie wymagamy lokalnej instalacji SQL Servera.

## Zasada pracy

Każdy kolejny lab rozwija poprzednie zagadnienia.

Ścieżka kursu:

```text
HTTP
  -> HTML/CSS/JavaScript
  -> ASP.NET Core
  -> REST
  -> EF Core + SQL Server
  -> full-stack
  -> JWT
  -> Docker Compose
  -> Nginx + HTTPS
  -> projekt końcowy
```

## Standard laboratoriów

Laboratoria zawierają, zależnie od tematu:
- cel,
- architekturę,
- wymagania,
- przygotowanie,
- instrukcję krok po kroku,
- weryfikację,
- zadania dodatkowe,
- cleanup,
- gotowe requesty `.http`,
- pliki Docker / Compose / Nginx.

## Uruchamianie

Każdy lab posiada własny README.

Dla labów wielokontenerowych standardem jest:

```powershell
docker compose up --build -d
```

Po zakończeniu:

```powershell
docker compose down
```

Jeżeli chcesz również usunąć dane zapisane w volumes:

```powershell
docker compose down -v
```

## Bezpieczeństwo

Sekrety nie są commitowane do repo.

Jeśli dany lab korzysta z lokalnej konfiguracji:
1. skopiuj `.env.example` do `.env`,
2. ustaw własne wartości,
3. nie commituj pliku `.env`.
