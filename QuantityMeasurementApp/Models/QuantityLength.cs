namespace QuantityMeasurementApp;

public class QuantityLength
{
    public readonly double Value;
    public readonly LengthUnit? Unit;

    public QuantityLength(LengthUnit? unit, double value)
    {
        if (Double.IsInfinity(value) || Double.IsNaN(value))
            throw new ArgumentException("Invalid value");

        if (unit == null)
            throw new ArgumentException("Unit cannot be null");

        Value = value;
        Unit = unit;
    }

    private double ToBaseUnit()
    {
        return Unit!.Value.ConvertToBaseUnit(Value);
    }

    public double ConvertTo(LengthUnit targetUnit)
    {
        double baseValue = ToBaseUnit();
        return targetUnit.ConvertFromBaseUnit(baseValue);
    }

    // UC6 addition (result in first operand unit)
    public static QuantityLength Add(QuantityLength q1, QuantityLength q2)
    {
        return Add(q1, q2, q1.Unit!.Value);
    }

    // UC7 addition (explicit target unit)
    public static QuantityLength Add(QuantityLength q1, QuantityLength q2, LengthUnit targetUnit)
    {
        if (q1 == null || q2 == null)
            throw new ArgumentException("Operands cannot be null");

        if (q1.Unit == null || q2.Unit == null)
            throw new ArgumentException("Unit cannot be null");

        double base1 = q1.Unit.Value.ConvertToBaseUnit(q1.Value);
        double base2 = q2.Unit.Value.ConvertToBaseUnit(q2.Value);

        double sum = base1 + base2;

        double result = targetUnit.ConvertFromBaseUnit(sum);

        return new QuantityLength(targetUnit, result);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        QuantityLength other = (QuantityLength)obj;

        double base1 = Unit!.Value.ConvertToBaseUnit(Value);
        double base2 = other.Unit!.Value.ConvertToBaseUnit(other.Value);

        return Math.Abs(base1 - base2) < 0.0001;
    }

    public override int GetHashCode()
    {
        return Unit!.Value.ConvertToBaseUnit(Value).GetHashCode();
    }
}