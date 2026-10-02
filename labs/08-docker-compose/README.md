# Lab 08 — Docker i Docker Compose

## Cel

W tym laboratorium skupiamy się na uruchamianiu, diagnozowaniu i utrzymaniu aplikacji wielokontenerowej.

Po wykonaniu laba potrafisz:
- zbudować obraz aplikacji ASP.NET Core,
- uruchomić kontener ręcznie,
- używać Docker Compose,
- wyjaśnić mapowanie portów,
- pracować z volumes,
- wyjaśnić sieć Docker Compose,
- używać healthchecków,
- kontrolować kolejność startu usług,
- analizować logi,
- wykonywać polecenia wewnątrz kontenera,
- diagnozować najczęstsze problemy.

## Architektura

```text
Browser
  |
  | :8080
  v
Nginx
  |
  | Docker network
  v
ASP.NET Core API
  |
  | Docker network
  v
SQL Server
  |
  v
Docker volume
```

## Struktura

```text
labs/08-docker-compose/
├── README.md
├── .env.example
├── .gitignore
├── docker-compose.yml
├── frontend/
│   ├── nginx.conf
│   └── html/
│       └── index.html
└── api/
    ├── Dockerfile
    └── src/
        └── CourseApi/
```

---

# Krok 1 — Sprawdzenie środowiska

```powershell
docker --version
docker compose version
docker info
```

## Pytania

1. Czym różni się obraz od kontenera?
2. Co oznacza warstwa obrazu?
3. Czy usunięcie kontenera usuwa obraz?

---

# Krok 2 — Budowanie obrazu

Przejdź do:

```powershell
cd labs/08-docker-compose
```

Zbuduj obraz API:

```powershell
docker build -t ti-lab08-api:1.0 .\api
```

Sprawdź:

```powershell
docker image ls
```

## Pytanie

Dlaczego w Dockerfile używamy multi-stage build?

---

# Krok 3 — Uruchomienie kontenera ręcznie

Uruchom API bez Compose:

```powershell
docker run --rm -p 8081:8080 ti-lab08-api:1.0
```

Jeśli aplikacja wymaga innych usług, zobaczysz błąd połączenia.

To celowe.

## Wniosek

Sam kontener API nie wystarcza, jeśli aplikacja zależy od bazy danych.

---

# Krok 4 — Docker Compose

Uruchom cały stos:

```powershell
docker compose up --build -d
```

Sprawdź:

```powershell
docker compose ps
```

Powinny działać:

```text
ti-lab08-frontend
ti-lab08-api
ti-lab08-sql
```

---

# Krok 5 — Port mapping

W Compose znajdź:

```yaml
ports:
  - "8080:80"
```

oraz:

```yaml
ports:
  - "8081:8080"
```

Pierwszy port należy do hosta.

Drugi do kontenera.

## Pytanie

Czy API może działać bez wystawiania portu na hosta?

---

# Krok 6 — Sieć Docker Compose

Wykonaj:

```powershell
docker network ls
```

Znajdź sieć utworzoną przez Compose.

Następnie:

```powershell
docker inspect ti-lab08-api
```

Sprawdź sekcję Networks.

## Ważne

Kontenery w jednej sieci mogą komunikować się po nazwach usług:

```text
api
sqlserver
frontend
```

Dlatego API łączy się do:

```text
Server=sqlserver,1433
```

a nie:

```text
Server=localhost
```

---

# Krok 7 — DNS wewnątrz Dockera

Wejdź do kontenera API:

```powershell
docker exec -it ti-lab08-api sh
```

Spróbuj:

```sh
getent hosts sqlserver
```

lub:

```sh
cat /etc/hosts
```

## Pytanie

Kto rozwiązuje nazwę `sqlserver`?

---

# Krok 8 — Volumes

Sprawdź:

```powershell
docker volume ls
```

Znajdź volume:

```text
lab08-sql-data
```

Podejrzyj:

```powershell
docker volume inspect lab08-sql-data
```

## Test trwałości

Dodaj dane.

Następnie:

```powershell
docker compose down
docker compose up -d
```

Dane powinny pozostać.

Potem:

```powershell
docker compose down -v
```

Dane zostaną usunięte.

---

# Krok 9 — Healthcheck

Sprawdź:

```powershell
docker compose ps
```

SQL Server powinien osiągnąć:

```text
healthy
```

API zależy od jego gotowości.

## Pytanie

Dlaczego running nie zawsze oznacza ready?

---

# Krok 10 — depends_on

W Compose znajdź:

```yaml
depends_on:
  sqlserver:
    condition: service_healthy
```

## Pytanie

Jaka jest różnica między:
- kolejnością uruchomienia,
- a faktyczną gotowością usługi?

---

# Krok 11 — Logi

Wszystkie logi:

```powershell
docker compose logs
```

API:

```powershell
docker compose logs api
```

Śledzenie:

```powershell
docker compose logs -f api
```

Ostatnie 50 linii:

```powershell
docker compose logs --tail 50 api
```

---

# Krok 12 — Restart policy

Dodaj do API:

```yaml
restart: unless-stopped
```

Zatrzymaj proces lub zrestartuj Docker Desktop i obserwuj zachowanie.

---

# Krok 13 — Zmienne środowiskowe

Sprawdź:

```powershell
docker exec ti-lab08-api printenv
```

Nie umieszczaj sekretów bezpośrednio w repozytorium.

## Pytanie

Dlaczego `.env` jest w `.gitignore`?

---

# Krok 14 — Troubleshooting: konflikt portu

Uruchom inną aplikację na porcie 8080.

Następnie:

```powershell
docker compose up
```

Obserwuj błąd.

Rozwiązania:
- zatrzymać proces zajmujący port,
- zmienić port hosta.

---

# Krok 15 — Troubleshooting: błędny hostname

Celowo zmień connection string:

```text
Server=wrongname,1433
```

Uruchom:

```powershell
docker compose up
```

Sprawdź logi API.

Przywróć:

```text
Server=sqlserver,1433
```

---

# Krok 16 — Troubleshooting: usunięty volume

Wykonaj:

```powershell
docker compose down -v
```

Uruchom ponownie.

Sprawdź, czy baza została utworzona od nowa.

---

# Krok 17 — Inspect

Przećwicz:

```powershell
docker inspect ti-lab08-api
docker inspect ti-lab08-sql
```

Znajdź:
- IP address,
- mounts,
- environment,
- network,
- health status.

---

# Krok 18 — Stats

Uruchom:

```powershell
docker stats
```

Obserwuj:
- CPU,
- RAM,
- network I/O,
- block I/O.

---

# Krok 19 — Compose config

Wykonaj:

```powershell
docker compose config
```

To pokazuje finalną konfigurację po rozwinięciu zmiennych.

---

# Weryfikacja

Lab jest zaliczony, jeśli potrafisz:
- zbudować obraz,
- uruchomić kontener,
- uruchomić cały stos Compose,
- wyjaśnić port mapping,
- wyjaśnić Docker network,
- wyjaśnić volume,
- wyjaśnić healthcheck,
- znaleźć problem w logach,
- wejść do kontenera,
- użyć inspect i stats.

---

# Zadania dodatkowe

## Zadanie A — osobna sieć backendowa

Utwórz:

```yaml
networks:
  frontend-net:
  backend-net:
```

Frontend nie powinien mieć dostępu bezpośrednio do SQL Servera.

## Zadanie B — limit zasobów

Dodaj limit pamięci dla API.

## Zadanie C — read-only filesystem

Sprawdź możliwość uruchomienia frontendu z read-only filesystem.

## Zadanie D — profiles

Dodaj profil Compose:

```text
debug
```

dla dodatkowego narzędzia diagnostycznego.

## Zadanie E — skalowanie

Spróbuj:

```powershell
docker compose up --scale api=2
```

Zastanów się, dlaczego mapowanie stałego portu może wtedy przeszkadzać.

---

# Cleanup

```powershell
docker compose down
```

Usunięcie danych:

```powershell
docker compose down -v
```

Usunięcie obrazu:

```powershell
docker image rm ti-lab08-api:1.0
```

---

# Co dalej?

W Lab 09 ustawimy Nginx jako prawdziwy reverse proxy przed API i zajmiemy się:
- HTTPS,
- security headers,
- compression,
- caching,
- HTTP/2,
- rate limiting,
- health endpoints.
