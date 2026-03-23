using QuantityMeasurementAppModelLayer.Enum;

namespace QuantityMeasurementAppBusinessLayer.Extensions;

public static class WeightUnitExtension
{
    public static double GetConversionFactor(this WeightUnit unit)
    {
        return unit switch
        {
            WeightUnit.Kilogram => 1.0,
            WeightUnit.Gram => 0.001,
            WeightUnit.Pound => 0.453592,
            _ => throw new ArgumentException("Invalid WeightUnit")
        };
    }

    public static double ConvertToBaseUnit(this WeightUnit unit, double value)
        => value * unit.GetConversionFactor();

    public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
        => baseValue / unit.GetConversionFactor();

    public static string GetUnitName(this WeightUnit unit)
        => unit.ToString();
}