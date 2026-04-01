# Quantity Measurement Application — UC13: Centralized Arithmetic

A single-project .NET application that refactors the UC12 arithmetic implementation by eliminating code duplication across Add, Subtract, and Divide. UC13 introduces two private helper methods — `ValidateArithmeticOperands` and `PerformBaseArithmetic` — so all three operations share a single validation path and a single base-unit computation path instead of repeating the same logic in each method.

---

## Overview

UC13 is a **refactoring release** — the external behaviour is identical to UC12. What changes is the internal structure of `Quantity<U>`:

- A private `ValidateArithmeticOperands` method centralises all null checks, NaN/infinity checks, cross-category checks, and target-unit checks in one place
- A private `PerformBaseArithmetic` method converts both operands to base units, then dispatches to the correct operation via an internal `ArithmeticOperation` enum
- `Add`, `Subtract`, and `Divide` are each reduced to a call to `ValidateArithmeticOperands` + `PerformBaseArithmetic` + convert the result back — no repeated logic
- Tests explicitly verify the private helper methods exist by name using reflection, confirming the DRY refactoring was actually applied

---

## Project Structure

```
UC13-CentralizedArithmetic/
│
├── QuantityMeasurementApp/
│   ├── Interface/
│   │   └── IMeasurable.cs                       # Unit contract: conversion factor, to/from base, name
│   ├── Models/
│   │   └── Quantity.cs                          # Generic Quantity<U> — centralized arithmetic helpers
│   ├── Units/
│   │   ├── LengthUnit.cs                        # Feet, Inch, Yard, Centimeter (base: Feet)
│   │   ├── WeightUnit.cs                        # Kilogram, Gram, Pound (base: Kilogram)
│   │   └── VolumeUnit.cs                        # Litre, Millilitre, Gallon (base: Litre)
│   └── QuantityMeasurementAppMain.cs            # Demo entry point — runs all three categories
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTest.cs                          # 40 MSTest tests
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

## What Changed from UC12

The public API is unchanged. The refactoring is entirely internal to `Quantity<U>`.

**Before (UC12):** Each of Add, Subtract, and Divide repeated the same null check, NaN check, cross-category check, and base-unit conversion inline.

**After (UC13):** Two private helpers eliminate that duplication.

`ValidateArithmeticOperands` — called at the top of every arithmetic method:

```csharp
private void ValidateArithmeticOperands(Quantity<U> other, U targetUnit, bool targetUnitRequired)
{
    if (other == null)
        throw new ArgumentException("Operand cannot be null");
    if (double.IsNaN(other.Value) || double.IsInfinity(other.Value))
        throw new ArgumentException("Invalid operand value");
    if (Unit.GetType() != other.Unit.GetType())
        throw new ArgumentException("Cross-category arithmetic not allowed");
    if (targetUnitRequired && targetUnit == null)
        throw new ArgumentException("Target unit cannot be null");
}
```

`PerformBaseArithmetic` — converts both operands to base units and dispatches via the internal `ArithmeticOperation` enum:

```csharp
private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
{
    double base1 = ToBaseUnit();
    double base2 = other.ToBaseUnit();
    return operation switch
    {
        ArithmeticOperation.ADD      => base1 + base2,
        ArithmeticOperation.SUBTRACT => base1 - base2,
        ArithmeticOperation.DIVIDE   => base2 == 0
                                        ? throw new ArithmeticException("Division by zero")
                                        : base1 / base2,
        _ => throw new ArgumentException("Invalid operation")
    };
}
```

Each public method then becomes a thin wrapper:

```csharp
public Quantity<U> Add(Quantity<U> other, U targetUnit)
{
    ValidateArithmeticOperands(other, targetUnit, true);
    double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);
    double result = ConvertFromBase(baseResult, targetUnit);
    return new Quantity<U>(Math.Round(result, 2), targetUnit);
}
```

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC13-CentralizedArithmetic
dotnet build
```

### Run the Demo

```bash
cd QuantityMeasurementApp
dotnet run
```

The demo runs Equality, Conversion, Addition, Subtraction, and Division for Length, Weight, and Volume and prints results to the console.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 40 tests in `QuantityTest.cs` cover:

- **Refactoring delegation** — Add, Subtract, and Divide all route through the helper correctly and return the right values
- **Validation consistency** — null operand, NaN/infinity value, cross-category, and null target unit all throw the expected exception type from a single path shared by all three operations
- **UC12 behaviour preserved** — `1 Feet + 12 Inch = 2 Feet`, `10 Feet − 6 Inch = 9.5 Feet`, `24 Inch ÷ 2 Feet = 1.0` all pass unchanged
- **Rounding** — Add/Subtract results are rounded to 2 decimal places; Divide returns the full ratio without rounding
- **Implicit vs explicit target unit** — calling `Add(other)` defaults the result unit to Q1's unit; calling `Add(other, targetUnit)` overrides it
- **Immutability** — the original quantity's `Value` is unchanged after any arithmetic call
- **All categories** — Length, Weight, and Volume all work through the same centralized path
- **Private helper verification** — reflection checks confirm `PerformBaseArithmetic` and `ValidateArithmeticOperands` exist as private instance methods, proving the DRY refactoring was applied
- **Error message consistency** — null operand error message contains `"Operand"` across all operations
- **Chained operations** — `q1.Add(q2).Subtract(q3).Divide(q4)` produces the correct result
- **Performance** — 100,000 iterations of all three operations complete in under 2 seconds
- **Large dataset** — 1,000 iterations create no errors or memory issues
