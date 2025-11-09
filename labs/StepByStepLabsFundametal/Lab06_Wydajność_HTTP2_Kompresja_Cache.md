# Lab06 — Wydajność: kompresja, cache, HTTP/2/3 (koncepcyjnie)

**Cel:** Zobaczyć wpływ kompresji i cache (ETag/Last-Modified), omówić HTTP/2/3.

**Czas:** 90–120 min    **Poziom:** średnio‑zaawansowany

## Kroki
1. **Duży payload.** Dodaj `/api/big` zwracający 10k obiektów; zmierz rozmiar/czas.
2. **ETag/304.** Policz hash JSON i obsłuż `If-None-Match` → `304 Not Modified`.
3. **Cache-Control.** Zmień `Cache-Control` w `/api/hello` na `max-age=60`; obserwuj w DevTools.
4. **Porównania.** OFF/ON „Disable cache” i zapisz wyniki w `labs/Lab06_Wyniki.md`.

## Weryfikacja
- Widzisz `304` przy drugim zapytaniu z `If-None-Match`.
- Rozumiesz `no-cache` vs `no-store` vs `max-age`.
