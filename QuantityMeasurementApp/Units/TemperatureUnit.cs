namespace QuantityMeasurementApp;

// Enum representing temperature units
public enum TemperatureUnit
{
    Celsius,
    Fahrenheit
}

// Extension methods for TemperatureUnit
public static class TemperatureUnitExtension
{
    // Returns conversion factor (not used for temperature, so kept as 1)
    public static double GetConversionFactor(this TemperatureUnit unit)
    {
        return 1.0;
    }

    // Converts given temperature to base unit (Celsius)
    public static double ConvertToBaseUnit(this TemperatureUnit unit, double value)
    {
        switch (unit)
        {
            case TemperatureUnit.Celsius:
                return value; // Celsius is the base unit

            case TemperatureUnit.Fahrenheit:
                return (value - 32) * 5.0 / 9.0; // Convert Fahrenheit to Celsius

            default:
                throw new ArgumentException("Invalid unit"); // Handle invalid unit
        }
    }

    // Converts base unit (Celsius) to target temperature unit
    public static double ConvertFromBaseUnit(this TemperatureUnit unit, double baseValue)
    {
        switch (unit)
        {
            case TemperatureUnit.Celsius:
                return baseValue; // Base unit remains unchanged

            case TemperatureUnit.Fahrenheit:
                return (baseValue * 9.0 / 5.0) + 32; // Convert Celsius to Fahrenheit

            default:
                throw new ArgumentException("Invalid unit"); // Handle invalid unit
        }
    }

    // Returns the unit name as a string
    public static string GetUnitName(this TemperatureUnit unit)
    {
        return unit.ToString();
    }

    // UC14: Temperature does NOT support arithmetic operations

    // Indicates arithmetic operations are not supported
    public static bool SupportsArithmetic(this TemperatureUnit unit)
    {
        return false;
    }

    // Throws exception if arithmetic operation is attempted
    public static void ValidateOperationSupport(this TemperatureUnit unit, string operation)
    {
        throw new NotSupportedException($"Temperature does not support {operation}");
    }
}