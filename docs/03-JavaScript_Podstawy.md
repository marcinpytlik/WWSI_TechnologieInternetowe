# JavaScript — DOM, fetch, moduły, async/await

## DOM i zdarzenia
```js
const btn = document.getElementById('btn')
btn.addEventListener('click', () => doThing())

function doThing(){ /* ... */ }
```

## Fetch i obsługa błędów
```js
async function http(url, opts={}){
  const res = await fetch(url, opts)
  if (!res.ok) throw new Error(`HTTP ${res.status}`)
  const ct = res.headers.get('content-type') || ''
  return ct.includes('json') ? res.json() : res.text()
}
```
- Timeout? Użyj `AbortController`.
- Retry? Tylko dla błędów tymczasowych (`502/503/504`).

## Moduły ES
```html
<script type="module" src="/js/main.js"></script>
```
```js
// main.js
import { listTodos } from './api.js'
listTodos().then(render)
```

## Asynchroniczność
- `async/await` upraszcza składnię, ale pamiętaj o **try/catch**.
- Równoległość: `await Promise.all([a(), b()])`.

## Mała architektura
- `api.js` — funkcje sieciowe (fetch, auth, retry).
- `state.js` — stan aplikacji (np. lista zadań).
- `ui.js` — manipulacje DOM, eventy.
- `main.js` — inicjalizacja.

## Testy ręczne
- Loguj `console.error(err)` i pokazuj komunikat w UI (`aria-live="polite"`).
- Dodaj `data-testid="..."` do trudnych elementów — ułatwia automatyzację.
