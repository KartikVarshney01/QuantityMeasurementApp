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
    public static double ToFeet(this LengthUnit unit, double value)
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
                return (value * 0.393701) / 12.0; // cm -> inch -> feet

            default:
                throw new ArgumentException("Invalid unit");
        }
    }
}