# Quantity Measurement Application — UC6: Unit Addition

A single-project .NET console application that extends the length measurement system with **addition**. UC6 introduces `Quantity.Add`, a static method that adds two quantities of any supported length unit by converting both to the base unit (Feet), summing them, and returning the result in the first operand's unit.

---

## Overview

UC6 adds one new capability to the existing `Quantity` class:

- `Add(q1, q2)` — converts both operands to Feet, adds them, and returns the result in q1's unit
- Mixed-unit addition works correctly — `1 Feet + 12 Inch = 2 Feet`, `12 Inch + 1 Feet = 24 Inch`
- The result unit is always the first operand's unit (UC7 will add an explicit target unit option)
- Null operands throw `ArgumentException`
- The interactive console `Main` prompts for two quantities and prints the sum

---

## Project Structure

```
UC6-UnitAddition/
│
├── QuantityMeasurementApp/
│   ├── Models/
│   │   ├── LengthUnit.cs               # LengthUnit enum + ToFeet extension method
│   │   └── Quantity.cs                 # Quantity class with Add, Convert, ConvertTo, Equals
│   └── QuantityMeasurementAppMain.cs   # Interactive console — reads two quantities and adds them
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTests.cs                # 12 MSTest tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project |
| Testing | MSTest |

---

## Supported Units

| Unit | Conversion to Feet (base) |
|---|---|
| `Feet` | 1.0 (base unit) |
| `Inch` | value ÷ 12 |
| `Yard` | value × 3 |
| `Centimeter` | (value × 0.393701) ÷ 12 |

---

## How Addition Works

All arithmetic is performed in the base unit (Feet):

```
q1 → Feet  +  q2 → Feet  =  sumInFeet  →  q1's unit
```

For example, `12 Inch + 1 Feet`:
1. 12 Inch → 1.0 Feet
2. 1 Feet → 1.0 Feet
3. Sum = 2.0 Feet
4. Convert 2.0 Feet back to Inch (q1's unit) → 24 Inch

The result unit is always determined by q1, so order matters for the unit label — though the magnitude is always the same when expressed in any equivalent unit.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC6-UnitAddition
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app prompts for two quantities and prints their sum in the first quantity's unit:

```
Enter First Quantity
Value: 1
Unit (Feet/Inch/Yard/Centimeter): Feet

Enter Second Quantity
Value: 12
Unit (Feet/Inch/Yard/Centimeter): Inch

Result: 2 Feet
```

Unit names are parsed case-insensitively.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 12 tests in `QuantityTests.cs` cover:

- **Same unit addition** — `1 Feet + 2 Feet = 3 Feet`, `6 Inch + 6 Inch = 12 Inch`
- **Cross-unit addition** — `1 Feet + 12 Inch = 2 Feet` (result in Feet), `12 Inch + 1 Feet = 24 Inch` (result in Inch)
- **Yard and Centimeter** — `1 Yard + 3 Feet = 2 Yard`, `2.54 cm + 1 Inch = 5.08 cm`
- **Commutativity** — `Add(q1, q2)` and `Add(q2, q1)` produce equal results (though in different units)
- **Adding zero** — `5 Feet + 0 Inch = 5 Feet`
- **Negative operand** — `5 Feet + (−2 Feet) = 3 Feet`
- **Null operand** — throws `ArgumentException`
- **Large values** — `1,000,000 Feet + 1,000,000 Feet = 2,000,000 Feet`
- **Small values** — `0.001 Feet + 0.002 Feet = 0.003 Feet`
