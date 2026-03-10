namespace QuantityMeasurementApp;

public class Quantity<U> where U : Enum
{
    private const double EPSILON = 0.0001;

    public readonly double Value;
    public readonly U Unit;

    public Quantity(double value, U unit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Invalid value");

        if (unit == null)
            throw new ArgumentException("Unit cannot be null");

        Value = value;
        Unit = unit;
    }

    private double ToBaseUnit()
    {
        if (Unit is LengthUnit lu)
            return LengthUnitExtension.ConvertToBaseUnit(lu, Value);

        if (Unit is WeightUnit wu)
            return WeightUnitExtension.ConvertToBaseUnit(wu, Value);

        if (Unit is VolumeUnit vu)
            return VolumeUnitExtension.ConvertToBaseUnit(vu, Value);

        throw new ArgumentException("Unsupported unit type");
    }

    public Quantity<U> ConvertTo(U targetUnit)
    {
        double baseValue = ToBaseUnit();
        double result;

        if (targetUnit is LengthUnit lu)
            result = LengthUnitExtension.ConvertFromBaseUnit(lu, baseValue);
        else if (targetUnit is WeightUnit wu)
            result = WeightUnitExtension.ConvertFromBaseUnit(wu, baseValue);
        else if (targetUnit is VolumeUnit vu)
            result = VolumeUnitExtension.ConvertFromBaseUnit(vu, baseValue);
        else
            throw new ArgumentException("Unsupported unit type");

        return new Quantity<U>(Math.Round(result, 4), targetUnit);
    }

    public Quantity<U> Add(Quantity<U> other)
    {
        return Add(other, Unit);
    }

    public Quantity<U> Add(Quantity<U> other, U targetUnit)
    {
        if (other == null)
            throw new ArgumentException("Operand cannot be null");

        double base1 = ToBaseUnit();
        double base2 = other.ToBaseUnit();
        double sum = base1 + base2;

        double result;

        if (targetUnit is LengthUnit lu)
            result = LengthUnitExtension.ConvertFromBaseUnit(lu, sum);
        else if (targetUnit is WeightUnit wu)
            result = WeightUnitExtension.ConvertFromBaseUnit(wu, sum);
        else if (targetUnit is VolumeUnit vu)
            result = VolumeUnitExtension.ConvertFromBaseUnit(vu, sum);
        else
            throw new ArgumentException("Unsupported unit type");

        return new Quantity<U>(Math.Round(result, 4), targetUnit);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        Quantity<U> other = (Quantity<U>)obj;

        if (Unit.GetType() != other.Unit.GetType())
            return false;

        double base1 = ToBaseUnit();
        double base2 = other.ToBaseUnit();

        return Math.Abs(base1 - base2) < EPSILON;
    }

    public override int GetHashCode()
    {
        return ToBaseUnit().GetHashCode();
    }

    public override string ToString()
    {
        return $"Quantity({Value}, {Unit})";
    }
}