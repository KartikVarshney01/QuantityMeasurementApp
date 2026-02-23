namespace QuantityMeasurementApp;
public class Quantity
{
    public readonly double Value;
    public readonly LengthUnit Unit;

    public Quantity(LengthUnit unit, double value)
    {
        if (!Enum.IsDefined(typeof(LengthUnit), unit))
        {
            throw new ArgumentException("Invalid unit");
        }

        Value = value;
        Unit = unit;
    }

    // Convert everything to base unit (Feet)
    private double ToBaseUnit()
    {
        return Unit.ToFeet(Value);
    }

    public override bool Equals(object obj)
    {
        // Reflexive
        if (this == obj)
            return true;

        // Null + Type check
        if (obj == null || GetType() != obj.GetType()) return false;

        Quantity other = (Quantity)obj;

        // Compare after conversion
        return Math.Abs(this.ToBaseUnit() - other.ToBaseUnit()) == 0;
    }

    public override int GetHashCode()
    {
        return ToBaseUnit().GetHashCode();
    }
}