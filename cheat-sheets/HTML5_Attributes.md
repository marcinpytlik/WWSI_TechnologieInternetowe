# 📘 Atrybuty HTML5 – Kompendium

## Globalne atrybuty (mogą być używane w większości znaczników)
- `id` – unikalny identyfikator elementu.  
- `class` – klasa CSS elementu.  
- `style` – styl inline (CSS).  
- `title` – dodatkowy opis elementu (tooltip).  
- `lang` – język zawartości (`pl`, `en`).  
- `dir` – kierunek tekstu (`ltr`, `rtl`).  
- `hidden` – ukrywa element.  
- `tabindex` – kolejność przechodzenia klawiszem Tab.  
- `contenteditable` – czy element może być edytowalny przez użytkownika.  
- `draggable` – czy element może być przeciągany (`true` / `false`).  
- `spellcheck` – sprawdzanie pisowni (`true` / `false`).  
- `accesskey` – skrót klawiaturowy do elementu.  

---

## Atrybuty dla linków i zasobów
- `href` – adres linku (dla `<a>`, `<link>`).  
- `target` – sposób otwierania linku (`_blank`, `_self`, `_parent`, `_top`).  
- `rel` – relacja linku (`stylesheet`, `nofollow`, `noopener`).  
- `download` – pobieranie pliku zamiast otwierania w przeglądarce.  
- `src` – źródło (dla `<img>`, `<script>`, `<iframe>`, `<audio>`, `<video>`).  
- `alt` – tekst alternatywny obrazu.  
- `crossorigin` – kontrola CORS dla zasobów zewnętrznych.  
- `integrity` – weryfikacja pliku przez hash (Subresource Integrity).  

---

## Atrybuty multimediów
- `controls` – pokazuje kontrolki odtwarzacza.  
- `autoplay` – automatyczne odtwarzanie.  
- `loop` – odtwarzanie w pętli.  
- `muted` – wyciszenie dźwięku.  
- `poster` – obrazek startowy dla `<video>`.  
- `preload` – sposób ładowania (`none`, `metadata`, `auto`).  

---

## Atrybuty obrazów i map
- `width` / `height` – szerokość i wysokość (w px).  
- `usemap` – powiązanie obrazu z mapą (`<map>`).  
- `coords` – współrzędne obszaru mapy (`<area>`).  
- `shape` – kształt obszaru mapy (`rect`, `circle`, `poly`).  

---

## Atrybuty formularzy
- `action` – adres przetwarzania formularza.  
- `method` – metoda przesyłania (`get`, `post`).  
- `enctype` – typ kodowania (`application/x-www-form-urlencoded`, `multipart/form-data`).  
- `name` – nazwa pola (przesyłana do serwera).  
- `value` – wartość domyślna pola.  
- `placeholder` – tekst podpowiedzi.  
- `required` – pole obowiązkowe.  
- `readonly` – pole tylko do odczytu.  
- `disabled` – pole wyłączone.  
- `checked` – zaznaczone domyślnie (`checkbox`, `radio`).  
- `selected` – zaznaczona opcja w `<select>`.  
- `multiple` – możliwość wyboru wielu opcji.  
- `maxlength` / `minlength` – maksymalna/minimalna długość tekstu.  
- `min` / `max` – wartości liczbowe lub daty.  
- `step` – krok wartości (np. w polach liczbowych).  
- `autocomplete` – podpowiedzi przeglądarki (`on`, `off`).  
- `pattern` – wyrażenie regularne do walidacji.  

---

## Atrybuty tabel
- `colspan` – łączenie kolumn w tabeli.  
- `rowspan` – łączenie wierszy w tabeli.  
- `scope` – określa nagłówek w tabeli (`row`, `col`).  

---

## Atrybuty specjalne HTML5
- `data-*` – własne atrybuty użytkownika (np. `data-id="123"`).  
- `aria-*` – atrybuty dostępności (Accessibility / ARIA).  
- `async` – asynchroniczne ładowanie `<script>`.  
- `defer` – opóźnione ładowanie `<script>` do czasu parsowania DOM.  
- `sandbox` – ograniczenia dla `<iframe>`.  
- `srcdoc` – zawartość HTML wewnątrz `<iframe>`.  
- `loading` – kontrola ładowania obrazów (`lazy`, `eager`).  
