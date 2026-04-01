# Quantity Measurement Application — UC14: Temperature Measurement

A single-project .NET application that extends the existing quantity measurement system to support temperature as a fourth measurement category. UC14 introduces `TemperatureUnit` (Celsius and Fahrenheit), integrates it into the existing generic `Quantity<U>` class, and enforces a strict rule: temperature supports only **equality comparison** and **unit conversion** — arithmetic operations (Add, Subtract, Divide) throw `NotSupportedException`.

---

## Overview

UC14 builds directly on the UC1–UC13 foundation (Length, Weight, Volume) by adding:

- A `TemperatureUnit` enum (`Celsius`, `Fahrenheit`) with non-linear offset-based conversions
- Extension methods on `TemperatureUnit` for `ConvertToBaseUnit`, `ConvertFromBaseUnit`, `SupportsArithmetic`, and `ValidateOperationSupport`
- A `ValidateOperationSupport` guard inside `Quantity<U>` that fires before any arithmetic call when the unit is a `TemperatureUnit`
- An `UnsupportedOperationException` custom exception class
- A demo `Main` that runs all four categories and deliberately triggers the temperature arithmetic errors to show the exception messages
- A 41-test MSTest suite covering equality, conversion, arithmetic rejection, cross-category incompatibility, edge cases, and backward compatibility

---

## Project Structure

```
UC14-TemperatureMeasurement/
│
├── QuantityMeasurementApp/
│   ├── Exception/
│   │   └── UnsupportedOperationException.cs     # Custom exception for unsupported operations
│   ├── Interface/
│   │   └── IMeasurable.cs                        # Unit contract with default arithmetic support
│   ├── Models/
│   │   └── Quantity.cs                           # Generic Quantity<U> — all operations live here
│   ├── Units/
│   │   ├── LengthUnit.cs                         # Feet, Inch, Yard, Centimeter (base: Feet)
│   │   ├── WeightUnit.cs                         # Kilogram, Gram, Pound (base: Kilogram)
│   │   ├── VolumeUnit.cs                         # Litre, Millilitre, Gallon (base: Litre)
│   │   └── TemperatureUnit.cs                    # Celsius, Fahrenheit (base: Celsius) — UC14 addition
│   └── QuantityMeasurementAppMain.cs             # Demo entry point — runs all four categories
│
└── QuantityMeasurementApp.Tests/
    └── TemperatureTest.cs                        # 41 MSTest tests
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

| Category | Units | Base Unit | Arithmetic |
|---|---|---|---|
| `LENGTH` | `Feet`, `Inch`, `Yard`, `Centimeter` | Feet | Supported |
| `WEIGHT` | `Kilogram`, `Gram`, `Pound` | Kilogram | Supported |
| `VOLUME` | `Litre`, `Millilitre`, `Gallon` | Litre | Supported |
| `TEMPERATURE` | `Celsius`, `Fahrenheit` | Celsius | **Not supported** |

---

## Temperature Conversion Formulas

| From | To | Formula |
|---|---|---|
| Celsius | Fahrenheit | `(C × 9/5) + 32` |
| Fahrenheit | Celsius | `(F − 32) × 5/9` |

All comparisons and conversions go through the base unit (Celsius), so `0°C == 32°F` and `-40°C == -40°F` are correctly detected as equal.

---

## Getting Started

### Prerequisites

- .NET SDK

### Build

```bash
git clone <your-repo-url>
cd UC14-TemperatureMeasurement
dotnet build
```

### Run the Demo

```bash
cd QuantityMeasurementApp
dotnet run
```

The demo runs operations across all four categories and prints results to the console. For temperature it deliberately tries Add, Subtract, and Divide and catches the exceptions to show the error messages:

```
===== TEMPERATURE OPERATIONS =====
Equality: Quantity(0, Celsius) == Quantity(32, Fahrenheit) → True
Conversion: Quantity(0, Celsius) → Quantity(32, Fahrenheit)
Addition Error: Temperature does not support ADD
Subtraction Error: Temperature does not support SUBTRACT
Division Error: Temperature does not support DIVIDE
```

---

## Temperature Arithmetic Restriction

Arithmetic is blocked at two levels:

`TemperatureUnitExtension.ValidateOperationSupport` always throws:

```csharp
public static void ValidateOperationSupport(this TemperatureUnit unit, string operation)
{
    throw new NotSupportedException($"Temperature does not support {operation}");
}
```

`Quantity<U>.Add / Subtract / Divide` checks the unit type before doing anything else:

```csharp
if (Unit is TemperatureUnit tu)
    tu.ValidateOperationSupport("ADD");
```

This means the exception is thrown regardless of the values passed in — there is no path through which temperature arithmetic can succeed.

---

## Running the Tests

```bash
cd QuantityMeasurementApp.Tests
dotnet test
```

The 41 tests in `TemperatureTest.cs` cover:

- **Equality** — same unit same value, Celsius vs Fahrenheit at 0°/32°, 100°/212°, −40°/−40° (the one value where both scales meet), symmetric and reflexive properties, different values that should not be equal
- **Conversion** — Celsius→Fahrenheit and back for positive, negative, zero, and large values; round-trip precision; same-unit conversion
- **Arithmetic rejection** — Add, Subtract, and Divide each throw an exception; the exception message contains "Temperature"; `NotSupportedException` is the specific type thrown
- **Cross-category incompatibility** — temperature vs Length, Weight, and Volume all return false from `Equals`
- **Edge cases** — very small values survive a round-trip, null unit rejected on construction, null operand returns false from `Equals`
- **Backward compatibility** — `1 Feet == 12 Inch` still passes, `length.Add(other)` still works confirming UC1–UC13 is unaffected
- **Interface and design** — `IMeasurable` default method inheritance, `SupportsArithmetic` returns false for temperature, `GetUnitName` returns `"Celsius"`, non-linear conversion confirmed (temperature result ≠ value × 2)

---

## How `Quantity<U>` Handles Temperature

`Quantity<U>` uses a `switch` on the concrete enum type inside `ToBaseUnit()` and `ConvertFromBase()` to dispatch to the right extension method. Temperature uses offset-based formulas rather than a simple multiplication factor, so it gets its own conversion path:

```
ToBaseUnit()          → TemperatureUnitExtension.ConvertToBaseUnit   (→ Celsius)
ConvertFromBase()     → TemperatureUnitExtension.ConvertFromBaseUnit  (← Celsius)
Add / Subtract / Divide → ValidateOperationSupport throws immediately
```

Length, Weight, and Volume all use `value × factor` / `value ÷ factor`, so they share the same arithmetic path. Temperature is the only unit type that deviates.
