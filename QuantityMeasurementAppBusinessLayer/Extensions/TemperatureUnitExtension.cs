using QuantityMeasurementAppModelLayer.Enums;

namespace QuantityMeasurementAppBusinessLayer.Extensions;

public static class TemperatureUnitExtension
{
    public static double ConvertToBaseUnit(this TemperatureUnit unit, double value)
        => unit switch
        {
            TemperatureUnit.Celsius    => value,
            TemperatureUnit.Fahrenheit => (value - 32) * 5.0 / 9.0,
            TemperatureUnit.Kelvin     => value - 273.15,
            _                          => throw new ArgumentException("Invalid TemperatureUnit")
        };

    public static double ConvertFromBaseUnit(this TemperatureUnit unit, double baseValue)
        => unit switch
        {
            TemperatureUnit.Celsius    => baseValue,
            TemperatureUnit.Fahrenheit => (baseValue * 9.0 / 5.0) + 32,
            TemperatureUnit.Kelvin     => baseValue + 273.15,
            _                          => throw new ArgumentException("Invalid TemperatureUnit")
        };

    public static string GetUnitName(this TemperatureUnit unit)
        => unit.ToString();

    public static void ValidateOperationSupport(this TemperatureUnit unit, string operation)
        => throw new NotSupportedException($"Temperature does not support {operation}");
}
