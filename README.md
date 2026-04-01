# Quantity Measurement Application — UC16: Database Integration

A multi-layered .NET console application that persists quantity measurement operations (Convert, Compare, Add, Subtract, Divide) to a SQL Server database using ADO.NET and stored procedures. Includes a custom connection pool, a JSON-backed in-memory cache fallback, and a 35-test MSTest suite covering the full database layer.

---

## Overview

UC16 extends the UC15 console application by adding:

- A **SQL Server database repository** (`QuantityMeasurementDatabaseRepository`) using raw ADO.NET and stored procedures — no ORM
- A **custom `ConnectionPool`** that pre-warms connections at startup and reuses them across operations
- An **`ApplicationConfig`** that reads `appsettings.json` line-by-line without any JSON library dependency
- Automatic **fallback to an in-memory cache repository** when SQL Server is unavailable
- A **`SqlSchema.sql`** script that creates the database, table, and all 6 stored procedures
- A **35-test MSTest suite** (`QuantityMeasurementDBTests`) covering connection pooling, CRUD, filtering, concurrency, SQL injection prevention, and schema verification

---

## Solution Structure

```
UC16-DatabaseIntegration/
│
├── QuantityMeasurementApp/                        # Console application entry point
│   ├── Controller/QuantityMeasurementController.cs
│   ├── Interfaces/IMenu.cs
│   ├── Menu/QuantityMenu.cs
│   ├── Program.cs
│   ├── QuantityMeasurementApp.cs                  # Wires all layers; handles DB/cache selection
│   └── appsettings.json                           # Connection string + repository type config
│
├── QuantityMeasurementAppBusinessLayer/           # Domain logic
│   ├── Exceptions/
│   │   ├── DatabaseException.cs
│   │   └── QuantityMeasurementException.cs
│   ├── Extensions/                                # Unit conversion factors per category
│   ├── Interfaces/
│   │   ├── IMeasurable.cs
│   │   ├── IQuantityMeasurementRepository.cs
│   │   └── IQuantityMeasurementService.cs
│   ├── Quantity.cs                                # Generic typed quantity with arithmetic + comparison
│   └── Services/QuantityMeasurementServiceImpl.cs
│
├── QuantityMeasurementAppModelLayer/              # Shared DTOs, entities, enums
│   ├── DTOs/QuantityDTO.cs
│   ├── Entities/QuantityMeasurementEntity.cs
│   └── Enums/                                     # LengthUnit, WeightUnit, VolumeUnit, TemperatureUnit
│
├── QuantityMeasurementAppRepoLayer/               # Data access
│   ├── Exceptions/DatabaseException.cs
│   ├── Interfaces/IQuantityMeasurementRepository.cs
│   ├── Repositories/
│   │   ├── QuantityMeasurementDatabaseRepository.cs  # ADO.NET + stored procedures
│   │   └── QuantityMeasurementCacheRepository.cs     # In-memory + JSON backup fallback
│   └── Utilities/
│       ├── ApplicationConfig.cs                   # Manual JSON parser for appsettings.json
│       └── ConnectionPool.cs                      # Custom LIFO connection pool (singleton)
│
├── QuantityMeasurementApp.Tests/                  # MSTest suite — 35 tests
│   ├── QuantityMeasurementDBTests.cs
│   └── appsettings.json
│
└── SqlSchema.sql                                  # Run this once to set up the database
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Console App | .NET (C#) |
| Data Access | ADO.NET (`Microsoft.Data.SqlClient`) |
| Database | SQL Server / SQL Server Express |
| Connection Management | Custom `ConnectionPool` (no third-party pool) |
| Configuration | Manual `appsettings.json` parser (`ApplicationConfig`) |
| Testing | MSTest |

---

## Supported Operations

| Operation | Description |
|---|---|
| Compare | Checks whether two quantities are equal in base units |
| Convert | Converts a quantity to a different unit in the same category |
| Add | Adds two quantities — result returned in Q1's unit |
| Subtract | Subtracts Q2 from Q1 — result returned in Q1's unit |
| Divide | Divides Q1 by Q2 — returns a dimensionless scalar ratio |

> **Note:** Arithmetic operations (Add, Subtract, Divide) are **not supported** for `TEMPERATURE`. Compare and Convert work for all categories.

---

## Supported Units

| Category | Units |
|---|---|
| `LENGTH` | `Feet`, `Inch`, `Yard`, `Centimeter` |
| `WEIGHT` | `Kilogram`, `Gram`, `Pound` |
| `VOLUME` | `Litre`, `Millilitre`, `Gallon` |
| `TEMPERATURE` | `Celsius`, `Fahrenheit`, `Kelvin` |

Unit names are case-insensitive. Common abbreviations like `kg`, `ft`, `cm`, `ml` are also accepted.

---

## Getting Started

### Prerequisites

- .NET SDK
- SQL Server or SQL Server Express (optional — falls back to in-memory cache automatically)

### Clone and Build

```bash
git clone <your-repo-url>
cd UC16-DatabaseIntegration
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app reads `appsettings.json`, attempts to connect to SQL Server, and either uses the database repository or falls back to the cache repository with a clear console message explaining which one is active.

---

## Database Setup

### 1. Run the SQL Schema Script

Open SQL Server Management Studio (SSMS) or `sqlcmd` and run `SqlSchema.sql` from the root of the project. This script is idempotent — it checks for existence before creating anything, so it is safe to run multiple times.

```sql
-- Run from SSMS or sqlcmd:
SqlSchema.sql
```

The script creates:

- The `QuantityMeasurementAppDB` database
- The `quantity_measurements` table
- 6 stored procedures: `sp_SaveMeasurement`, `sp_GetAllMeasurements`, `sp_GetMeasurementsByOperation`, `sp_GetMeasurementsByType`, `sp_GetTotalCount`, `sp_DeleteAllMeasurements`

### 2. Database Schema

| Column | Type | Notes |
|---|---|---|
| `id` | `INT` (PK, identity) | Auto-incremented |
| `operation` | `NVARCHAR(50)` | e.g. `COMPARE`, `ADD` |
| `first_value` | `FLOAT` | First operand value |
| `first_unit` | `NVARCHAR(50)` | First operand unit name |
| `second_value` | `FLOAT` | Second operand value (0 for Convert) |
| `second_unit` | `NVARCHAR(50)` | Second operand unit (NULL for Convert) |
| `result_value` | `FLOAT` | Result of the operation |
| `measurement_type` | `NVARCHAR(50)` | e.g. `LENGTH`, `WEIGHT` |
| `is_error` | `BIT` | 1 if the operation failed |
| `error_message` | `NVARCHAR(500)` | Populated on error, otherwise NULL |
| `created_at` | `DATETIME` | Auto-set to `GETDATE()` |

### 3. Update the Connection String

Edit `QuantityMeasurementApp/appsettings.json`:

```json
{
  "Database": {
    "Provider": "sqlserver",
    "ConnectionString": "Server=localhost\\SQLEXPRESS;Database=QuantityMeasurementAppDB;Trusted_Connection=True;TrustServerCertificate=True;",
    "PoolSize": 5,
    "ConnectionTimeout": 30
  },
  "Repository": {
    "Type": "DataBase"
  }
}
```

Change `Server=` to match your SQL Server instance name. Set `Repository:Type` to `"Cache"` to skip the database entirely.

---

## Configuration

### appsettings.json Settings

| Key | Default | Description |
|---|---|---|
| `Database:ConnectionString` | SQL Express local | Full ADO.NET connection string |
| `Database:PoolSize` | `5` | Maximum connections in the pool |
| `Database:ConnectionTimeout` | `30` | Seconds before a connection attempt times out |
| `Repository:Type` | `cache` | `DataBase` to use SQL Server, `Cache` for in-memory |

---

## Repository Fallback Behaviour

At startup, `QuantityMeasurementApp.cs` reads `Repository:Type` from `appsettings.json`:

- **`DataBase`** → attempts to connect to SQL Server. If the connection succeeds, `QuantityMeasurementDatabaseRepository` is used. If SQL Server is unreachable, the app automatically falls back to `QuantityMeasurementCacheRepository` and prints a warning.
- **`Cache`** → skips the database entirely and uses `QuantityMeasurementCacheRepository` from the start.

The cache repository stores records in memory and also writes a `QuantityMeasurement.json` audit file next to the executable after every save.

---

## Connection Pool

`ConnectionPool` is a custom singleton that manages a stack of `SqlConnection` objects:

- Pre-warms 2 connections at startup so the first operations don't wait for a handshake
- Uses LIFO (stack) so the most recently returned — and least likely to have timed out — connection is always reused first
- Re-opens a connection automatically if it has dropped while idle
- Creates an emergency connection beyond the pool limit rather than failing an operation if all connections are exhausted
- Exposes `GetPoolStatistics()` which reports total created, idle, and max counts
- Provides a `Reset()` method used by tests to tear down and recreate the pool between test runs

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 35 tests cover:

- Build and assembly loading verification
- `ApplicationConfig` loading and default fallback values
- `ConnectionPool` initialization, acquire/release, and exhaustion handling
- `QuantityMeasurementDatabaseRepository` — Save, GetAll, GetByOperation, GetByMeasurementType, GetTotalCount, DeleteAll
- SQL injection prevention via parameterized queries
- Transaction rollback on null entity
- Database schema verification (table + all 6 stored procedures exist)
- Concurrent access across 4 threads with 5 saves each
- Large dataset performance (1000 records retrieved in under 5 seconds)
- Cache repository isolation and singleton behaviour
- Repository factory — correct type returned for both database and cache
- Service + database repository integration test
- Resource cleanup — connection returned to pool after every operation
- Batch insert of 20 records without exhausting the pool

> Tests that require SQL Server are marked `Assert.Inconclusive` when SQL Server is unavailable, so the test run never fails due to a missing database.

---

## Project Architecture

```
Console Input
     │
     ▼
QuantityMeasurementController   (reads input, calls service, prints output)
     │
     ▼
QuantityMeasurementServiceImpl  (validates, converts, computes, saves to repo)
     │
     ▼
IQuantityMeasurementRepository
     │
     ├── QuantityMeasurementDatabaseRepository   (ADO.NET + stored procedures + ConnectionPool)
     └── QuantityMeasurementCacheRepository      (in-memory List + JSON backup file)
```

All layers depend on interfaces so the repository implementation can be swapped via config without touching business logic.
