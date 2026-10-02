# Lab 09 — Nginx, HTTPS, security i performance

## Cel

W tym laboratorium stawiamy Nginx jako prawdziwy punkt wejścia do aplikacji i dodajemy podstawowe mechanizmy bezpieczeństwa oraz optymalizacji ruchu HTTP.

Po wykonaniu laba potrafisz:
- skonfigurować Nginx jako reverse proxy,
- terminować HTTPS na Nginx,
- przekazywać ruch do ASP.NET Core,
- ustawiać security headers,
- włączyć kompresję gzip,
- skonfigurować cache dla statycznych zasobów,
- dodać rate limiting,
- wyjaśnić rolę nagłówków X-Forwarded-*,
- analizować HTTP/2,
- sprawdzić health endpoint,
- diagnozować konfigurację Nginx.

## Architektura

```text
Browser
   |
   | HTTPS :8443
   v
Nginx
   |
   | HTTP
   v
ASP.NET Core API
   |
   v
SQL Server
```

Nginx:
- serwuje frontend,
- terminuję TLS,
- proxy'uje `/api/`,
- dodaje nagłówki bezpieczeństwa,
- kompresuje odpowiedzi,
- cache'uje statyczne zasoby,
- ogranicza liczbę requestów.

## Struktura

```text
labs/09-nginx-security-performance/
├── README.md
├── .env.example
├── .gitignore
├── docker-compose.yml
├── nginx/
│   ├── default.conf
│   ├── certs/
│   │   └── README.md
│   └── html/
│       ├── index.html
│       ├── app.js
│       └── styles.css
└── api/
    ├── Dockerfile
    └── src/CourseApi/
```

## Przygotowanie

Utwórz plik:

```powershell
Copy-Item .env.example .env
```

Ustaw hasło SQL Servera.

Następnie wygeneruj lokalny certyfikat zgodnie z instrukcją w:

```text
nginx/certs/README.md
```

## Uruchomienie

```powershell
docker compose up --build -d
docker compose ps
```

HTTP:

```text
http://localhost:8080
```

HTTPS:

```text
https://localhost:8443
```

## Krok 1 — Reverse proxy

Nginx proxy'uje:

```text
/api/
```

do:

```text
http://api:8080
```

Sprawdź:

```powershell
curl.exe -k -i https://localhost:8443/api/topics
```

## Krok 2 — X-Forwarded-*

W konfiguracji Nginx znajdź:

```nginx
proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
proxy_set_header X-Forwarded-Proto $scheme;
proxy_set_header X-Forwarded-Host $host;
```

## Pytanie

Dlaczego backend powinien wiedzieć, że klient łączył się po HTTPS, skoro ruch Nginx -> API jest HTTP?

## Krok 3 — HTTPS

Sprawdź:

```powershell
curl.exe -k -I https://localhost:8443/
```

Opcja `-k` jest używana tylko dlatego, że certyfikat laboratoryjny jest lokalny.

## Krok 4 — HTTP -> HTTPS

Wejdź na:

```text
http://localhost:8080
```

Nginx powinien przekierować ruch na HTTPS.

## Krok 5 — Security headers

Sprawdź:

```powershell
curl.exe -k -I https://localhost:8443/
```

Znajdź:
- `X-Content-Type-Options`
- `X-Frame-Options`
- `Referrer-Policy`
- `Content-Security-Policy`

## Krok 6 — Content-Security-Policy

W tym labie CSP pozwala tylko na zasoby z tego samego origin.

Przykład:

```text
default-src 'self'
```

## Pytanie

Jak CSP ogranicza skutki XSS?

## Krok 7 — gzip

Sprawdź:

```powershell
curl.exe -k -H "Accept-Encoding: gzip" -I https://localhost:8443/app.js
```

Szukaj:

```text
Content-Encoding: gzip
```

## Krok 8 — Cache statycznych plików

Dla CSS i JS ustawiamy:

```text
Cache-Control
Expires
```

Sprawdź:

```powershell
curl.exe -k -I https://localhost:8443/styles.css
```

## Krok 9 — no-cache dla HTML

HTML nie powinien być agresywnie cache'owany.

Sprawdź:

```powershell
curl.exe -k -I https://localhost:8443/
```

## Krok 10 — Rate limiting

Konfiguracja Nginx posiada strefę:

```nginx
limit_req_zone
```

i ogranicza ruch do API.

Wykonaj serię requestów i obserwuj odpowiedzi.

## Krok 11 — Health endpoint

Sprawdź:

```powershell
curl.exe -k -i https://localhost:8443/health
```

## Krok 12 — Test konfiguracji Nginx

```powershell
docker exec ti-lab09-nginx nginx -t
```

## Krok 13 — Logi

```powershell
docker compose logs nginx
docker compose logs api
```

## Krok 14 — HTTP/2

Sprawdź w DevTools protokół użyty dla requestu HTTPS.

Możesz też użyć:

```powershell
curl.exe -k -I --http2 https://localhost:8443/
```

## Krok 15 — Cache busting

Zmień nazwę pliku:

```text
app.js
```

na:

```text
app.v2.js
```

i zaktualizuj HTML.

## Pytanie

Dlaczego zmiana nazwy pliku rozwiązuje problem długiego cache?

## Weryfikacja

Lab jest zaliczony, jeśli potrafisz:
- wyjaśnić reverse proxy,
- uruchomić HTTPS,
- znaleźć security headers,
- wykazać gzip,
- pokazać cache-control,
- wyjaśnić rate limiting,
- sprawdzić HTTP/2,
- użyć `nginx -t`,
- przeanalizować logi.

## Zadania dodatkowe

1. Dodaj nagłówek `Permissions-Policy`.
2. Zmień rate limit i porównaj zachowanie.
3. Dodaj osobny cache dla obrazów.
4. Wyłącz jeden security header i sprawdź różnicę.
5. Dodaj drugi upstream API.

## Cleanup

```powershell
docker compose down
```

Usunięcie danych:

```powershell
docker compose down -v
```

## Co dalej?

Lab 10 będzie projektem końcowym, w którym student składa wszystkie elementy:
- frontend,
- REST API,
- EF Core,
- SQL Server,
- JWT,
- Docker Compose,
- Nginx,
- HTTPS,
- security,
- healthcheck.
