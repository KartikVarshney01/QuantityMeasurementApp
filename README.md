# Quantity Measurement Application — UC11: Volume Measurement

A single-project .NET application that extends the quantity measurement system to support volume as a third measurement category. UC11 adds `VolumeUnit` (Litre, Millilitre, Gallon) with full support for equality comparison, unit conversion, and addition — all working through the existing generic `Quantity<U>` class without any changes to the Length or Weight implementation.

---

## Overview

UC11 integrates volume into the existing system by:

- Adding a `VolumeUnit` enum with three units: `Litre`, `Millilitre`, and `Gallon`
- Adding `VolumeUnitExtension` methods — `GetConversionFactor`, `ConvertToBaseUnit`, `ConvertFromBaseUnit`, `GetUnitName` — following the same pattern as Length and Weight
- Registering `VolumeUnit` in the `ToBaseUnit()` and `ConvertFromBase()` dispatch inside `Quantity<U>` so all existing operations (Equals, ConvertTo, Add) work automatically for volume
- A 50-test MSTest suite focused entirely on volume, with a backward-compatibility test confirming UC1–UC10 (Length and Weight) behaviour is unaffected

---

## Project Structure

```
UC11-VolumeMeasurement/
│
├── QuantityMeasurementApp/
│   ├── Interface/
│   │   └── IMeasurable.cs                      # Unit contract: conversion factor, to/from base, name
│   ├── Models/
│   │   └── Quantity.cs                         # Generic Quantity<U> — unchanged from UC10
│   ├── Units/
│   │   ├── LengthUnit.cs                       # Feet, Inch, Yard, Centimeter (base: Feet)
│   │   ├── WeightUnit.cs                       # Kilogram, Gram, Pound (base: Kilogram)
│   │   └── VolumeUnit.cs                       # Litre, Millilitre, Gallon (base: Litre) — UC11 addition
│   └── QuantityMeasurementAppMain.cs           # Demo — runs all three categories
│
└── QuantityMeasurementApp.Tests/
    └── VolumeQuantityTest.cs                   # 50 MSTest tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project, enum + extension method pattern |
| Testing | MSTest |

---

## Supported Units

| Category | Units | Base Unit |
|---|---|---|
| `LENGTH` | `Feet`, `Inch`, `Yard`, `Centimeter` | Feet |
| `WEIGHT` | `Kilogram`, `Gram`, `Pound` | Kilogram |
| `VOLUME` | `Litre`, `Millilitre`, `Gallon` | Litre |

---

## Volume Conversion Factors

| Unit | Factor (relative to Litre) |
|---|---|
| `Litre` | 1.0 |
| `Millilitre` | 0.001 |
| `Gallon` | 3.78541 (US liquid gallon) |

All conversions use `value × factor` to reach Litres, and `baseValue ÷ factor` to convert back — the same multiply/divide pattern used for Length and Weight.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC11-VolumeMeasurement
dotnet build
```

### Run the Demo

```bash
cd QuantityMeasurementApp
dotnet run
```

The demo runs Equality, Conversion, and Addition for Length, Weight, and Volume and prints results to the console. Volume examples include Litre↔Millilitre and Gallon↔Litre conversions and cross-unit addition.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 50 tests in `VolumeQuantityTest.cs` cover:

**Equality (tests 1–16):**
- Same unit same value, same unit different value
- Litre↔Millilitre and Millilitre↔Litre cross-unit equivalence
- Litre↔Gallon and Gallon↔Litre cross-unit equivalence
- Volume vs Length and Volume vs Weight — confirmed incompatible
- Null comparison, same reference, null unit rejected on construction
- Transitive property — `a==b && b==c → a==c`
- Zero value, negative value, large value (1,000,000 mL == 1,000 L), small value (0.001 L == 1 mL)

**Conversion (tests 17–25):**
- Litre→Millilitre, Millilitre→Litre
- Gallon→Litre, Litre→Gallon
- Millilitre→Gallon
- Same unit conversion, zero value, negative value, round-trip precision

**Addition (tests 26–38):**
- Same unit — Litre+Litre, Millilitre+Millilitre
- Cross-unit — Litre+Millilitre (result in Litre), Millilitre+Litre (result in Millilitre)
- Gallon+Litre cross-unit
- Explicit target unit — result in Litre, Millilitre, or Gallon
- Commutativity — `q1+q2 == q2+q1`
- Adding zero, negative values, large values (10⁶ L), small values (0.001 L)

**Extension method unit tests (tests 39–47):**
- `GetConversionFactor` returns correct value for Litre (1.0), Millilitre (0.001), Gallon (3.78541)
- `ConvertToBaseUnit` for all three units
- `ConvertFromBaseUnit` for all three units

**Integration and backward compatibility (tests 48–50):**
- UC1–UC10 backward compatibility — `1 Feet == 12 Inch` and `1 Kilogram == 1000 Gram` still pass
- Generic quantity volume operations consistency
- Scalability check — `Quantity<VolumeUnit>` with Gallon constructs without error
