namespace QuantityMeasurementApp;

// Enum representing different volume units
public enum VolumeUnit
{
    Litre,
    Millilitre,
    Gallon
}

// Extension methods for VolumeUnit
public static class VolumeUnitExtension
{
    // Returns conversion factor of unit to base unit (Litre)
    public static double GetConversionFactor(this VolumeUnit unit)
    {
        switch (unit)
        {
            case VolumeUnit.Litre:
                return 1.0; // Base unit

            case VolumeUnit.Millilitre:
                return 0.001; // 1 millilitre = 0.001 litre

            case VolumeUnit.Gallon:
                return 3.78541; // 1 gallon = 3.78541 litres

            default:
                throw new ArgumentException("Invalid unit"); // Handles invalid unit
        }   
    }

    // Converts given value to base unit (Litre)
    public static double ConvertToBaseUnit(this VolumeUnit unit, double value)
    {
        return value * unit.GetConversionFactor(); // Multiply by conversion factor
    }

    // Converts base unit value (Litre) to the specified unit
    public static double ConvertFromBaseUnit(this VolumeUnit unit, double baseValue)
    {
        return baseValue / unit.GetConversionFactor(); // Divide by conversion factor
    }

    // Returns the name of the unit as string
    public static string GetUnitName(this VolumeUnit unit)
    {
        return unit.ToString(); // Converts enum to string
    }
}