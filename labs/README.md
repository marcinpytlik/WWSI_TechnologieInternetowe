# Laboratoria

Laboratoria są organizowane w 10 blokach odpowiadających planowi kursu.

## Docelowa mapa

| Blok | Temat | Główny rezultat |
|---|---|---|
| 01 | HTTP | analiza request/response i pierwszy kontener |
| 02 | HTML/CSS/JS | frontend korzystający z fetch |
| 03 | ASP.NET Core API | pierwsze endpointy i Swagger |
| 04 | REST API | CRUD, DTO, validation, Problem Details |
| 05 | EF Core + SQL Server | API z trwałą bazą danych |
| 06 | Frontend + API | pełna komunikacja klient–serwer |
| 07 | JWT | chronione endpointy |
| 08 | Docker Compose | API + DB uruchamiane jako jeden system |
| 09 | Nginx + security | reverse proxy, HTTPS, nagłówki i performance |
| 10 | Final Project | kompletna aplikacja kontenerowa |

## Standard katalogu laboratorium

Każdy katalog powinien zawierać co najmniej:

```text
README.md
starter/
solution/
requests/
```

W zależności od laboratorium może również zawierać:
- `docker-compose.yml`,
- `Dockerfile`,
- skrypty SQL,
- pliki konfiguracyjne Nginx,
- przykładowe pliki `.http`.

## Standard README laboratorium

1. Cel
2. Architektura
3. Wymagania
4. Przygotowanie
5. Kroki krok po kroku
6. Weryfikacja
7. Zadania dodatkowe
8. Cleanup

## Uwaga o starej wersji repo

Obecne materiały Node.js / Express i starsze laboratoria będą stopniowo migrowane lub archiwizowane. Nie należy usuwać ich przed przeniesieniem wartościowych fragmentów do nowej struktury.
