# Lab 02 — HTML, CSS i JavaScript + fetch()

## Cel

W tym laboratorium zbudujesz prosty frontend działający w przeglądarce i komunikujący się z serwerem przez HTTP.

Po wykonaniu laba potrafisz:
- zbudować semantyczną stronę HTML,
- zastosować podstawowe style CSS,
- reagować na zdarzenia w JavaScript,
- modyfikować DOM,
- pobierać dane przez `fetch()`,
- obsłużyć odpowiedź JSON,
- rozróżnić stan loading / success / error,
- analizować requesty `fetch()` w DevTools,
- wyjaśnić różnicę między kodem wykonywanym w przeglądarce a kodem serwera.

## Architektura

```text
Browser
  |
  | GET /
  v
Nginx
  |
  +--> index.html
  +--> styles.css
  +--> app.js
  +--> /api/course.json
```

W tym labie backend jest nadal statyczny.

Celem jest zrozumienie zachowania frontendu i komunikacji HTTP z poziomu JavaScript.

## Wymagania

- wykonany lub pobrany Lab 01,
- Docker Desktop,
- przeglądarka z DevTools,
- opcjonalnie Visual Studio Code.

## Struktura

```text
labs/02-html-css-js/
├── README.md
├── docker-compose.yml
├── nginx/
│   ├── default.conf
│   └── html/
│       ├── index.html
│       ├── styles.css
│       ├── app.js
│       └── api/
│           └── course.json
└── requests/
    └── frontend.http
```

---

# Krok 1 — Uruchom środowisko

Przejdź do katalogu:

```powershell
cd labs/02-html-css-js
```

Uruchom:

```powershell
docker compose up -d
```

Sprawdź:

```powershell
docker compose ps
```

Otwórz:

```text
http://localhost:8080
```

---

# Krok 2 — Przeanalizuj HTML

Otwórz plik:

```text
nginx/html/index.html
```

Znajdź elementy:
- `header`,
- `main`,
- `section`,
- `form`,
- `button`,
- `ul`.

## Pytanie

Dlaczego semantyczny HTML jest lepszy niż budowanie całej strony z samych `div`?

---

# Krok 3 — CSS

Otwórz:

```text
nginx/html/styles.css
```

Zwróć uwagę na:
- selektory,
- klasy,
- box model,
- flexbox,
- media query,
- stany przycisku,
- klasę `.error`.

Zmień tytuł strony lub szerokość kontenera i odśwież przeglądarkę.

---

# Krok 4 — Pierwsza interakcja JavaScript

Na stronie wpisz imię i kliknij:

```text
Przywitaj
```

JavaScript:
- przechwytuje submit formularza,
- blokuje domyślne przeładowanie strony,
- odczytuje wartość pola,
- aktualizuje DOM.

Znajdź w `app.js`:

```javascript
event.preventDefault();
```

## Pytanie

Co by się stało bez `preventDefault()`?

---

# Krok 5 — DOM

W DevTools przejdź do:

```text
Elements
```

Kliknij przycisk kilka razy i obserwuj element:

```html
<p id="greeting"></p>
```

Zwróć uwagę, że JavaScript zmienia istniejący dokument HTML bez ponownego pobierania całej strony.

---

# Krok 6 — fetch()

Kliknij:

```text
Pobierz informacje o kursie
```

Kod wykonuje:

```javascript
fetch("/api/course.json")
```

Następnie:
- sprawdza `response.ok`,
- konwertuje odpowiedź do JSON,
- renderuje dane do DOM.

## Pytania

1. Jaka metoda HTTP została użyta?
2. Jaki kod statusu otrzymał browser?
3. Jaki jest `Content-Type` odpowiedzi?
4. Czy JSON jest HTML-em?

---

# Krok 7 — DevTools Network

Otwórz:

```text
F12 -> Network
```

Kliknij ponownie:

```text
Pobierz informacje o kursie
```

Znajdź request:

```text
course.json
```

Sprawdź:
- Request URL,
- Request Method,
- Status Code,
- Response Headers,
- Response,
- Timing.

Porównaj z:

```powershell
curl.exe -i http://localhost:8080/api/course.json
```

---

# Krok 8 — Loading state

Przed wysłaniem requestu aplikacja pokazuje:

```text
Ładowanie...
```

To ważny wzorzec UI.

W realnej aplikacji request może trwać:
- 20 ms,
- 500 ms,
- 3 sekundy,
- albo zakończyć się błędem.

Frontend powinien informować użytkownika o stanie.

---

# Krok 9 — Obsługa błędu

W `app.js` tymczasowo zmień:

```javascript
fetch("/api/course.json")
```

na:

```javascript
fetch("/api/not-found.json")
```

Odśwież stronę i kliknij przycisk.

Powinien pojawić się komunikat błędu.

W DevTools zobaczysz:

```text
404 Not Found
```

Przywróć poprawny URL po ćwiczeniu.

---

# Krok 10 — async / await

Znajdź funkcję:

```javascript
async function loadCourse()
```

Zwróć uwagę na:

```javascript
const response = await fetch(...);
const data = await response.json();
```

## Pytanie

Dlaczego komunikacja HTTP jest asynchroniczna?

---

# Krok 11 — renderowanie listy

Dane JSON zawierają tablicę tematów.

JavaScript tworzy elementy:

```javascript
document.createElement("li")
```

i dodaje je do DOM.

To pierwszy krok do dynamicznego frontendu.

---

# Krok 12 — formularz

Rozbuduj formularz o pole:

```text
kierunek
```

Po kliknięciu przycisku wyświetl:

```text
Cześć Jan, studiujesz Informatykę.
```

Nie używaj `alert()`.

Wynik powinien pojawić się w DOM.

---

# Weryfikacja

Lab jest zaliczony, jeśli potrafisz:
- uruchomić stronę z Dockera,
- wskazać semantyczne elementy HTML,
- zmienić styl CSS,
- obsłużyć submit formularza,
- zmodyfikować DOM,
- pobrać JSON przez `fetch()`,
- znaleźć request w Network,
- obsłużyć 404,
- wyjaśnić `async/await`.

---

# Zadania dodatkowe

## Zadanie A — licznik

Dodaj przycisk:

```text
Kliknięcia: 0
```

Każde kliknięcie ma zwiększać licznik.

## Zadanie B — filtrowanie

Dodaj pole tekstowe, które filtruje listę tematów kursu.

Przykład:

```text
docker
```

powinien zostawić tylko tematy zawierające to słowo.

## Zadanie C — drugi endpoint statyczny

Dodaj:

```text
/api/student.json
```

z przykładowymi danymi studenta i pobierz je przez osobny przycisk.

## Zadanie D — sztuczne opóźnienie

Dodaj w JavaScript:

```javascript
await new Promise(resolve => setTimeout(resolve, 1500));
```

przed renderowaniem danych.

Obserwuj loading state.

## Zadanie E — błąd w JSON

Celowo uszkodź składnię pliku JSON i zobacz, jaki błąd pojawi się w konsoli.

Potem napraw plik.

---

# Cleanup

```powershell
docker compose down
```

---

# Co dalej?

W Lab 03 statyczny JSON zostanie zastąpiony prawdziwym backendem.

Po raz pierwszy uruchomimy:

```text
ASP.NET Core Web API
```

i frontend zacznie pobierać dane z endpointu aplikacyjnego zamiast z pliku statycznego.
