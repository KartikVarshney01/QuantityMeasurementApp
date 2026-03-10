using QuantityMeasurementApp;

namespace QuantityMeasurementApp;

public enum LengthUnit
{
    Feet,
    Inch,
    Yard,
    Centimeter
}

public static class LengthUnitExtension
{
    public static double GetConversionFactor(this LengthUnit unit)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return 1.0;

            case LengthUnit.Inch:
                return 1.0 / 12.0;

            case LengthUnit.Yard:
                return 3.0;

            case LengthUnit.Centimeter:
                return 1.0 / 30.48;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }

    public static double ConvertToBaseUnit(this LengthUnit unit, double value)
    {
        return value * unit.GetConversionFactor();
    }

    public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
    {
        return baseValue / unit.GetConversionFactor();
    }

    public static string GetUnitName(this LengthUnit unit)
    {
        return unit.ToString();
    }
}