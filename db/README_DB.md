# Instrukcja uruchomienia skryptów T-SQL

## Kolejność
1. `01_schema.sql` – baza i tabele
2. `02_indexes.sql` – indeksy
3. `03_views_tvf.sql` – widoki i funkcje
4. `04_procs.sql` – procedury
5. `05_seed.sql` – dane przykładowe

## Jak uruchomić (sqlcmd)
```bash
sqlcmd -S localhost\SQLEXPRESS -E -i 01_schema.sql
sqlcmd -S localhost\SQLEXPRESS -E -i 02_indexes.sql
sqlcmd -S localhost\SQLEXPRESS -E -i 03_views_tvf.sql
sqlcmd -S localhost\SQLEXPRESS -E -i 04_procs.sql
sqlcmd -S localhost\SQLEXPRESS -E -i 05_seed.sql
```
Lub użyj rozszerzenia **SQL Server (mssql)** w VS Code i uruchamiaj pliki po kolei.
