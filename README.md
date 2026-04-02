# Quantity Measurement Application — UC8: Standalone Unit

A single-project .NET console application that refactors the length measurement system by moving all unit conversion logic out of the `Quantity` class and into a standalone `LengthUnit` enum with extension methods. UC8 is a structural redesign — the external behaviour (equality, conversion, addition) is identical to UC7, but unit knowledge now lives entirely in `LengthUnit.cs` rather than inside `Quantity`.

---

## Overview

UC8 is a **refactoring release**. The key change from UC7:

- `LengthUnit` becomes a self-contained enum with two extension methods — `ConvertToBaseUnit` and `ConvertFromBaseUnit` — so the enum itself knows how to convert without needing to be asked by `Quantity`
- `Quantity` becomes a thin shell: its `ToBaseUnit()`, `ConvertTo`, `Add`, and `Equals` methods all delegate straight to the extension methods on the enum value
- The interactive console `Main` now prompts for two quantities and a target unit, performs addition at runtime, and prints the result — replacing the hardcoded demo values from earlier UCs
- All conversion factors and formulas live in one place (`LengthUnit.cs`), making it easy to add or correct a unit without touching `Quantity`

---

## Project Structure

```
UC8-StandaloneUnit/
│
├── QuantityMeasurementApp/
│   ├── Models/
│   │   ├── LengthUnit.cs               # LengthUnit enum + ConvertToBaseUnit / ConvertFromBaseUnit
│   │   └── Quantity.cs                 # Thin wrapper — delegates all conversion to LengthUnit
│   └── QuantityMeasurementAppMain.cs   # Interactive console — reads two quantities and adds them
│
└── QuantityMeasurementApp.Tests/
    └── QuantityTests.cs                # 26 MSTest tests
```

---

## Tech Stack

| | |
|---|---|
| Language | C# / .NET |
| Architecture | Single project, enum + extension methods |
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

## What Changed from UC7

The public API is unchanged — `Quantity.Add`, `ConvertTo`, and `Equals` work identically. The refactoring is internal.

**Before (UC7):** Conversion logic was spread across `Quantity` — the class contained the switch statements that knew what 1 Inch equalled in Feet.

**After (UC8):** Conversion logic lives entirely in `LengthUnitExtension`:

```csharp
public static double ConvertToBaseUnit(this LengthUnit unit, double value)
{
    switch (unit)
    {
        case LengthUnit.Feet:        return value;
        case LengthUnit.Inch:        return value / 12.0;
        case LengthUnit.Yard:        return value * 3.0;
        case LengthUnit.Centimeter:  return (value * 0.393701) / 12.0;
        ...
    }
}
```

`Quantity` then calls it with a single line:

```csharp
private double ToBaseUnit() => Unit!.Value.ConvertToBaseUnit(Value);
```

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC8-StandaloneUnit
dotnet build
```

### Run the Console App

```bash
cd QuantityMeasurementApp
dotnet run
```

The app prompts for two quantity inputs and a target unit, then adds them and prints the result:

```
Enter First Quantity
Value: 1
Unit (Feet/Inch/Yard/Centimeter): Feet

Enter Second Quantity
Value: 12
Unit (Feet/Inch/Yard/Centimeter): Inch

Enter target unit (Feet/Inch/Yard/Centimeter): Feet

Result: 2 Feet
```

Unit names are parsed case-insensitively (`feet`, `FEET`, and `Feet` all work).

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 26 tests in `QuantityTests.cs` cover:

**Extension method unit tests (tests 1–12):**
- `ConvertToBaseUnit` for all four units — confirms each unit's conversion formula produces the correct Feet value (e.g. 12 Inch → 1 Foot, 1 Yard → 3 Feet, 30.48 cm → 1 Foot)
- `ConvertFromBaseUnit` for all four units — confirms the inverse formula works in each direction (e.g. 1 Foot → 12 Inches, 3 Feet → 1 Yard, 1 Foot → 30.48 cm)

**`Quantity` behaviour tests (tests 13–19):**
- Equality — `1 Feet == 12 Inch`
- `ConvertTo` — `1 Feet → 12 Inch`
- `Add` (implicit target unit) — `1 Feet + 12 Inch = 2 Feet`
- `Add` with explicit target unit — `1 Feet + 12 Inch = 0.6667 Yard`
- Null unit rejected on construction
- NaN value rejected on construction

**Backward-compatibility tests (tests 20–23):**
- UC1 equality, UC5 conversion, UC6 addition, UC7 addition with target unit all pass unchanged

**Design and correctness tests (tests 24–26):**
- Round-trip conversion — `5 Feet → Inches → back to Feet` returns 5.0 within tolerance
- Unit immutability — enum value does not change after assignment
- Architecture scalability — enum constants are defined and accessible via `Enum.IsDefined`
