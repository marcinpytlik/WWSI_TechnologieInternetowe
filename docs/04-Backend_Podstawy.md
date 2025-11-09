# Backend — REST, walidacja, bezpieczeństwo

## Minimalny serwer HTTP
- Routing: rozpoznawanie ścieżki i metody.
- JSON: `Content-Type: application/json; charset=utf-8`.
- Obsługa błędów: zawsze zwracaj opis `{ "error": "..." }`.

**Kontrakt REST (przykład `todos`):**
- `GET /api/todos?page=1&size=10&sort=createdAt:desc`
- `POST /api/todos {title}` → `201 Created` + `Location`
- `PATCH /api/todos/{id}` — modyfikacja częściowa
- `DELETE /api/todos/{id}` — `204 No Content`

## Walidacja i statusy
- `400` — zły input (np. pusty `title`).
- `401/403` — brak autoryzacji/zakaz.
- `404` — zasób nie istnieje.
- `409` — konflikt (np. duplikat).
- `422` — semantyka (np. nieprawidłowy stan przejścia).

## Sesje, cookies, CSRF
- Cookie z `HttpOnly`, `SameSite=Lax/Strict`, rozważ `Secure`.
- CSRF: token w nagłówku (np. `X-CSRF-Token`) weryfikowany po stronie serwera.
- CORS: jawne `Access-Control-Allow-*` + preflight.

## Nagłówki bezpieczeństwa (minimum)
- `X-Content-Type-Options: nosniff`
- `Referrer-Policy: no-referrer`
- `Permissions-Policy: geolocation=()`
- `Content-Security-Policy: default-src 'self'`

## Rejestrowanie i diagnstyka
- Log żądań (metoda, ścieżka, status, czas).
- ID korelacji (trace id) dla requestu = łatwiejszy debug.
- Wersjonowanie API (`/api/v1/…`) i **deprecations** w komunikatach.

## Testy API
- `.http` (VS Code REST Client) lub Postman.
- Testy „happy path” i „sad path” (błędy walidacji, 401/403/404/409).
