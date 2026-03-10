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
    public static double ConvertToBaseUnit(this LengthUnit unit, double value)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return value;

            case LengthUnit.Inch:
                return value / 12.0;

            case LengthUnit.Yard:
                return value * 3.0;

            case LengthUnit.Centimeter:
                return (value * 0.393701) / 12.0;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }

    public static double ConvertFromBaseUnit(this LengthUnit unit, double baseValue)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return baseValue;

            case LengthUnit.Inch:
                return baseValue * 12.0;

            case LengthUnit.Yard:
                return baseValue / 3.0;

            case LengthUnit.Centimeter:
                return baseValue * 12.0 / 0.393701;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }
}