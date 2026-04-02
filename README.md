# Quantity Measurement Application — UC7: Target Unit Addition

A single-project .NET console application that extends the UC6 addition feature by allowing the caller to specify an **explicit target unit** for the result. Instead of always returning the sum in the first operand's unit, `Add` now accepts a third `targetUnit` parameter so the result can be expressed in any supported unit — Feet, Inch, Yard, or Centimeter.

---

## Overview

UC7 adds one new overload to the existing `Quantity.Add` method:

- `Add(q1, q2)` — original UC6 behaviour, result in q1's unit (kept for backward compatibility)
- `Add(q1, q2, targetUnit)` — UC7 addition, result expressed in the explicitly chosen unit

Both overloads convert operands to Feet internally, sum in base units, then convert the result to the target unit before returning. The interactive console `Main` is updated to prompt the user for the target unit as well.

---

## Project Structure

```
UC7-TargetUnitAddition/
│
├── QuantityMeasurementApp/
│   ├── Models/
│   │   ├── LengthUnit.cs               # LengthUnit enum + ToFeet extension method
│   │   └── Quantity.cs                 # Quantity class — Add(q1, q2) and Add(q1, q2, targetUnit)
│   └── QuantityMeasurementAppMain.cs   # Interactive console — prompts for two quantities + target unit
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTests.cs                # 14 MSTest tests
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

## What Changed from UC6

UC6's `Add` always returned the result in the first operand's unit. UC7 adds an overload that lets the caller choose:

```csharp
// UC6 — result always in q1's unit
Quantity result = Quantity.Add(q1, q2);

// UC7 — result in any unit the caller picks
Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);
```

Both overloads coexist — `Add(q1, q2)` delegates to `Add(q1, q2, q1.Unit)` internally, so UC6 behaviour is fully preserved.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC7-TargetUnitAddition
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app prompts for two quantities and then asks for the target unit the result should be expressed in:

```
Enter First Quantity
Value: 1
Unit (Feet/Inch/Yard/Centimeter): Feet

Enter Second Quantity
Value: 12
Unit (Feet/Inch/Yard/Centimeter): Inch

Enter target unit for result (Feet/Inch/Yard/Centimeter): Yard

Result: 0.6667 Yard
```

Unit names are parsed case-insensitively.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 14 tests in `QuantityTests.cs` all exercise the new `Add(q1, q2, targetUnit)` overload:

- **Target unit = Feet** — `1 Feet + 12 Inch = 2 Feet`
- **Target unit = Inch** — `1 Feet + 12 Inch = 24 Inch`
- **Target unit = Yard** — `1 Feet + 12 Inch = 0.6667 Yard`
- **Target unit = Centimeter** — `1 Inch + 1 Inch = 5.08 cm`
- **Target = same as first operand** — `2 Yard + 3 Feet = 3 Yard`
- **Target = same as second operand** — `2 Yard + 3 Feet = 9 Feet`
- **Commutativity with explicit target** — `Add(q1, q2, Yard) == Add(q2, q1, Yard)`
- **Adding zero** — `5 Feet + 0 Inch = 1.6667 Yard`
- **Negative operand** — `5 Feet + (−2 Feet) = 36 Inch`
- **Null target unit** — throws `ArgumentException`
- **Large to small scale** — `1000 Feet + 500 Feet = 18000 Inch`
- **Small to large scale** — `12 Inch + 12 Inch = 0.6667 Yard`
- **All four target units in one test** — `1 Feet + 12 Inch` expressed in Feet, Inch, Yard, and Centimeter all checked in a single test
- **Precision tolerance** — `2.54 cm + 1 Inch = 5.08 cm` within floating-point tolerance
