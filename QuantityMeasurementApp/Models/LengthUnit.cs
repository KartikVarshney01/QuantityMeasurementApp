namespace QuantityMeasurementApp;
public enum LengthUnit
{
    Feet,
    Inch
}

public static class LengthUnitExtension
{
    public static double ToFeet(this LengthUnit unit, double value)
    {
        switch (unit)
        {
            case LengthUnit.Feet:
                return value;

            case LengthUnit.Inch:
                return value / 12.0;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }
}