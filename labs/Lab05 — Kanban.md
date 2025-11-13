## Lab05 — Kanban (kolumny i zadania)

**Cel:** Prosta tablica Kanban z przenoszeniem kart między kolumnami.

### Wymagania funkcjonalne
1. **Kolumny:**
   - Predefiniowane 3 kolumny: `Todo`, `Doing`, `Done` z polem `ord` (kolejność).
2. **Zadania:**
   - Dodawanie `title`, przypisanie do kolumny, pole `ord` w kolumnie.
   - Przenoszenie zadania do innej kolumny (zmiana `col_id`, `ord`).

### Model danych
- `columns(id, name, ord)`
- `tasks(id, title, col_id→columns.id, ord)`

### Kontrakt API
- `GET /api/board` → `{ cols:[], tasks:[] }` posortowane.
- `POST /api/tasks {title,col_id}` → `201` (ustaw `ord = MAX+1` w danej kolumnie).
- `POST /api/tasks/{id}/move {col_id,ord}` → `200` (aktualizacja pozycji).

### UI (minimum)
- Trzy kolumny obok siebie, listy zadań, przycisk do przeniesienia „w prawo”.

### Akceptacja
- Dodanie/przeniesienie utrzymuje stabilną kolejność (`ord`).
- Po odświeżeniu przeglądarki widok odtwarza ten sam stan.

### Bonusy
- Drag&drop, masowa zmiana kolejności z przeliczeniem `ord`.

---