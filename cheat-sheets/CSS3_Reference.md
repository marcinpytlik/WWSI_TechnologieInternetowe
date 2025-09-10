# 🎨 CSS3 – Kompendium

## Selektory podstawowe
- `*` – wszystkie elementy.  
- `element` – selektor po nazwie (np. `p`, `h1`).  
- `.class` – selektor po klasie.  
- `#id` – selektor po identyfikatorze.  
- `element1, element2` – grupowanie.  
- `element element` – selektor potomków.  
- `element > element` – selektor dzieci.  
- `element + element` – selektor bezpośredniego sąsiada.  
- `element ~ element` – selektor wszystkich następnych elementów.  

---

## Selektory atrybutów
- `[attr]` – elementy z atrybutem.  
- `[attr=value]` – elementy z konkretną wartością.  
- `[attr^=value]` – wartość zaczyna się od.  
- `[attr$=value]` – wartość kończy się na.  
- `[attr*=value]` – wartość zawiera fragment.  

---

## Selektory pseudoklas
- `:hover` – po najechaniu.  
- `:focus` – w trakcie aktywacji.  
- `:first-child` / `:last-child` – pierwszy/ostatni element w rodzicu.  
- `:nth-child(n)` – element o pozycji n.  
- `:not(selector)` – elementy niepasujące.  
- `:checked` – zaznaczone pole formularza.  
- `:disabled` / `:enabled` – wyłączone/włączone elementy.  
- `:required` – wymagane pole formularza.  

---

## Selektory pseudoelementów
- `::before` – treść przed elementem.  
- `::after` – treść po elemencie.  
- `::first-letter` – pierwsza litera.  
- `::first-line` – pierwsza linia.  
- `::selection` – zaznaczony fragment.  

---

## Właściwości tekstu
- `color` – kolor tekstu.  
- `font-family` – krój czcionki.  
- `font-size` – rozmiar czcionki.  
- `font-weight` – grubość (`normal`, `bold`, liczba 100–900).  
- `font-style` – styl (`normal`, `italic`).  
- `text-align` – wyrównanie (`left`, `center`, `right`, `justify`).  
- `text-decoration` – dekoracja (`none`, `underline`, `line-through`).  
- `line-height` – odstęp między wierszami.  
- `letter-spacing` – odstęp między literami.  
- `word-spacing` – odstęp między słowami.  

---

## Właściwości tła i obramowań
- `background-color` – kolor tła.  
- `background-image` – obraz w tle.  
- `background-repeat` – powtarzanie obrazu (`repeat`, `no-repeat`).  
- `background-position` – pozycja obrazu (`center`, `top`, `10px 20px`).  
- `background-size` – rozmiar obrazu (`cover`, `contain`).  
- `border` – obramowanie (np. `1px solid black`).  
- `border-radius` – zaokrąglenie rogów.  
- `box-shadow` – cień elementu.  
- `opacity` – przezroczystość (0–1).  

---

## Właściwości rozmiaru i pozycjonowania
- `width` / `height` – szerokość i wysokość.  
- `min-width` / `max-width` – minimalna/maksymalna szerokość.  
- `margin` – marginesy.  
- `padding` – odstępy wewnętrzne.  
- `display` – sposób wyświetlania (`block`, `inline`, `inline-block`, `flex`, `grid`, `none`).  
- `position` – pozycjonowanie (`static`, `relative`, `absolute`, `fixed`, `sticky`).  
- `top`, `right`, `bottom`, `left` – przesunięcia elementu.  
- `z-index` – kolejność nakładania elementów.  

---

## Flexbox
- `display: flex;` – włącza Flexbox.  
- `flex-direction` – kierunek (`row`, `column`).  
- `justify-content` – wyrównanie w osi głównej (`flex-start`, `center`, `space-between`).  
- `align-items` – wyrównanie w osi poprzecznej (`stretch`, `center`).  
- `flex-wrap` – zawijanie (`nowrap`, `wrap`).  
- `align-content` – wyrównanie wielu linii.  
- `flex` – skrót dla `grow`, `shrink`, `basis`.  

---

## Grid
- `display: grid;` – włącza Grid.  
- `grid-template-columns` – układ kolumn.  
- `grid-template-rows` – układ wierszy.  
- `gap` – odstęp między elementami.  
- `grid-column` – pozycja kolumnowa.  
- `grid-row` – pozycja wierszowa.  
- `align-items` / `justify-items` – wyrównanie elementów.  
- `align-content` / `justify-content` – wyrównanie całej siatki.  

---

## Animacje i przejścia
- `transition` – definiuje animację przejścia (np. `all 0.3s ease`).  
- `transform` – przekształcenia (`rotate`, `scale`, `translate`, `skew`).  
- `animation-name` – nazwa animacji.  
- `animation-duration` – czas trwania.  
- `animation-timing-function` – funkcja (np. `linear`, `ease`).  
- `animation-delay` – opóźnienie animacji.  
- `animation-iteration-count` – liczba powtórzeń.  
- `animation-direction` – kierunek (`normal`, `reverse`, `alternate`).  

---

## Jednostki CSS
- **Absolutne:** `px`, `pt`, `cm`, `mm`, `in`.  
- **Względne:** `%`, `em`, `rem`, `vw`, `vh`, `vmin`, `vmax`.  

---

## Kolory
- Nazwy kolorów: `red`, `blue`, `green`.  
- HEX: `#ff0000`.  
- RGB: `rgb(255,0,0)`.  
- RGBA: `rgba(255,0,0,0.5)` (z przezroczystością).  
- HSL: `hsl(0, 100%, 50%)`.  
- HSLA: `hsla(0, 100%, 50%, 0.5)`.  
