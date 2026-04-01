# Quantity Measurement Application — UC12: New Arithmetic (Subtract and Divide)

A single-project .NET application that extends the UC11 quantity measurement system by adding two new arithmetic operations: **Subtract** and **Divide**. Both operations work in base units so mixed-unit calculations (e.g. `10 Feet − 6 Inch`) produce correct results, and Divide always returns a dimensionless scalar ratio rather than a new quantity.

---

## Overview

UC12 adds two operations to the existing `Quantity<U>` class:

- `Subtract(other)` — subtracts Q2 from Q1 in base units and returns the result in Q1's unit
- `Subtract(other, targetUnit)` — same but converts the result to an explicit target unit
- `Divide(other)` — divides Q1 by Q2 in base units and returns a plain `double` ratio
- Division by zero throws `ArithmeticException`
- Both operations reject null operands and cross-category inputs with `ArgumentException`
- Results from Subtract are rounded to 2 decimal places; Divide returns the full precision ratio

---

## Project Structure

```
UC12-NewArithmetic/
│
├── QuantityMeasurementApp/
│   ├── Interface/
│   │   └── IMeasurable.cs                      # Unit contract: conversion factor, to/from base, name
│   ├── Models/
│   │   └── Quantity.cs                         # Generic Quantity<U> with Add, Subtract, Divide, ConvertTo
│   ├── Units/
│   │   ├── LengthUnit.cs                       # Feet, Inch, Yard, Centimeter (base: Feet)
│   │   ├── WeightUnit.cs                       # Kilogram, Gram, Pound (base: Kilogram)
│   │   └── VolumeUnit.cs                       # Litre, Millilitre, Gallon (base: Litre)
│   └── QuantityMeasurementAppMain.cs           # Demo entry point — runs Subtract and Divide for all categories
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTest.cs                         # 40 MSTest tests
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

## Operations

### Subtract

Converts both operands to base units, subtracts, then converts the result back to the target unit (defaults to Q1's unit if no target is specified).

```csharp
// Same unit
new Quantity<LengthUnit>(10, LengthUnit.Feet)
    .Subtract(new Quantity<LengthUnit>(5, LengthUnit.Feet));
// → Quantity(5, Feet)

// Mixed units — result in Q1's unit
new Quantity<LengthUnit>(10, LengthUnit.Feet)
    .Subtract(new Quantity<LengthUnit>(6, LengthUnit.Inch));
// → Quantity(9.5, Feet)

// Mixed units — explicit target unit
new Quantity<LengthUnit>(10, LengthUnit.Feet)
    .Subtract(new Quantity<LengthUnit>(6, LengthUnit.Inch), LengthUnit.Inch);
// → Quantity(114, Inch)

// Negative result is valid
new Quantity<LengthUnit>(5, LengthUnit.Feet)
    .Subtract(new Quantity<LengthUnit>(10, LengthUnit.Feet));
// → Quantity(-5, Feet)
```

### Divide

Converts both operands to base units and returns the ratio as a plain `double`. There is no unit on the result.

```csharp
// Same unit
new Quantity<LengthUnit>(10, LengthUnit.Feet)
    .Divide(new Quantity<LengthUnit>(2, LengthUnit.Feet));
// → 5.0

// Mixed units — base-unit ratio
new Quantity<LengthUnit>(24, LengthUnit.Inch)
    .Divide(new Quantity<LengthUnit>(2, LengthUnit.Feet));
// → 1.0  (24 inches == 2 feet, so ratio is 1)

// Division by zero
new Quantity<LengthUnit>(10, LengthUnit.Feet)
    .Divide(new Quantity<LengthUnit>(0, LengthUnit.Feet));
// → throws ArithmeticException("Division by zero")
```

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC12-NewArithmetic
dotnet build
```

### Run the Demo

```bash
cd QuantityMeasurementApp
dotnet run
```

The demo runs Subtract and Divide for Length, Weight, and Volume and prints results to the console.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 40 tests in `QuantityTest.cs` cover:

**Subtract (tests 1–19):**
- Same unit, same category — Feet−Feet, Litre−Litre
- Mixed units — Feet−Inch, Inch−Feet, result in explicit target unit (Feet or Inch or Millilitre)
- Negative result, zero result, zero operand, negative operand values
- Non-commutativity — `a−b ≠ b−a`
- Large and small values
- Null operand throws `ArgumentException`
- Null target unit throws an exception
- Cross-category (Length−Weight forced via cast) throws an exception
- All three categories in one test
- Chained subtraction

**Divide (tests 20–34):**
- Same unit — Feet÷Feet, Litre÷Litre
- Mixed units — Inch÷Feet, Kilogram÷Gram
- Ratio greater than one, less than one, equal to one
- Non-commutativity — `a÷b ≠ b÷a`
- Division by zero throws `ArithmeticException`
- Large ratio (10⁶), small ratio (10⁻⁶)
- Null operand throws `ArgumentException`
- Cross-category throws an exception
- All three categories in one test
- Non-associativity confirmed

**Integration and correctness (tests 35–40):**
- Subtract then Divide in one chain — `(10 Feet − 2 Feet) ÷ 2 Feet = 4`
- Add then Subtract inverse — `a + b − b = a`
- Immutability — original quantity unchanged after Subtract and after Divide
- Subtract rounding — result rounded to 2 decimal places
- Divide precision — full double precision returned, no forced rounding
