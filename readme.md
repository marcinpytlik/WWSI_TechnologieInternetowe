# Technologie Internetowe — ASP.NET Core + Docker (40h)

Repozytorium materiałów laboratoryjnych do kursu **Technologie Internetowe**.

Nowa edycja kursu jest oparta o architekturę Docker-first i jeden spójny stos technologiczny:

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

Celem kursu nie jest nauka samego ASP.NET Core.

ASP.NET Core służy jako platforma do praktycznego pokazania:
- HTTP,
- REST API,
- komunikacji klient–serwer,
- pracy z bazą danych,
- bezpieczeństwa aplikacji webowych,
- reverse proxy,
- konteneryzacji,
- wdrażania aplikacji internetowych.

## Architektura docelowa

```text
Browser / JavaScript
        |
        | HTTP / HTTPS
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

Wszystkie usługi serwerowe uruchamiane są w kontenerach.

## Organizacja kursu

Kurs ma 40 godzin i składa się z 10 bloków po 4 godziny:

1. HTTP, DNS i model klient–serwer
2. HTML, CSS i JavaScript
3. ASP.NET Core Web API
4. REST i projektowanie API
5. EF Core + SQL Server w Dockerze
6. Frontend komunikujący się z własnym API
7. Authentication / Authorization / JWT
8. Docker i Docker Compose
9. Nginx, HTTPS, security i performance
10. Projekt końcowy

Szczegółowy plan:
- [docs/00-Plan_kursu.md](docs/00-Plan_kursu.md)

## Wymagane narzędzia

- Docker Desktop
- .NET SDK
- Visual Studio Code lub Visual Studio
- Git
- przeglądarka z DevTools

Opcjonalnie:
- REST Client
- Postman
- curl

Nie wymagamy lokalnej instalacji SQL Servera.

## Docelowa struktura repo

```text
docs/
labs/
  01-http/
  02-html-css-js/
  03-aspnetcore-api/
  04-rest-api/
  05-efcore-sqlserver/
  06-frontend-api/
  07-auth-jwt/
  08-docker-compose/
  09-nginx-security-performance/
  10-final-project/
src/
  frontend/
  api/
docker/
  nginx/
  sqlserver/
docker-compose.yml
```

Repozytorium jest obecnie przebudowywane z poprzedniej wersji opartej o Node.js + Express do nowej wersji opartej o ASP.NET Core.

## Zasada kursu

Jedna aplikacja rozwija się przez cały kurs.

Zaczynamy od protokołu HTTP i prostego frontendu, następnie budujemy API, dokładamy bazę danych, authentication, konteneryzację i reverse proxy.

Na końcu student uruchamia kompletny system poleceniem:

```bash
docker compose up --build
```

## Standard laboratoriów

Każdy lab powinien zawierać:

1. Cel
2. Architektura
3. Wymagania
4. Przygotowanie
5. Kroki krok po kroku
6. Weryfikacja
7. Zadania dodatkowe
8. Cleanup

Dzięki temu każdy moduł może być wykonany samodzielnie również po zajęciach.
