# AdventureWorksAPI – Full-stack demo (API + React)

Minimalny stack do wystawiania danych z **AdventureWorks 2022** przez **ASP.NET Core 9 (Minimal API + EF Core)** i prosty frontend **React + Vite + Tailwind**.

## Wymagania

- **Windows** + **VS Code**
- **.NET SDK 9**
- **Node.js 20+ / 22+**
- **SQL Server 2022** + baza **AdventureWorks2022**
- Połączenie w `appsettings.json` pod kluczem `ConnectionStrings:AdventureWorks2022`

> Domyślne porty: **API → http://localhost:5000**, **Frontend → http://localhost:5173**

## Start – 2 terminale

### 1) Backend (API)

```powershell
cd .\api\src\Api\
dotnet restore
dotnet build
dotnet run --project Api.csproj
```

Swagger: http://localhost:5000/swagger

### 2) Frontend (React)

```powershell
cd .\aw-frontend\
npm install
npm run dev
```

Otwórz: http://localhost:5173

## Konfiguracja

`appsettings.json`
```json
{
  "ConnectionStrings": {
    "AdventureWorks2022": "Server=localhost;Database=AdventureWorks2022;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

## Endpoints

### GET
- `/aw/products` – produkty
- `/aw/customers` – klienci
- `/aw/invoices` – faktury

### POST /aw/customers
```json
{
  "firstName": "Jan",
  "lastName": "Kowalski",
  "territoryId": 1
}
```

### POST /aw/invoices
```json
{
  "customerId": 1,
  "issueDate": "2025-11-12",
  "lines": [
    { "productId": 707, "quantity": 1, "unitPrice": 100, "unitPriceDiscount": 0.0 }
  ]
}
```

## Struktura repo

```
AdventureWorksAPI/
├─ api/
│  └─ src/Api/
│     ├─ Program.cs
│     ├─ appsettings.json
│     ├─ Data/
│     ├─ AwModels/
│     └─ DTOs/
└─ aw-frontend/
   ├─ index.html
   ├─ src/
   │  ├─ main.tsx
   │  ├─ App.tsx
   │  ├─ api.ts
   │  └─ index.css
   ├─ tailwind.config.js
   └─ postcss.config.js
```

## Troubleshooting

**Biały ekran / brak styli Tailwind**
- `tailwind.config.js`:
```js
content: ["./index.html", "./src/**/*.{ts,tsx}"]
```
- Restart `npm run dev`

**CORS**
- `app.UseCors(...)` w `Program.cs`
- `AllowOrigins("http://localhost:5173")`

**Port w użyciu**
- `dotnet run --urls http://localhost:5001`
- Zmień `API_BASE` w `aw-frontend/src/api.ts`

**Node 24 problem z npx**
```
npm remove tailwindcss
npm i -D tailwindcss@3 postcss autoprefixer
```

## Licencja

MIT / edukacyjnie SQLManiak
