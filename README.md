# Quantity Measurement Application — UC15: N-Tier Architecture

A .NET console application that performs quantity measurement operations (Convert, Compare, Add, Subtract, Divide) across four measurement categories, built with a clean N-Tier architecture. Every operation is persisted to an in-memory cache repository and the full operation history is viewable from the main menu.

---

## Overview

UC15 introduces a proper layered architecture by separating the codebase into four independent projects:

- A **Presentation layer** (`QuantityMeasurementApp`) with a console controller and `IQuantityMeasurementApp` interface
- A **Business layer** (`QuantityMeasurementAppBusinessLayer`) with the service, the generic `Quantity<U>` domain class, and unit extension methods
- A **Repository layer** (`QuantityMeasurementAppRepoLayer`) with a thread-safe singleton `QuantityMeasurementCacheRepository`
- A **Model layer** (`QuantityMeasurementAppModelLayer`) with shared DTOs, entities, and enums

All layers communicate through interfaces so no layer depends directly on a concrete class from another.

---

## Solution Structure

```
UC15-NTierArchitecture/
│
├── QuantityMeasurementApp/                              # Console entry point
│   ├── Controller/QuantityMeasurementController.cs     # Menu, input, Perform* methods
│   ├── Interface/IQuantityMeasurementApp.cs            # App contract (Run)
│   └── Program.cs                                      # Wires all layers, calls app.Run()
│
├── QuantityMeasurementAppBusinessLayer/                 # Domain logic
│   ├── Exception/QuantityMeasurementException.cs
│   ├── Extensions/
│   │   ├── LengthUnitExtension.cs
│   │   ├── WeightUnitExtension.cs
│   │   ├── VolumeUnitExtension.cs
│   │   └── TemperatureUnitExtension.cs
│   ├── Interface/
│   │   ├── IMeasurable.cs
│   │   └── IQuantityMeasurementService.cs
│   └── Service/
│       ├── Quantity.cs                                  # Generic typed quantity with arithmetic
│       └── QuantityMeasurementServiceImpl.cs
│
├── QuantityMeasurementAppModelLayer/                    # Shared data types
│   ├── Dto/QuantityDTO.cs
│   ├── Entities/QuantityMeasurementEntity.cs
│   └── Enum/
│       ├── LengthUnit.cs
│       ├── WeightUnit.cs
│       ├── VolumeUnit.cs
│       └── TemperatureUnit.cs
│
├── QuantityMeasurementAppRepoLayer/                     # Data access
│   ├── Interface/IQuantityMeasurementRepository.cs
│   └── Services/QuantityMeasurementCacheRepository.cs  # Thread-safe singleton in-memory store
│
└── QuantityMeasurementApp.Tests/                        # MSTest suite — 40 tests
    └── NTierTests.cs
```

---

## Tech Stack

| Layer | Technology |
|---|---|
| Console App | .NET (C#) |
| Architecture | N-Tier (4 separate projects) |
| Repository | In-memory cache (singleton, thread-safe) |
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

| Category | Units | Base Unit |
|---|---|---|
| `LENGTH` | `Feet`, `Inch`, `Yard`, `Centimeter` | Feet |
| `WEIGHT` | `Kilogram`, `Gram`, `Pound` | Kilogram |
| `VOLUME` | `Litre`, `Millilitre`, `Gallon` | Litre |
| `TEMPERATURE` | `Celsius`, `Fahrenheit`, `Kelvin` | Celsius |

Unit names are case-insensitive. Common abbreviations like `kg`, `ft`, `cm`, `ml` are also accepted.

---

## Getting Started

### Prerequisites

- .NET SDK

### Clone and Build

```bash
git clone <your-repo-url>
cd UC15-NTierArchitecture
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app starts an interactive menu loop. Use option **5 — Operation History** at any time to view every operation performed in the current session.

---

## Console Menu

```
╔══════════════════════════════════════╗
║   Quantity Measurement Application   ║
╚══════════════════════════════════════╝

===== MAIN MENU =====
1. Length
2. Weight
3. Volume
4. Temperature
5. Operation History
0. Exit
```

Each category sub-menu offers Convert, Compare, Add, Subtract, and Divide (Temperature only offers Convert and Compare).

---

## How the Layers Connect

Dependency wiring happens entirely in `Program.cs` — no framework, no config file:

```csharp
IQuantityMeasurementRepository repository = QuantityMeasurementCacheRepository.Instance;
IQuantityMeasurementService    service    = new QuantityMeasurementServiceImpl(repository);
IQuantityMeasurementApp        app        = new QuantityMeasurementController(service, repository);

app.Run();
```

Every layer depends only on the interface from the layer below it, never on a concrete class.

---

## Repository

`QuantityMeasurementCacheRepository` is a thread-safe singleton that stores `QuantityMeasurementEntity` records in a `List` for the lifetime of the process. It exposes three methods:

- `Save(entity)` — adds a record (both success and error results are saved)
- `GetAllMeasurements()` — returns a read-only snapshot of all records
- `Clear()` — empties the list (used between tests)

There is no persistence to disk — data lives only in memory for the current run.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 40 tests in `NTierTests.cs` cover:

- `QuantityMeasurementEntity` construction — single operand, binary operand, error record, and `ToString` formatting
- Service layer — Compare (same unit, different unit, cross-category error), Convert, Add, Subtract, Divide, divide-by-zero, and temperature arithmetic rejection
- Controller layer — `PerformComparison`, `PerformConversion`, `PerformAddition` success and error paths, result format verification
- Layer separation and interface decoupling checks
- End-to-end integration for length addition and temperature unsupported operation
- Structural tests verifying scalability and backward compatibility markers

---

## Project Architecture

```
Program.cs  (wires interfaces only)
     │
     ▼
IQuantityMeasurementApp
     │
     ▼
QuantityMeasurementController    (presentation — reads console input, formats output)
     │
     ▼
IQuantityMeasurementService
     │
     ▼
QuantityMeasurementServiceImpl   (business — validates, resolves units, computes, saves)
     │              │
     ▼              ▼
  Quantity<U>     IQuantityMeasurementRepository
  (domain)               │
                         ▼
              QuantityMeasurementCacheRepository
              (in-memory singleton)
```
