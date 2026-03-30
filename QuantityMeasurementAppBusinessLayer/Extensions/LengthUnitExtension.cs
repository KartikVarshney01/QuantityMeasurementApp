using QuantityMeasurementAppModelLayer.Enums;

namespace QuantityMeasurementAppBusinessLayer.Extensions;

public static class LengthUnitExtension
{
    public static double GetConversionFactor(this LengthUnit unit)
        => unit switch
        {
            LengthUnit.Feet       => 1.0,
            LengthUnit.Inch       => 1.0 / 12.0,
            LengthUnit.Yard       => 3.0,
            LengthUnit.Centimeter => 1.0 / 30.48,
            _                     => throw new ArgumentException("Invalid LengthUnit")
        };

    public static double ConvertToBaseUnit(this LengthUnit unit, double value)
        => value * unit.GetConversionFactor();

    public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
        => baseValue / unit.GetConversionFactor();

    public static string GetUnitName(this LengthUnit unit)
        => unit.ToString();
}
