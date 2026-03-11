using QuantityMeasurementApp;

namespace QuantityMeasurementApp;

// Enum representing different length units
public enum LengthUnit
{
    Feet,
    Inch,
    Yard,
    Centimeter
}

// Extension methods for LengthUnit
public static class LengthUnitExtension
{
    // Returns conversion factor of unit to base unit (Feet)
    public static double GetConversionFactor(this LengthUnit unit)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return 1.0; // Base unit

            case LengthUnit.Inch:
                return 1.0 / 12.0; // 1 inch = 1/12 feet

            case LengthUnit.Yard:
                return 3.0; // 1 yard = 3 feet

            case LengthUnit.Centimeter:
                return 1.0 / 30.48; // 1 cm = 1/30.48 feet

            default:
                throw new ArgumentException("Invalid unit"); // Handles invalid unit
        }
    }

    // Converts given value to base unit (Feet)
    public static double ConvertToBaseUnit(this LengthUnit unit, double value)
    {
        return value * unit.GetConversionFactor(); // Multiply by conversion factor
    }

    // Converts base unit value (Feet) to the given unit
    public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
    {
        return baseValue / unit.GetConversionFactor(); // Divide by conversion factor
    }

    // Returns the name of the unit as string
    public static string GetUnitName(this LengthUnit unit)
    {
        return unit.ToString(); // Converts enum to string
    }
}