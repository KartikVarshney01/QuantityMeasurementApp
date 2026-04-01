# Quantity Measurement Application — UC17: REST API + EF Core Integration

A multi-layered .NET 8 solution that exposes quantity measurement operations (Convert, Compare, Add, Subtract, Divide) as a RESTful Web API backed by Entity Framework Core and SQL Server. Includes a console app, full unit test suite, and an automatic fallback to an in-memory cache repository when SQL Server is unavailable.


---

## Overview

UC17 builds on the earlier UC16 console application by adding:

- An **ASP.NET Core 8 Web API** (`QuantityMeasurementAPI`) with Swagger/OpenAPI documentation
- **Entity Framework Core** replacing the manual ADO.NET + stored procedure approach from UC16
- **EF Core Migrations** for automatic schema creation and updates
- **Global Exception Handling Middleware** that maps domain exceptions to appropriate HTTP status codes
- **Request DTO validation** via `DataAnnotations` so bad input is rejected before business logic runs
- Structured **operation history endpoints** (filter by operation type or measurement category)

---

## Solution Structure

```
UC17-BackendIntegration/
│
├── QuantityMeasurementAPI/               # ASP.NET Core 8 Web API
│   ├── Controllers/
│   │   └── QuantitiesController.cs       # All REST endpoints
│   ├── Middleware/
│   │   └── GlobalExceptionHandlingMiddleware.cs
│   ├── Program.cs                        # DI wiring, EF Core setup, middleware pipeline
│   ├── appsettings.json                  # Connection string + logging config
│   └── Properties/launchSettings.json   # Runs on http://localhost:5000
│
├── QuantityMeasurementBusinessLayer/     # Domain logic
│   ├── Engines/
│   │   ├── ArithmeticEngine.cs
│   │   ├── ConversionEngine.cs
│   │   └── ValidationEngine.cs
│   ├── Extensions/                       # Unit conversion factors
│   ├── Interfaces/IQuantityMeasurementService.cs
│   └── Services/QuantityMeasurementServiceImpl.cs
│
├── QuantityMeasurementModelLayer/        # Shared DTOs, entities, enums
│   ├── DTOs/
│   │   ├── RequestDTOs.cs                # QuantityRequest, BinaryOperationRequest, ConversionRequest
│   │   └── ResponseDTOs.cs               # ApiResponse<T>, ErrorResponse, ComparisonResponse, etc.
│   ├── Entities/QuantityMeasurementEntity.cs
│   ├── Enums/                            # LengthUnit, WeightUnit, VolumeUnit, TemperatureUnit, OperationType
│   └── Validations/QuantityValidator.cs
│
├── QuantityMeasurementRepoLayer/         # Data access
│   ├── Data/ApplicationDbContext.cs      # EF Core DbContext with JSON column converters
│   ├── Implementations/
│   │   ├── EFCoreQuantityMeasurementRepository.cs   # SQL Server via EF Core
│   │   └── QuantityMeasurementCacheRepository.cs    # In-memory fallback (singleton)
│   ├── Interfaces/IQuantityMeasurementRepository.cs
│   └── Migrations/                       # EF Core migration: InitialCreate
│
├── QuantityMeasurementConsoleApp/        # Original UC16 console interface (still works)
│
└── QuantityMeasurementApp.Tests/         # xUnit test suite
    ├── Engines/                          # ArithmeticEngine, ConversionEngine, ValidationEngine tests
    ├── Repository/                       # Repository tests
    └── Services/                         # Service layer tests
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Web API | ASP.NET Core 8 |
| ORM | Entity Framework Core 8 |
| Database | SQL Server / SQL Server Express |
| Documentation | Swashbuckle (Swagger / OpenAPI) |
| Testing | xUnit |
| Target Framework | .NET 8 |

---

## API Endpoints

Base URL: `http://localhost:5000/api/quantities`

Swagger UI: `http://localhost:5000/swagger`

### Operations

| Method | Endpoint | Description |
|---|---|---|
| `POST` | `/compare` | Compare two quantities — returns `true` / `false` |
| `POST` | `/convert` | Convert a quantity to a different unit |
| `POST` | `/add` | Add two quantities (result in Q1's unit) |
| `POST` | `/subtract` | Subtract Q2 from Q1 (result in Q1's unit) |
| `POST` | `/divide` | Divide Q1 by Q2 — returns a scalar ratio |

### History

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/history` | All operation records, newest first |
| `GET` | `/history/operation/{operation}` | Filter by operation (e.g. `Add`, `Convert`) |
| `GET` | `/history/type/{measurementType}` | Filter by category (e.g. `LENGTH`, `WEIGHT`) |
| `GET` | `/count` | Total number of records saved |
| `GET` | `/health` | Health check — returns `200 OK` with timestamp |

### Example Request Body (POST /compare)

```json
{
  "q1": {
    "value": 1,
    "unitName": "Feet",
    "category": "LENGTH"
  },
  "q2": {
    "value": 12,
    "unitName": "Inch",
    "category": "LENGTH"
  }
}
```

### Example Response

```json
{
  "success": true,
  "message": "Comparison successful.",
  "data": {
    "areEqual": true,
    "message": "Quantities are equal."
  },
  "timestamp": "2026-03-30T09:45:32Z"
}
```

### Example Request Body (POST /convert)

```json
{
  "quantity": {
    "value": 100,
    "unitName": "Celsius",
    "category": "TEMPERATURE"
  },
  "targetUnit": "Fahrenheit"
}
```

---

## Supported Units

| Category | Units |
|---|---|
| `LENGTH` | `Feet`, `Inch`, `Yard`, `Centimeter` |
| `WEIGHT` | `Kilogram`, `Gram`, `Pound` |
| `VOLUME` | `Litre`, `Millilitre`, `Gallon` |
| `TEMPERATURE` | `Celsius`, `Fahrenheit`, `Kelvin` |

> **Note:** Arithmetic operations (Add, Subtract, Divide) are **not supported** for `TEMPERATURE`. Compare and Convert work for all categories.

Unit names are case-insensitive. Common abbreviations like `kg`, `ml`, `ft`, `cm` are also accepted.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8)
- SQL Server or SQL Server Express (optional — the app falls back to in-memory cache if unavailable)

### Clone and Build

```bash
git clone <your-repo-url>
cd UC17-BackendIntegration
dotnet build
```

### Run the API

```bash
cd QuantityMeasurementAPI
dotnet run
```

The API starts at `http://localhost:5000`. Open `http://localhost:5000/swagger` in your browser to explore all endpoints interactively.

---

## Database Setup

### Connection String

Edit `QuantityMeasurementAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=QuantityMeasurementAppDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
  }
}
```

Adjust `Server=` to match your SQL Server instance name.

### Apply Migrations

Migrations run **automatically on startup** when a connection string is configured. To run them manually:

```bash
# From the solution root
dotnet ef database update \
  --project QuantityMeasurementRepoLayer \
  --startup-project QuantityMeasurementAPI
```

### Add a New Migration

```bash
dotnet ef migrations add <MigrationName> \
  --project QuantityMeasurementRepoLayer \
  --startup-project QuantityMeasurementAPI
```

### Database Schema

The migration creates a single `QuantityMeasurements` table:

| Column | Type | Notes |
|---|---|---|
| `Id` | `int` (PK, identity) | Auto-incremented |
| `Operation` | `nvarchar(50)` | e.g. `Compare`, `Add` |
| `Operand1` | `nvarchar(max)` | JSON-serialised quantity |
| `Operand2` | `nvarchar(max)` | JSON-serialised quantity (nullable) |
| `Result` | `nvarchar(max)` | JSON-serialised result |
| `HasError` | `bit` | `1` if the operation failed |
| `ErrorMessage` | `nvarchar(500)` | Populated on error |
| `CreatedAt` | `datetime2` | UTC timestamp |

Indexes are created on `CreatedAt`, `Operation`, and `HasError` for query performance.

---

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=QuantityMeasurementAppDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

Remove or leave blank the `DefaultConnection` value to run the app entirely in-memory using the cache repository.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

Tests cover:

- `ArithmeticEngine` — add, subtract, divide across all supported categories
- `ConversionEngine` — unit conversion with base-unit round-trip accuracy
- `ValidationEngine` — same-category checks, null guards, unsupported operations
- `QuantityMeasurementRepository` — save, retrieve, filter, count, delete
- `QuantityMeasurementService` — full integration of engine + repository per operation

---

## Repository Fallback Behaviour

At startup, `Program.cs` checks whether a `DefaultConnection` connection string exists:

- **Connection string present** → registers `EFCoreQuantityMeasurementRepository` (Scoped). EF Core migrations are applied automatically.
- **Connection string absent or SQL Server unreachable** → registers `QuantityMeasurementCacheRepository` (Singleton). All data is stored in memory for the duration of the process.

This means the API is always functional even without a database, which is useful during local development or testing.

---

## Project Architecture

```
HTTP Request
     │
     ▼
QuantitiesController   (API layer — routing, model validation, response mapping)
     │
     ▼
QuantityMeasurementServiceImpl   (Business layer — orchestrates engines + repository)
     │              │
     ▼              ▼
ConversionEngine   ArithmeticEngine   ValidationEngine
     │
     ▼
IQuantityMeasurementRepository
     │
     ├── EFCoreQuantityMeasurementRepository   (SQL Server via EF Core)
     └── QuantityMeasurementCacheRepository    (In-memory fallback)
```

All layers depend on interfaces, not concrete classes, so the repository can be swapped without touching business or API code.
