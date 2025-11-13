# Lab02 — Strona statyczna: semantic HTML, RWD i a11y

**Cel:** Zbudować responsywną stronę z semantyką HTML5, grid/flex, dostępnością (a11y), i sprawdzić Lighthouse.

**Czas:** 90–120 min    **Poziom:** średni

## Kroki
1. **Semantyka.** W `public/index.html` dodaj `header/nav/main/article/aside/footer` + `aria-label`.
2. **RWD.** W `public/style.css` zrób layout 2‑kolumnowy dla >960px, 1‑kolumnowy dla mniejszych.
3. **Formularz.** Dodaj `form` z `label for`, walidacją (`required`, `type="email"`).
4. **Fokus i kontrast.** Style dla `:focus-visible`; kontrast ≥ 4.5:1.
5. **Obrazy.** `alt` + `srcset` 1x/2x.
6. **Lighthouse.** Wygeneruj raport i zapisz wynik w `labs/Lab02_Lighthouse.md`.

## Weryfikacja
- Działa na mobilu (device toolbar).
- A11y wynik ≥ 90.
