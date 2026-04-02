# Quantity Measurement Application — UC10: Generic Class

A single-project .NET application that refactors the quantity measurement system to use a single generic class `Quantity<U>` instead of separate `LengthQuantity` and `WeightQuantity` classes. UC10 is a structural redesign — the external behaviour (equality, conversion, addition) is identical to UC9, but all measurement categories now share one type-safe class constrained to `where U : Enum`.

---

## Overview

UC10 is a **refactoring and architectural release**. The key changes from UC9:

- `LengthQuantity` and `WeightQuantity` are replaced by a single `Quantity<U> where U : Enum` class
- The compiler enforces category safety — `Quantity<LengthUnit>` and `Quantity<WeightUnit>` are distinct types, so cross-category operations are caught at compile time, not just runtime
- All arithmetic (equality, conversion, addition) dispatches to the correct unit extension methods via a `ToBaseUnit()` type switch inside the generic class
- Adding a new measurement category in future (e.g. Volume, Temperature) only requires a new enum + extension class — `Quantity<U>` needs no changes beyond a new dispatch branch
- `GetHashCode` is overridden consistently with `Equals` — two quantities equal in base units share the same hash code

---

## Project Structure

```
UC10-GenericClass/
│
├── QuantityMeasurementApp/
│   ├── Interface/
│   │   └── IMeasurable.cs                      # Unit contract: conversion factor, to/from base, name
│   ├── Models/
│   │   └── Quantity.cs                         # Single generic Quantity<U> class — UC10 core change
│   ├── Units/
│   │   ├── LengthUnit.cs                       # Feet, Inch, Yard, Centimeter + extension methods
│   │   └── WeightUnit.cs                       # Kilogram, Gram, Pound + extension methods
│   └── QuantityMeasurementAppMain.cs           # Demo — Length and Weight via generic class
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTest.cs                         # 35 MSTest tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project, generic class + enum + extension method pattern |
| Testing | MSTest |

---

## Supported Units

| Category | Units | Base Unit |
|---|---|---|
| `LENGTH` | `Feet`, `Inch`, `Yard`, `Centimeter` | Feet |
| `WEIGHT` | `Kilogram`, `Gram`, `Pound` | Kilogram |

---

## What Changed from UC9

UC9 had two separate measurement classes. UC10 replaces them with one:

**Before (UC9):**
```csharp
var length = new LengthQuantity(1.0, LengthUnit.Feet);
var weight = new WeightQuantity(1.0, WeightUnit.Kilogram);
```

**After (UC10):**
```csharp
var length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
```

The type parameter `U` carries the category. Cross-category operations are blocked at compile time — you cannot pass a `Quantity<WeightUnit>` where a `Quantity<LengthUnit>` is expected without an explicit cast.

The `ToBaseUnit()` dispatch inside `Quantity<U>` uses a type pattern switch to route to the correct extension method:

```csharp
private double ToBaseUnit()
{
    if (Unit is LengthUnit lu)  return LengthUnitExtension.ConvertToBaseUnit(lu, Value);
    if (Unit is WeightUnit wu)  return WeightUnitExtension.ConvertToBaseUnit(wu, Value);
    throw new ArgumentException("Unsupported unit type");
}
```

Adding Volume in UC11 only required adding `VolumeUnit` as a new branch here — the rest of `Quantity<U>` was unchanged.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC10-GenericClass
dotnet build
```

### Run the Demo

```bash
cd QuantityMeasurementApp
dotnet run
```

The demo constructs `Quantity<LengthUnit>` and `Quantity<WeightUnit>` objects and prints equality, conversion, and addition results for both categories.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 35 tests in `QuantityTest.cs` cover:

- **IMeasurable interface** — `GetConversionFactor` returns 1.0 for base units (Feet, Kilogram); `GetUnitName` returns the correct string for both categories
- **Generic equality** — `Quantity<LengthUnit>` and `Quantity<WeightUnit>` equality checks with equivalent cross-unit values (`1 Feet == 12 Inch`, `1 Kilogram == 1000 Gram`)
- **Generic conversion** — `ConvertTo` for both categories, all unit combinations including Yard→Inch and Kilogram→Gram
- **Generic addition** — `Add` with explicit target unit for both categories, including mixed-unit addition
- **Cross-category prevention** — `Quantity<LengthUnit>.Equals(Quantity<WeightUnit>)` returns false; compiler type safety confirmed
- **Constructor validation** — null unit and NaN/Infinity value both throw `ArgumentException`
- **Hash code consistency** — two quantities equal in base units share the same hash code (`1 Feet` and `12 Inch` have equal hash codes)
- **Equals contract** — reflexive and transitive equality across three equivalent representations of 1 Foot
- **Immutability** — `ConvertTo` returns a new object; the original value is unchanged
- **`ToString` format** — `"Quantity(5, Pound)"` confirms the expected output format
- **Backward compatibility** — `3 Feet == 36 Inch`, `2 Kilogram == 2000 Gram`, and other UC1–UC9 assertions still pass
- **Scalability markers** — tests confirming the architecture is ready for new categories without modifying `Quantity<U>`
