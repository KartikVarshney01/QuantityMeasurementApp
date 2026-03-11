using QuantityMeasurementApp;

namespace QuantityMeasurementApp;

// Enum representing different weight units
public enum WeightUnit 
{
    Kilogram,
    Gram,
    Pound
}

// Extension methods for WeightUnit
public static class WeightUnitExtension
{
    // Returns conversion factor of unit to base unit (Kilogram)
    public static double GetConversionFactor(this WeightUnit unit)
    {
        switch (unit)
        {
            case WeightUnit.Kilogram:
                return 1.0; // Base unit

            case WeightUnit.Gram:
                return 0.001; // 1 gram = 0.001 kilogram

            case WeightUnit.Pound:
                return 0.453592; // 1 pound = 0.453592 kilogram

            default:
                throw new ArgumentException("Invalid unit"); // Handles invalid unit
        }
    }

    // Converts given weight value to base unit (Kilogram)
    public static double ConvertToBaseUnit(this WeightUnit unit, double value)
    {
        return value * unit.GetConversionFactor(); // Multiply by conversion factor
    }

    // Converts base unit value (Kilogram) to the specified unit
    public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
    {
        return baseValue / unit.GetConversionFactor(); // Divide by conversion factor
    }

    // Returns the unit name as a string
    public static string GetUnitName(this WeightUnit unit)
    {
        return unit.ToString(); // Converts enum to string
    }
}