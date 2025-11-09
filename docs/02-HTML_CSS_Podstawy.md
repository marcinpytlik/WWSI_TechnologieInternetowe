# HTML & CSS — semantyka, RWD, dostępność

## HTML — semantyka
- Używaj właściwych znaczników: `header`, `nav`, `main`, `article`, `aside`, `footer`.
- Formularze: `label for`, typy (`email`, `number`), wbudowana walidacja (`required`, `min`, `pattern`).
- A11y: atrybuty ARIA oszczędnie; klawiaturowość i kolejność focusu.

**Przykład:**
```html
<main>
  <article>
    <h1>Tytuł</h1>
    <p>Treść…</p>
  </article>
  <aside aria-label="Powiązane">…</aside>
</main>
```

## CSS — layout i responsywność
- **Flexbox**: układy osiowe (header, nawigacja).
- **Grid**: złożone siatki (karty, dashboardy).
- **Media queries**: mobile‑first (`@media (min-width: 768px)`).
- **Focus & kontrast**: `:focus-visible`, WCAG ≥ 4.5:1.

**Przykład RWD:**
```css
main{display:grid;grid-template-columns:3fr 1fr;gap:1rem}
@media (max-width:960px){ main{grid-template-columns:1fr} }
```

## Obrazy i czcionki
- `img[alt]` zawsze; dla RWD: `srcset` + `sizes`.
- Fonty: preconnect do CDN, `font-display: swap`, preloading kluczowych.

## Performance krytycznej ścieżki
- Krytyczne **CSS** w `<head>`; minimalizuj blokowanie renderu.
- Używaj **preload** (`<link rel="preload" as="style" href="...">`).
- Cache statyków: długie `max-age` + hasze w nazwach plików.

## Checklista
- Semantyka? Kontrast? Focus? RWD?  
- Wyłączone obrazki? Czy treść nadal zrozumiała?  
- Lighthouse: ≥ 90 dla A11y i Best Practices.
