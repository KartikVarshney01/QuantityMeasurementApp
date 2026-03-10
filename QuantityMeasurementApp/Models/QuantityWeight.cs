namespace QuantityMeasurementApp;

public class QuantityWeight
{
    public readonly double Value;
    public readonly WeightUnit? Unit;

    public QuantityWeight(WeightUnit? unit, double value)
    {
        if (unit == null)
            throw new ArgumentException("Unit cannot be null");

        if (Double.IsNaN(value) || Double.IsInfinity(value))
            throw new ArgumentException("Invalid value");

        Unit = unit;
        Value = value;
    }

    private double ToBaseUnit()
    {
        return Unit!.Value.ConvertToBaseUnit(Value);
    }

    public QuantityWeight ConvertTo(WeightUnit targetUnit)
    {
        double baseValue = ToBaseUnit();
        double result = targetUnit.ConvertFromBaseUnit(baseValue);

        return new QuantityWeight(targetUnit, result);
    }

    // UC6 equivalent
    public static QuantityWeight Add(QuantityWeight w1, QuantityWeight w2)
    {
        return Add(w1, w2, w1.Unit!.Value);
    }

    // UC7 equivalent
    public static QuantityWeight Add(QuantityWeight w1, QuantityWeight w2, WeightUnit targetUnit)
    {
        if (w1 == null || w2 == null)
            throw new ArgumentException("Operands cannot be null");

        double base1 = w1.Unit!.Value.ConvertToBaseUnit(w1.Value);
        double base2 = w2.Unit!.Value.ConvertToBaseUnit(w2.Value);

        double sum = base1 + base2;

        double result = targetUnit.ConvertFromBaseUnit(sum);

        return new QuantityWeight(targetUnit, result);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || GetType() != obj.GetType())
            return false;

        QuantityWeight other = (QuantityWeight)obj;

        double base1 = Unit!.Value.ConvertToBaseUnit(Value);
        double base2 = other.Unit!.Value.ConvertToBaseUnit(other.Value);

        return Math.Abs(base1 - base2) < 0.0001;
    }

    public override int GetHashCode()
    {
        return Unit!.Value.ConvertToBaseUnit(Value).GetHashCode();
    }

    public override string ToString()
    {
        return $"Quantity({Value}, {Unit})";
    }
}