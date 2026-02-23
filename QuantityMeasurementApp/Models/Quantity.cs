namespace QuantityMeasurementApp;
public class Quantity
{
    public readonly double Value;
    public readonly LengthUnit? Unit;

    public Quantity(LengthUnit? unit, double value)
    {
        // if (unit == null)
        //     throw new ArgumentException("Unit cannot be null");

        if (Double.IsInfinity(value) || Double.IsNaN(value))
            throw new ArgumentException("Invalid Value Parameter");

        if (unit != null && !Enum.IsDefined(typeof(LengthUnit), unit))
        {
            throw new ArgumentException("Invalid unit");
        }

        Value = value;
        Unit = unit;
    }

    // Convert everything to base unit (Feet)
    private double ToBaseUnit()
    {
        if (Unit == null)
            throw new InvalidOperationException("Unit is null");

        return Unit.Value.ToFeet(Value);
    }

    public override bool Equals(object? obj)
    {
        // Reflexive
        if (this == obj)
            return true;

        // Null + Type check
        if (obj == null || GetType() != obj.GetType()) return false;

        Quantity other = (Quantity)obj;

        var unit1 = this.Unit;
        var unit2 = other.Unit;

        if (unit1 == null || unit2 == null)
            return false;

        double base1 = unit1.Value.ToFeet(this.Value);
        double base2 = unit2.Value.ToFeet(other.Value);

        return Math.Abs(base1 - base2) < 0.0001;
    }

    public override int GetHashCode()
    {
        if (Unit == null)
            return 0;

        return Unit.Value.ToFeet(Value).GetHashCode();
    }
}