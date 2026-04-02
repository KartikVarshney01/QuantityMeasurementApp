# Quantity Measurement Application — UC9: Weight Equality

A single-project .NET console application that adds a complete **weight measurement** category alongside the existing length measurement. UC9 introduces `QuantityWeight` with three units (Kilogram, Gram, Pound) and full support for equality comparison, unit conversion, and addition — mirroring the design of `QuantityLength` from UC1–UC8.

---

## Overview

UC9 extends the system by adding weight as a second independent measurement category:

- A `WeightUnit` enum (`Kilogram`, `Gram`, `Pound`) with `ConvertToBaseUnit` and `ConvertFromBaseUnit` extension methods
- A `QuantityWeight` class with the same API shape as `QuantityLength` — `Equals`, `ConvertTo`, and a static `Add` method (both implicit and explicit target unit overloads)
- An interactive console `Main` that walks the user through weight equality, conversion, and addition at runtime
- Two separate test files — `QuantityWeightTests.cs` (weight-focused) and `QuantityLengthTests.cs` (backward-compatibility checks for UC1–UC8)
- Cross-category incompatibility confirmed — `QuantityWeight.Equals(QuantityLength)` always returns false

---

## Project Structure

```
UC9-WeightEquality/
│
├── QuantityMeasurementApp/
│   ├── Models/
│   │   ├── QuantityLength.cs      # Length quantity (Feet, Inch, Yard, Centimeter)
│   │   ├── QuantityWeight.cs      # Weight quantity (Kilogram, Gram, Pound) — UC9 addition
│   │   ├── LengthUnit.cs          # LengthUnit enum + ConvertToBaseUnit / ConvertFromBaseUnit
│   │   └── WeightUnit.cs          # WeightUnit enum + ConvertToBaseUnit / ConvertFromBaseUnit
│   └── QuantityMeasurementAppMain.cs   # Interactive console — weight equality, conversion, addition
│
└── QuantityMeasurementApp.Tests/
    ├── QuantityWeightTests.cs     # 28 weight tests
    └── QuantityLengthTests.cs     # 24 length / backward-compatibility tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project, two measurement classes + enum extension methods |
| Testing | MSTest |

---

## Supported Units

| Category | Class | Units | Base Unit |
|---|---|---|---|
| Length | `QuantityLength` | `Feet`, `Inch`, `Yard`, `Centimeter` | Feet |
| Weight | `QuantityWeight` | `Kilogram`, `Gram`, `Pound` | Kilogram |

### Weight Conversion Factors

| Unit | Factor (to Kilogram) |
|---|---|
| `Kilogram` | 1.0 |
| `Gram` | 0.001 |
| `Pound` | 0.453592 |

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC9-WeightEquality
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app prompts for two weight values and units, compares them, then walks through conversion and addition interactively. Unit names are parsed case-insensitively (`Kilogram`, `kilogram`, `KILOGRAM` all work).

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

**`QuantityWeightTests.cs` — 28 weight tests:**

- Equality — same unit same value, same unit different value, Kilogram↔Gram, Gram↔Kilogram cross-unit equivalence
- Cross-category — `QuantityWeight.Equals(QuantityLength)` returns false
- Null comparison returns false; same reference returns true; null unit throws `ArgumentException`
- Transitive property — `a==b && b==c → a==c`
- Zero, negative, large (1,000,000 g == 1,000 kg), and small (0.001 kg == 1 g) values
- Conversion — Pound→Kilogram, Kilogram→Pound, same unit, zero value, negative value, round-trip precision
- Addition — same unit (Kg+Kg), cross-unit (Kg+g), Pound+Kilogram cross-unit, explicit target unit override
- Addition commutativity, adding zero, negative values, large values (10⁶ kg + 10⁶ kg)

**`QuantityLengthTests.cs` — 24 length / backward-compatibility tests:**

- `ConvertToBaseUnit` for all four length units (Feet, Inch, Yard, Centimeter)
- `ConvertFromBaseUnit` for all four length units
- `QuantityLength` equality, `ConvertTo`, `Add` (implicit and explicit target unit)
- Null unit and NaN value rejected on construction
- Backward-compatibility checks — UC1 equality, UC5 conversion, UC6 addition, UC7 addition with target unit all pass unchanged
- Round-trip conversion — 5 Feet → Inches → back to Feet returns 5.0
- Unit immutability and architecture scalability markers

---

## Project Architecture

```
QuantityMeasurementAppMain
        │
        ├── QuantityWeight  ←──  WeightUnit (Kilogram | Gram | Pound)
        │       Equals / ConvertTo / Add (static)
        │
        └── QuantityLength  ←──  LengthUnit (Feet | Inch | Yard | Centimeter)
                Equals / ConvertTo / Add (static)
```

Both classes are independent and share no base class. Cross-category comparison is blocked by the `GetType()` check inside `Equals` — a `QuantityWeight` and a `QuantityLength` with the same numeric value will never be considered equal.
