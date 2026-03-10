namespace QuantityMeasurementApp;

public enum WeightUnit
{
    Kilogram,
    Gram,
    Pound
}

public static class WeightUnitExtensions
{
    public static double ConvertToBaseUnit(this WeightUnit unit, double value)
    {
        switch (unit)
        {
            case WeightUnit.Kilogram:
                return value;

            case WeightUnit.Gram:
                return value * 0.001;

            case WeightUnit.Pound:
                return value * 0.453592;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }

    public static double ConvertFromBaseUnit(this WeightUnit unit, double value)
    {
        switch (unit)
        {
            case WeightUnit.Kilogram:
                return value;

            case WeightUnit.Gram:
                return value / 0.001;

            case WeightUnit.Pound:
                return value / 0.453592;

            default:
                throw new ArgumentException("Invalid unit");
        }
    }
}