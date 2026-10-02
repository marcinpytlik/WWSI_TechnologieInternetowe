# Lab 01 — HTTP, DNS i model klient–serwer

## Cel

W tym laboratorium poznasz praktycznie podstawy działania Web.

Po wykonaniu laba potrafisz:
- wyjaśnić model klient–serwer,
- rozróżnić URL, host, port, ścieżkę i query string,
- wykonać żądanie HTTP przez przeglądarkę i `curl`,
- odczytać metodę, kod statusu i nagłówki HTTP,
- rozpoznać podstawowe kody odpowiedzi,
- wyjaśnić rolę DNS,
- zobaczyć różnicę między nazwą hosta a adresem IP,
- uruchomić prosty serwer HTTP w Dockerze,
- obserwować ruch HTTP w DevTools.

## Architektura

```text
Browser / curl
     |
     | HTTP
     v
localhost:8080
     |
     v
Docker container
nginx:alpine
```

W tym labie nie używamy jeszcze ASP.NET Core.

Najpierw chcemy dokładnie zobaczyć, jak wygląda komunikacja HTTP.

## Wymagania

- Docker Desktop
- przeglądarka
- PowerShell lub terminal
- opcjonalnie Visual Studio Code + REST Client

Sprawdź:

```powershell
docker --version
docker compose version
curl.exe --version
```

## Struktura

```text
labs/01-http/
├── README.md
├── docker-compose.yml
├── nginx/
│   ├── default.conf
│   └── html/
│       ├── index.html
│       ├── about.html
│       └── data.json
└── requests/
    └── http-basics.http
```

---

# Krok 1 — Uruchom serwer HTTP

Przejdź do katalogu:

```powershell
cd labs/01-http
```

Uruchom środowisko:

```powershell
docker compose up -d
```

Sprawdź kontener:

```powershell
docker compose ps
```

Otwórz:

```text
http://localhost:8080
```

Powinna pojawić się strona „Technologie Internetowe — Lab 01”.

---

# Krok 2 — Pierwsze żądanie HTTP

W PowerShell:

```powershell
curl.exe -i http://localhost:8080/
```

Zwróć uwagę na:

- linię statusu,
- kod `200`,
- nagłówki,
- typ zawartości,
- body odpowiedzi.

Przykładowy początek:

```text
HTTP/1.1 200 OK
Server: nginx
Content-Type: text/html
```

## Pytania

1. Jaka metoda HTTP została użyta?
2. Jaki kod statusu zwrócił serwer?
3. Co oznacza `Content-Type`?
4. Co oznacza nagłówek `Server`?

---

# Krok 3 — HEAD

Wykonaj:

```powershell
curl.exe -I http://localhost:8080/
```

Metoda `HEAD` zwraca nagłówki bez body.

Porównaj wynik z:

```powershell
curl.exe -i http://localhost:8080/
```

---

# Krok 4 — Różne zasoby

Sprawdź:

```powershell
curl.exe -i http://localhost:8080/about.html
curl.exe -i http://localhost:8080/data.json
```

Porównaj nagłówek:

```text
Content-Type
```

dla HTML i JSON.

---

# Krok 5 — 404 Not Found

Wykonaj:

```powershell
curl.exe -i http://localhost:8080/nie-istnieje
```

Sprawdź:

- kod odpowiedzi,
- body odpowiedzi,
- nagłówki.

## Pytanie

Dlaczego klient otrzymał odpowiedź HTTP, mimo że zasób nie istnieje?

---

# Krok 6 — Query string

Otwórz:

```powershell
curl.exe -i "http://localhost:8080/?name=Jan&lang=pl"
```

W tym statycznym serwerze parametry nie zmieniają treści strony, ale są częścią URL.

Rozbij URL:

```text
http://localhost:8080/?name=Jan&lang=pl
```

na:
- scheme,
- host,
- port,
- path,
- query string.

---

# Krok 7 — Nagłówki requestu

Wyślij własny nagłówek:

```powershell
curl.exe -i -H "X-Lab: TI-01" http://localhost:8080/
```

Następnie użyj trybu verbose:

```powershell
curl.exe -v -H "X-Lab: TI-01" http://localhost:8080/
```

W trybie verbose:
- `>` oznacza request,
- `<` oznacza response.

Znajdź:

```text
> GET / HTTP/1.1
> Host: localhost:8080
> X-Lab: TI-01
```

---

# Krok 8 — DevTools

Otwórz w przeglądarce:

```text
http://localhost:8080
```

Naciśnij:

```text
F12
```

Przejdź do zakładki:

```text
Network
```

Odśwież stronę.

Dla requestu `/` znajdź:

- Request URL
- Request Method
- Status Code
- Remote Address
- Request Headers
- Response Headers
- Response

## Zadanie

Porównaj informacje z DevTools z wynikiem:

```powershell
curl.exe -v http://localhost:8080/
```

---

# Krok 9 — DNS

Sprawdź rozwiązywanie nazwy:

```powershell
Resolve-DnsName example.com
```

Alternatywnie:

```powershell
nslookup example.com
```

## Pytania

1. Co jest wynikiem zapytania DNS?
2. Czy jedna nazwa może wskazywać wiele adresów IP?
3. Czy HTTP i DNS to ten sam protokół?
4. Co następuje najpierw: DNS czy request HTTP?

---

# Krok 10 — localhost i adres IP

Sprawdź:

```powershell
ping localhost
```

oraz:

```powershell
curl.exe -i http://127.0.0.1:8080/
```

Porównaj z:

```powershell
curl.exe -i http://localhost:8080/
```

## Pytanie

Dlaczego oba adresy prowadzą do tego samego serwera?

---

# Krok 11 — Porty

Zmień w `docker-compose.yml`:

```yaml
ports:
  - "8080:80"
```

na:

```yaml
ports:
  - "8081:80"
```

Uruchom ponownie:

```powershell
docker compose down
docker compose up -d
```

Sprawdź:

```text
http://localhost:8081
```

## Pytanie

Który port należy do hosta, a który do kontenera?

---

# Krok 12 — Podstawowe kody HTTP

W tym labie widziałeś:

- `200 OK`
- `404 Not Found`

Zapamiętaj grupy:

| Zakres | Znaczenie |
|---|---|
| 1xx | informacyjne |
| 2xx | sukces |
| 3xx | przekierowania |
| 4xx | błąd po stronie klienta |
| 5xx | błąd po stronie serwera |

W kolejnych labach będziemy używać m.in.:

- 201 Created
- 204 No Content
- 400 Bad Request
- 401 Unauthorized
- 403 Forbidden
- 404 Not Found
- 409 Conflict
- 500 Internal Server Error

---

# Weryfikacja

Lab jest zaliczony, jeśli potrafisz:

- uruchomić kontener,
- wejść na `http://localhost:8080`,
- wykonać request GET,
- wykonać HEAD,
- rozpoznać 200 i 404,
- wskazać request headers,
- wskazać response headers,
- znaleźć request w DevTools,
- wyjaśnić rolę DNS,
- wyjaśnić mapowanie `8080:80`.

## Szybki test

Wykonaj:

```powershell
curl.exe -s -o NUL -w "%{http_code}" http://localhost:8080/
```

Oczekiwany wynik:

```text
200
```

Dla nieistniejącego zasobu:

```powershell
curl.exe -s -o NUL -w "%{http_code}" http://localhost:8080/not-found
```

Oczekiwany wynik:

```text
404
```

---

# Zadania dodatkowe

## Zadanie A

Dodaj plik:

```text
student.html
```

i sprawdź go przez `curl`.

## Zadanie B

Dodaj własny nagłówek odpowiedzi w konfiguracji Nginx:

```nginx
add_header X-Course "Technologie-Internetowe";
```

Sprawdź:

```powershell
curl.exe -I http://localhost:8080/
```

## Zadanie C

Skonfiguruj przekierowanie:

```text
/old
```

do:

```text
/about.html
```

i sprawdź kod `301` lub `302`.

## Zadanie D

Sprawdź:

```powershell
curl.exe -v http://localhost:8080/data.json
```

i wskaż dokładnie:
- początek requestu,
- początek response,
- status,
- Content-Type,
- body.

---

# Cleanup

Zatrzymaj środowisko:

```powershell
docker compose down
```

Sprawdź:

```powershell
docker compose ps
```

Po zakończeniu nie powinien działać żaden kontener z tego laboratorium.

---

# Co dalej?

W Lab 02 przejdziemy do HTML, CSS i JavaScript oraz pierwszych wywołań `fetch()`.

Tam przeglądarka przestanie być tylko klientem wyświetlającym statyczny HTML i zacznie aktywnie komunikować się z API.
