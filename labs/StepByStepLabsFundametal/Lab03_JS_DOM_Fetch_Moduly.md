# Lab03 — JS: DOM, zdarzenia, fetch, moduły

**Cel:** Manipulacja DOM, obsługa zdarzeń, fetch, moduły ES, obsługa błędów.

**Czas:** 90 min    **Poziom:** średni

## Kroki
1. **Moduły.** Podziel skrypt na `public/js/main.js` i `public/js/api.js`; w HTML użyj `<script type="module">`.
2. **API helper.** W `api.js` dodaj `getHello()`, `login()`, `listTodos()`, `createTodo()` z `fetch`. Dodaj timeout (AbortController).
3. **UI.** W `main.js` dodaj formularz logowania i listę Todo. Obsłuż błędy (region `aria-live="polite"`).
4. **Debounce.** Wyszukiwanie w liście (debounce 300ms).
5. **Retry.** Retry ×2 dla błędów 502/503/504 przy GET.

## Weryfikacja
- Po loginie masz cookie `token`.
- Lista Todo ładuje się i pozwala dodać element.
