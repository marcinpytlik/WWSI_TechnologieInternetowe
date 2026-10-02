# Plan kursu — Technologie Internetowe (40h)

## Założenie kursu

Kurs pokazuje technologie internetowe na przykładzie kompletnej aplikacji webowej uruchamianej w kontenerach.

Stos technologiczny:
- HTML5, CSS3, JavaScript
- HTTP/HTTPS
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server
- Docker
- Docker Compose
- Nginx jako reverse proxy
- Swagger / OpenAPI
- JWT
- podstawy observability, security i performance

ASP.NET Core jest platformą demonstracyjną. Głównym celem kursu pozostaje zrozumienie komunikacji webowej, REST API, bezpieczeństwa, wydajności i wdrażania aplikacji internetowych.

## Środowisko

Student potrzebuje:
- Docker Desktop
- .NET SDK
- Visual Studio Code lub Visual Studio
- Git
- przeglądarkę z DevTools
- opcjonalnie REST Client / Postman

SQL Server nie jest instalowany lokalnie — działa w kontenerze.

## Organizacja — 10 bloków po 4h

### Blok 1 — Jak działa Web: HTTP, DNS, klient–serwer
- model klient–serwer
- URI / URL
- DNS
- HTTP request / response
- metody HTTP
- kody statusu
- nagłówki
- cookies
- cache
- DevTools
- curl

Lab:
- analiza rzeczywistych żądań HTTP
- uruchomienie pierwszego kontenera HTTP
- obserwacja nagłówków i kodów odpowiedzi

### Blok 2 — HTML, CSS i JavaScript w przeglądarce
- semantyczny HTML5
- formularze
- CSS
- responsive design
- DOM
- zdarzenia
- moduły JavaScript
- fetch
- JSON
- async/await

Lab:
- prosty frontend komunikujący się z publicznym API

### Blok 3 — ASP.NET Core Web API
- struktura projektu
- Program.cs
- middleware pipeline
- routing
- kontrolery
- Minimal API vs Controllers
- Dependency Injection
- konfiguracja
- logging
- Swagger / OpenAPI

Lab:
- pierwsze API ASP.NET Core
- endpointy GET / POST
- testowanie przez curl i Swagger

### Blok 4 — REST API i projektowanie kontraktu
- zasady REST
- resource-oriented API
- DTO
- walidacja
- model binding
- statusy HTTP
- Problem Details
- filtrowanie
- sortowanie
- paginacja
- wersjonowanie API

Lab:
- CRUD dla wybranego zasobu

### Blok 5 — Entity Framework Core + SQL Server w Dockerze
- kontener SQL Server
- connection string
- DbContext
- encje
- relacje
- migrations
- seed danych
- LINQ
- async database access
- podstawy indeksów i transakcji

Lab:
- API + EF Core + SQL Server
- migracja bazy
- Docker Compose dla API i DB

### Blok 6 — Frontend + własne API
- fetch do własnego backendu
- obsługa błędów
- formularze
- CORS
- serializacja JSON
- loading / empty / error state
- podstawy architektury SPA bez frameworka

Lab:
- frontend HTML/CSS/JS konsumujący własne ASP.NET Core API

### Blok 7 — Authentication i Authorization
- authentication vs authorization
- JWT
- claims
- role / policy based authorization
- bezpieczne przechowywanie sekretów
- hasła i hashing — zasady
- cookies a tokeny
- podstawowe zagrożenia webowe

Lab:
- logowanie
- JWT
- endpoint chroniony
- role lub policy

### Blok 8 — Docker i Docker Compose
- obraz a kontener
- Dockerfile
- multi-stage build
- ports
- volumes
- networks
- environment variables
- healthcheck
- depends_on
- persistent storage

Lab:
- konteneryzacja ASP.NET Core API
- SQL Server jako osobny kontener
- uruchomienie całego środowiska jednym poleceniem

### Blok 9 — Reverse proxy, HTTPS, security i performance
- Nginx
- reverse proxy
- TLS / HTTPS
- CORS
- security headers
- compression
- caching
- ETag
- HTTP/2
- HTTP/3 — omówienie
- podstawy rate limiting
- health endpoints

Lab:
- Nginx przed API
- HTTPS / proxy
- nagłówki bezpieczeństwa
- pomiary w DevTools i curl

### Blok 10 — Projekt końcowy
Student buduje kompletną aplikację:

```text
Browser
   |
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

Całość działa przez Docker Compose.

Wymagania projektu:
- frontend HTML/CSS/JavaScript
- REST API
- ASP.NET Core
- EF Core
- SQL Server
- CRUD
- walidacja
- poprawne kody HTTP
- obsługa błędów
- minimum jeden chroniony endpoint
- JWT
- Dockerfile
- Docker Compose
- Nginx
- README z instrukcją uruchomienia
- healthcheck
- testy endpointów przez curl / REST Client

## Efekty uczenia

Po kursie student:
- rozumie HTTP i model działania aplikacji webowej,
- potrafi analizować ruch w DevTools,
- buduje frontend korzystający z API,
- tworzy REST API w ASP.NET Core,
- korzysta z EF Core i relacyjnej bazy danych,
- potrafi uruchomić SQL Server w Dockerze,
- konteneryzuje aplikację ASP.NET Core,
- łączy kilka usług przez Docker Compose,
- rozumie podstawy JWT, CORS, TLS i security headers,
- rozumie rolę reverse proxy,
- potrafi uruchomić kompletną aplikację webową bez lokalnej instalacji serwera bazy danych.

## Zasady pracy

- Każdy lab ma instrukcję krok po kroku.
- Każdy lab kończy się sekcją weryfikacji.
- Wszystkie usługi serwerowe uruchamiamy w Dockerze.
- Kod ma być możliwy do uruchomienia od zera na podstawie README.
- Preferujemy rozwiązania proste i czytelne.
- Jedna aplikacja jest rozwijana stopniowo przez kolejne bloki kursu.
