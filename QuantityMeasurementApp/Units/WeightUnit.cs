using QuantityMeasurementApp;

namespace QuantityMeasurementApp;

public enum WeightUnit 
{
    Kilogram,
    Gram,
    Pound
}

public static class WeightUnitExtension
{
    public static double GetConversionFactor(this WeightUnit unit)
    {
        switch (unit)
        {
            case WeightUnit.Kilogram:
                return 1.0;

            case WeightUnit.Gram:
                return 0.001;

            case WeightUnit.Pound:
                return 0.453592;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }

    public static double ConvertToBaseUnit(this WeightUnit unit, double value)
    {
        return value * unit.GetConversionFactor();
    }

    public static double ConvertFromBaseUnit(this WeightUnit unit, double baseValue)
    {
        return baseValue / unit.GetConversionFactor();
    }

    public static string GetUnitName(this WeightUnit unit)
    {
        return unit.ToString();
    }
}