# Ćwiczenia – SQL Server Express w aplikacjach WWW

- Wykonaj skrypt `init_demo_db.sql`, aby utworzyć bazę `DemoDB` z tabelą `Users`.
- Sprawdź zawartość tabeli w `sqlcmd` lub rozszerzeniu **mssql** w VS Code.
- Rozszerz aplikację Node.js o endpoint `/users`, który pobierze rekordy z tabeli `Users` i wyświetli w formacie JSON.
- Dodaj endpoint `/addUser?name=Test&email=test@example.com`, który doda użytkownika do tabeli.