# 📦 Repozytorium – Technologie Internetowe (40h)

Kompletny zestaw materiałów: sylabus, ściągi, ćwiczenia, mini-laby, skrypty T-SQL, zadania projektowe oraz dwa przykładowe projekty Node.js (demo i pełniejszy szkielet API).

## Struktura
- `docs/` – sylabus i dokumenty kursowe
- `setup/` – instrukcje instalacji + checklisty
- `cheat-sheets/` – ściągi: HTML5, atrybuty, CSS3, JS (ES6+)
- `exercises/` – 10 plików ćwiczeń do bloków 4h
- `labs/mini/` – mini-zadania do HTML5, CSS3, JS
- `db/` – skrypty T-SQL (schema, indeksy, widoki/TVF, procedury, seed)
- `tasks/` – 6 zadań projektowych (MD) dla Node.js + SQL Server
- `demo/` – mały projekt Express + SQL Server + `init_demo_db.sql`
- `api/` – pełniejszy szkielet API (Express + mssql + JWT + import + raporty)

## Szybki start API
1. Uruchom skrypty z `db/` (kolejno 01..05).
2. Przejdź do `api/`, skopiuj `.env.example` → `.env` i uzupełnij hasło.
3. Zainstaluj paczki i odpal:
   ```bash
   npm install
   npm start
   ```

## Szybki start DEMO
1. Wykonaj `demo/init_demo_db.sql` w SQL Server Express.
2. W `demo/`:
   ```bash
   npm install
   npm start
   ```
3. Odwiedź `http://localhost:3000/users`.

Miłej pracy! 🚀
