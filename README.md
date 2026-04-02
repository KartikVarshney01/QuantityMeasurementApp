# Quantity Measurement Application — UC4: Yard and Centimeter Equality

A single-project .NET console application that extends the length equality system to support all four length units — Feet, Inch, **Yard**, and **Centimeter**. UC4 adds Yard and Centimeter to the `LengthUnit` enum and their conversion factors to `ToFeet`, enabling cross-unit equality comparisons across all unit combinations.

---

## Overview

UC4 adds two new units to the existing Feet/Inch equality system from UC1–UC3:

- `Yard` — 1 Yard = 3 Feet = 36 Inches
- `Centimeter` — 1 cm ≈ 0.393701 Inches (via the cm→inch→feet path)
- All four units are now members of `LengthUnit` and handled by the `ToFeet` extension method
- `Quantity.Equals` works across every combination — `1 Yard == 3 Feet == 36 Inch`, and `1 cm == 0.393701 Inch`
- The interactive console accepts all four unit names and compares any two quantities

---

## Project Structure

```
UC4-YardEquality/
│
├── QuantityMeasurementApp/
│   ├── Models/
│   │   ├── LengthUnit.cs               # LengthUnit enum (Feet, Inch, Yard, Centimeter) + ToFeet
│   │   └── Quantity.cs                 # Quantity class — Equals compares via base unit (Feet)
│   └── QuantityMeasurementAppMain.cs   # Interactive console — reads two quantities and compares them
│
└── QuantityMeasurementApp.Tests/
    └── LengthTests.cs                  # 17 MSTest tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project |
| Testing | MSTest |

---

## Supported Units and Conversion to Feet

| Unit | Conversion to Feet |
|---|---|
| `Feet` | × 1 |
| `Inch` | ÷ 12 |
| `Yard` | × 3 |
| `Centimeter` | × 0.393701 ÷ 12 (cm → inch → feet) |

Equality uses a floating-point tolerance of `0.0001` to handle precision differences across unit paths.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC4-YardEquality
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app prompts for two quantities and reports whether they are equal:

```
Enter first value: 1
Enter first unit (Feet/Inch/Yard/Centimeter): Yard
Enter second value: 36
Enter second unit (Feet/Inch/Yard/Centimeter): Inch
Equal (true)
```

Unit names are parsed case-insensitively.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 17 tests in `LengthTests.cs` cover:

**Yard equality (tests 1–7):**
- Yard == Yard same value → true
- Yard == Yard different value → false
- Yard == Feet equivalent (1 Yard == 3 Feet) → true, and symmetric (3 Feet == 1 Yard) → true
- Yard == Inch equivalent (1 Yard == 36 Inch) → true, and symmetric → true
- Yard == Feet non-equivalent (1 Yard ≠ 2 Feet) → false

**Centimeter equality (tests 8–9):**
- Centimeter == Inch equivalent (1 cm == 0.393701 Inch) → true
- Centimeter == Feet non-equivalent (1 cm ≠ 1 Feet) → false

**Cross-unit transitive property (tests 10, 17):**
- `1 Yard == 3 Feet && 3 Feet == 36 Inch && 1 Yard == 36 Inch` → true
- `2 Yard == 6 Feet && 6 Feet == 72 Inch && 2 Yard == 72 Inch` → true

**Null and reference checks (tests 11–16):**
- Yard with null unit → false
- Yard same reference → true
- Yard == null → false
- Centimeter with null unit → false
- Centimeter same reference → true
- Centimeter == null → false
