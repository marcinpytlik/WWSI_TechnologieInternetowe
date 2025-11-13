# Lab04 — REST API: routing, walidacja, CORS, paginacja

**Cel:** Zaimplementować REST dla `todos` z CRUD, walidacją, paginacją i CORS.

**Czas:** 120 min    **Poziom:** średni

## Kroki
1. **Start API.** `node api/server.js` (port 4000), sprawdź `/api/health`.
2. **Rozszerz model.** Dodaj `createdAt`, `updatedAt` do todos; ustawiaj czasy w POST/PATCH.
3. **Walidacja.** `400` gdy tytuł pusty lub >120 znaków.
4. **Paginacja/sort.** Query `?page=1&size=10&sort=createdAt:desc`.
5. **CORS.** Wyjaśnij preflight i przetestuj w przeglądarce operacje modyfikujące.
6. **Testy REST.** Użyj `tests/api.http`.

## Weryfikacja
- `POST /api/todos` → `201 Created` + `Location`.
- `GET /api/todos` → `items/page/size/total`.
