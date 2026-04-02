# Quantity Measurement Application — UC5: Unit Conversion

A single-project .NET console application that extends the length measurement system with **unit conversion**. UC5 introduces two conversion methods — a static `Quantity.Convert` and an instance `ConvertTo` — that convert any supported length unit to any other by routing through Feet as the common base unit.

---

## Overview

UC5 adds conversion to the existing `Quantity` class:

- `Quantity.Convert(value, source, target)` — static method, converts a raw `double` between any two `LengthUnit` values
- `q.ConvertTo(target)` — instance method, delegates to `Convert` using the quantity's own value and unit
- Both methods go via Feet internally: source → Feet → target
- Null units, invalid enum values, NaN, and Infinity all throw `ArgumentException`
- The interactive console prompts for a value, source unit, and target unit, then prints the result from both methods

---

## Project Structure

```
UC5-UnitConversion/
│
├── QuantityMeasurementApp/
│   ├── Models/
│   │   ├── LengthUnit.cs               # LengthUnit enum + ToFeet extension method
│   │   └── Quantity.cs                 # Quantity class — Convert (static) + ConvertTo (instance)
│   └── QuantityMeasurementAppMain.cs   # Interactive console — value, source unit, target unit
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTests.cs                # 13 MSTest tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project |
| Testing | MSTest |

---

## Supported Units and Conversion Factors

All conversions go through Feet as the base unit.

| Unit | To Feet | From Feet |
|---|---|---|
| `Feet` | × 1 | × 1 |
| `Inch` | ÷ 12 | × 12 |
| `Yard` | × 3 | ÷ 3 |
| `Centimeter` | × 0.393701 ÷ 12 | × 12 ÷ 0.393701 |

---

## How Conversion Works

Every conversion uses Feet as the intermediate step:

```
source value  →  ToFeet()  →  Feet  →  switch(target)  →  result
```

For example, `1 Yard → Inch`:
1. 1 Yard × 3 = 3 Feet
2. 3 Feet × 12 = 36 Inch

This two-step approach means adding a new unit only requires updating `ToFeet` and the target switch — no other code changes needed.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC5-UnitConversion
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app prompts for a value and two units, then prints the converted result from both the static and instance methods:

```
Enter value: 1
Enter source unit (Feet/Inch/Yard/Centimeter): Feet
Enter target unit (Feet/Inch/Yard/Centimeter): Inch
Converted Value: 12
(Using ConvertTo) → 12
```

Unit names are parsed case-insensitively.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 13 tests in `QuantityTests.cs` cover:

- **Direct conversions** — Feet→Inch, Inch→Feet, Yard→Inch, Inch→Yard, Centimeter→Inch, Feet→Yard
- **Round-trip precision** — `5 Feet → Inch → Feet` returns 5.0 within tolerance
- **Zero value** — `0 Feet → Inch = 0`
- **Negative value** — `-1 Feet → Inch = -12`
- **Invalid unit** — casting an out-of-range integer to `LengthUnit` throws `ArgumentException` for both source and target
- **NaN and Infinity** — all three invalid double values (NaN, PositiveInfinity, NegativeInfinity) throw `ArgumentException`
- **Centimeter precision** — `1 cm → Inch` result is within 0.0001 of 0.393701
