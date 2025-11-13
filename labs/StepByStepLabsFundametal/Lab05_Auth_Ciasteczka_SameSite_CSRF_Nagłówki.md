# Lab05 — Auth: cookies vs tokeny, SameSite, CSRF, nagłówki bezpieczeństwa

**Cel:** Zrozumieć sesję na ciasteczkach, SameSite/HttpOnly (koncepcyjnie), dodać prosty CSRF i security headers.

**Czas:** 120 min    **Poziom:** średnio‑zaawansowany

## Kroki
1. **Cookie + Max-Age.** W `/api/login` ustaw `Max-Age=1800`.
2. **CSRF.** Dodaj `GET /api/csrf` generujący token; dla metod modyfikujących wymagaj nagłówka `X-CSRF-Token` zgodnego z sesją.
3. **Nagłówki.** Ustaw globalnie:
   - `X-Content-Type-Options: nosniff`
   - `Referrer-Policy: no-referrer`
   - `Permissions-Policy: geolocation=()`
   - `Content-Security-Policy: default-src 'self'`
4. **SameSite.** Porównaj `Lax` vs `Strict` (ręczna zmiana) w DevTools.
5. **Wylogowanie.** `POST /api/logout` czyści cookie.

## Weryfikacja
- Bez `X-CSRF-Token` modyfikacje → `403`.
- Nagłówki security obecne w odpowiedziach.
