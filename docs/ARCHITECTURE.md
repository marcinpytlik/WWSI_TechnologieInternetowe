# Architektura kursu — ASP.NET Core + Docker

## Cel

W trakcie kursu rozwijana jest jedna aplikacja webowa. Każdy kolejny blok dodaje nowy element architektury.

## Stan końcowy

```text
+-----------------------+
| Browser               |
| HTML / CSS / JS       |
+-----------+-----------+
            |
            | HTTPS
            v
+-----------------------+
| Nginx                 |
| Reverse Proxy         |
+-----------+-----------+
            |
            | HTTP
            v
+-----------------------+
| ASP.NET Core Web API  |
| REST / JWT / Logging  |
+-----------+-----------+
            |
            | EF Core
            v
+-----------------------+
| SQL Server            |
| persistent volume     |
+-----------------------+
```

## Kontenery

Docelowo środowisko składa się z:
- `frontend` — statyczny frontend lub pliki serwowane przez Nginx,
- `nginx` — reverse proxy i punkt wejścia do aplikacji,
- `api` — ASP.NET Core Web API,
- `sqlserver` — SQL Server,
- opcjonalnie dodatkowych kontenerów wykorzystywanych w ćwiczeniach diagnostycznych.

## Sieć

Kontenery komunikują się po prywatnej sieci Docker Compose.

Przeglądarka nie łączy się bezpośrednio z SQL Serverem.

## Konfiguracja

Konfiguracja środowiska nie może być zakodowana na stałe.

Wykorzystujemy:
- `appsettings.json`,
- `appsettings.Development.json`,
- zmienne środowiskowe,
- Docker Compose,
- User Secrets podczas lokalnego developmentu, jeśli są potrzebne.

Sekrety nie są commitowane do repozytorium.

## Dane

SQL Server korzysta z persistent volume, dzięki czemu restart kontenera nie usuwa danych.

Migracje EF Core tworzą i aktualizują schemat aplikacji.

## Obserwowalność

Od początku kursu API wykorzystuje standardowy logging ASP.NET Core.

W późniejszych laboratoriach dodajemy:
- structured logging,
- health checks,
- correlation / request id,
- analizę requestów w DevTools i curl.

## Bezpieczeństwo

Kurs pokazuje praktycznie:
- HTTPS,
- CORS,
- JWT,
- authorization policies,
- validation,
- Problem Details,
- security headers,
- bezpieczne przekazywanie konfiguracji,
- podstawy rate limiting.

## Zasada progresji

Każdy lab powinien bazować na poprzednim lub jasno wskazywać stan początkowy.

Nie tworzymy dziesięciu niezależnych aplikacji. Budujemy jeden system krok po kroku.
