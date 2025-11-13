# Lab01 — HTTP: anatomia żądania/odpowiedzi + narzędzia

**Cel:** Zrozumieć składnię i semantykę HTTP (metody, statusy, nagłówki, ciało), ćwiczyć z DevTools i curl, zobaczyć cache i przekierowania.

**Czas:** 60–90 min    **Poziom:** podstawowy/średni

## Wymagania
- VS Code + Chrome/Edge (DevTools)
- curl (zalecany Git Bash / `curl.exe`)

## Kroki
1. **DevTools → Network.** Otwórz `public/index.html` (Live Server). Zbadaj pierwsze żądania. Zanotuj metodę, status, `Content-Type`, rozmiar, czas.
2. **curl – statusy i nagłówki.**
   ```bash
   curl -i http://localhost:3000/api/hello
   curl -i -X POST http://localhost:3000/api/hello
   ```
3. **Cache-Control i ETag.** Zmień w `tools/scripts/http-hello.js` nagłówki:
   ```js
   res.writeHead(200, {'Content-Type':'text/plain','Cache-Control':'max-age=60'})
   ```
   Odśwież i porównaj „Size” (Memory cache vs Network).
4. **Przekierowania.**
   ```js
   if (req.url === '/old') { res.writeHead(301, {'Location':'/'}); return res.end() }
   ```
   Wejdź na `/old` i sprawdź 301→200 w Network.
5. **Ciasteczka.** Zaloguj przez `/api/login` (patrz Lab03/05) i obejrzyj cookie w Application → Cookies.

## Weryfikacja
- Rozpoznajesz podstawowe nagłówki i kody.
- Rozumiesz `Cache-Control`, `ETag`, „preflight” (concept).

## Dodatkowo
- Dodaj `X-Content-Type-Options: nosniff` do odpowiedzi tekstowej.
