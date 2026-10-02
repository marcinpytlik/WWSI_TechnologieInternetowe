# Lab 04 — REST API: DTO, walidacja, Problem Details i paginacja

## Cel

W tym laboratorium uporządkujesz API z Lab 03 tak, aby miało spójny kontrakt i zachowywało się jak poprawnie zaprojektowane REST API.

Po wykonaniu laba potrafisz:
- rozróżnić model domenowy od DTO,
- stosować osobne DTO dla request i response,
- walidować dane wejściowe,
- zwracać spójne błędy w formacie Problem Details,
- filtrować dane,
- sortować dane,
- paginować wyniki,
- zwracać poprawne kody HTTP,
- używać nagłówka `Location`,
- projektować czytelne endpointy zasobowe.

## Architektura

```text
Client
  |
  | HTTP
  v
ASP.NET Core API
  |
  +--> DTO
  +--> Validation
  +--> Problem Details
  +--> Filtering
  +--> Sorting
  +--> Pagination
  |
  v
In-memory service
```

W tym labie dane nadal przechowujemy w pamięci.

Dzięki temu skupiamy się na kontrakcie API, a nie na bazie danych.

## Struktura

```text
labs/04-rest-api/
├── README.md
├── docker-compose.yml
├── Dockerfile
├── requests/
│   └── rest-api.http
└── src/
    └── CourseApi/
        ├── CourseApi.csproj
        ├── Program.cs
        ├── Contracts/
        │   ├── CreateTopicRequest.cs
        │   ├── UpdateTopicRequest.cs
        │   ├── TopicResponse.cs
        │   └── PagedResponse.cs
        ├── Models/
        │   └── CourseTopic.cs
        └── Services/
            ├── ICourseTopicService.cs
            └── CourseTopicService.cs
```

---

# Krok 1 — DTO zamiast bezpośredniego modelu

Model wewnętrzny:

```csharp
CourseTopic
```

nie jest już bezpośrednio zwracany klientowi.

Zamiast tego API używa:

```text
CreateTopicRequest
UpdateTopicRequest
TopicResponse
```

## Pytanie

Dlaczego kontrakt HTTP nie powinien być przypadkowo zależny od wewnętrznego modelu aplikacji?

---

# Krok 2 — GET kolekcji

Uruchom:

```powershell
curl.exe -i http://localhost:8080/api/topics
```

Odpowiedź ma strukturę stronicowaną:

```json
{
  "items": [],
  "page": 1,
  "pageSize": 5,
  "totalItems": 0,
  "totalPages": 0
}
```

---

# Krok 3 — Filtrowanie

Wykonaj:

```powershell
curl.exe -i "http://localhost:8080/api/topics?search=api"
```

Parametr:

```text
search
```

filtruje po nazwie.

## Pytanie

Czy filtrowanie powinno odbywać się po stronie klienta czy API, jeśli danych jest bardzo dużo?

---

# Krok 4 — Sortowanie

Sprawdź:

```powershell
curl.exe -i "http://localhost:8080/api/topics?sort=name"
```

oraz:

```powershell
curl.exe -i "http://localhost:8080/api/topics?sort=-name"
```

Konwencja:

- `name` — rosnąco,
- `-name` — malejąco.

Obsługujemy również:

```text
id
-id
```

---

# Krok 5 — Paginacja

Sprawdź:

```powershell
curl.exe -i "http://localhost:8080/api/topics?page=1&pageSize=2"
```

Potem:

```powershell
curl.exe -i "http://localhost:8080/api/topics?page=2&pageSize=2"
```

## Pytania

1. Dlaczego API nie powinno zawsze zwracać całej tabeli?
2. Co może się stać przy milionie rekordów?
3. Dlaczego `pageSize` powinien mieć limit?

---

# Krok 6 — Walidacja query string

Spróbuj:

```powershell
curl.exe -i "http://localhost:8080/api/topics?page=0&pageSize=5000"
```

API powinno zwrócić:

```text
400 Bad Request
```

w formacie Problem Details.

---

# Krok 7 — Problem Details

Błąd ma spójną strukturę:

```json
{
  "type": "...",
  "title": "Validation error",
  "status": 400,
  "detail": "...",
  "instance": "/api/topics"
}
```

## Pytanie

Dlaczego spójny format błędów jest ważny dla frontendu?

---

# Krok 8 — POST

Dodaj nowy temat:

```powershell
curl.exe -i -X POST -H "Content-Type: application/json" -d "{\"name\":\"Entity Framework Core\"}" http://localhost:8080/api/topics
```

Oczekiwany wynik:

```text
201 Created
```

Nagłówek:

```text
Location
```

powinien wskazywać nowy zasób.

---

# Krok 9 — Walidacja request body

Wyślij:

```powershell
curl.exe -i -X POST -H "Content-Type: application/json" -d "{\"name\":\"A\"}" http://localhost:8080/api/topics
```

Oczekuj:

```text
400 Bad Request
```

Zasada w tym labie:
- nazwa jest wymagana,
- minimum 3 znaki,
- maksimum 100 znaków.

---

# Krok 10 — Konflikt 409

Spróbuj dodać temat o nazwie już istniejącej.

Oczekuj:

```text
409 Conflict
```

## Pytanie

Dlaczego duplikat nie jest błędem `500`?

---

# Krok 11 — PUT

Aktualizacja pełna:

```powershell
curl.exe -i -X PUT -H "Content-Type: application/json" -d "{\"name\":\"Web API Design\"}" http://localhost:8080/api/topics/1
```

Dla istniejącego zasobu:

```text
200 OK
```

Dla nieistniejącego:

```text
404 Not Found
```

---

# Krok 12 — DELETE

Usuń:

```powershell
curl.exe -i -X DELETE http://localhost:8080/api/topics/1
```

Oczekiwany wynik:

```text
204 No Content
```

## Pytanie

Dlaczego `204` nie powinien zawierać body?

---

# Krok 13 — Spójne endpointy

W tym labie API udostępnia:

```text
GET    /api/topics
GET    /api/topics/{id}
POST   /api/topics
PUT    /api/topics/{id}
DELETE /api/topics/{id}
```

To klasyczny kontrakt CRUD dla zasobu:

```text
topics
```

---

# Krok 14 — Swagger

Otwórz:

```text
http://localhost:8080/swagger
```

Sprawdź:
- parametry query,
- request body,
- response schema,
- kody HTTP.

---

# Weryfikacja

Lab jest zaliczony, jeśli potrafisz:
- wyjaśnić po co są DTO,
- wykonać filtrowanie,
- wykonać sortowanie,
- wykonać paginację,
- rozpoznać Problem Details,
- zwrócić 201 + Location,
- rozpoznać 400, 404 i 409,
- wykonać PUT i DELETE,
- wyjaśnić 204 No Content.

---

# Zadania dodatkowe

## Zadanie A — sortowanie po wielu polach

Rozbuduj parser sortowania tak, aby obsługiwał:

```text
sort=name,-id
```

## Zadanie B — filtr po ID

Dodaj query:

```text
minId
```

## Zadanie C — limit pageSize

Zmień maksymalny `pageSize` z 100 na 25.

## Zadanie D — nagłówki paginacji

Dodaj do odpowiedzi:

```text
X-Total-Count
X-Page
X-Page-Size
```

## Zadanie E — PATCH

Dodaj endpoint:

```text
PATCH /api/topics/{id}
```

i zastanów się, czym różni się semantycznie od PUT.

---

# Cleanup

```powershell
docker compose down
```

---

# Co dalej?

W Lab 05 zamienimy serwis in-memory na:

```text
Entity Framework Core + SQL Server w Dockerze
```

Kontrakt API pozostanie praktycznie ten sam.

To właśnie jest korzyść z oddzielenia API od sposobu przechowywania danych.
