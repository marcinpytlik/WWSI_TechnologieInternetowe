# HTTP — podstawy i praktyka

## Co to jest HTTP?
Protokół żądanie/odpowiedź w modelu klient–serwer. Klient (przeglądarka, curl) wysyła **request**, serwer zwraca **response**.

---

## Anatomia żądania
```
GET /api/hello HTTP/1.1
Host: example.com
Accept: application/json
Authorization: Bearer <token>
```
- **Metoda**: `GET|POST|PUT|PATCH|DELETE|HEAD|OPTIONS`
- **Ścieżka + query**: `/api/todos?page=2&size=10`
- **Nagłówki**: metadane (format, auth, cache)
- **Body** (opcjonalnie): np. JSON przy POST/PUT

## Kody statusu (esencja)
- **2xx**: sukces (`200 OK`, `201 Created`)
- **3xx**: przekierowania (`301`, `302`, `307`)
- **4xx**: błąd klienta (`400`, `401`, `403`, `404`, `409`, `422`)
- **5xx**: błąd serwera (`500`, `502`, `503`)

## Najważniejsze nagłówki
- **Content-Type** / **Accept** — format treści (np. `application/json`)
- **Cache-Control** — `no-store`, `no-cache`, `max-age=...`
- **ETag** + `If-None-Match` → `304 Not Modified`
- **Location** (dla `201 Created` i 3xx)
- **CORS**: `Access-Control-Allow-*` (dla przeglądarek)

---

## Cache — jak myśleć
- **`max-age=...`** — pozwól klientowi nie pytać przez X sekund.
- **ETag/If-None-Match** — szybkie „czy zmieniło się?” → `304` bez body.
- **`no-store`** — nic nie zapisuj (np. dane wrażliwe).  
Uwaga: „`no-cache` ≠ brak cache”; to „waliduj przed użyciem”.

## CORS — minimalny przewodnik
Przeglądarka wymaga zgody serwera na cross‑origin:
- `Access-Control-Allow-Origin: https://app.example`
- `Access-Control-Allow-Credentials: true`
- `Access-Control-Allow-Methods: GET,POST,PUT,PATCH,DELETE,OPTIONS`
Preflight (`OPTIONS`) poprzedza metody modyfikujące i niestandardowe nagłówki.

---

## Narzędzia
- **DevTools → Network**: podgląd request/response, cache, CORS.
- **curl**: `curl -i -X POST -H "Content-Type: application/json" -d '{"x":1}' http://localhost:4000/api/echo`
- **HTTP clients**: VS Code REST Client, Postman/Insomnia.

## Dobre praktyki
- Zwracaj sensowne statusy + treść błędu (`{ "error": "..." }`).
- Dla `POST` twórz `201 Created` + `Location: /api/zasób/{id}`.
- Ustal spójne zasady **paginacji** i **sortowania** w API.
