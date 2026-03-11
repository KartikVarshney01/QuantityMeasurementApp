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

    private enum ArithmeticOperation
    {
        ADD,
        SUBTRACT,
        DIVIDE
    }

    private double ToBaseUnit()
    {
        if (Unit is LengthUnit lu)
            return lu.ConvertToBaseUnit(Value);

        if (Unit is WeightUnit wu)
            return wu.ConvertToBaseUnit(Value);

        if (Unit is VolumeUnit vu)
            return vu.ConvertToBaseUnit(Value);

        throw new ArgumentException("Unsupported unit type");
    }

    private void ValidateArithmeticOperands(Quantity<U> other, U targetUnit, bool targetUnitRequired)
    {
        if (other == null)
            throw new ArgumentException("Operand cannot be null");

        if (double.IsNaN(other.Value) || double.IsInfinity(other.Value))
            throw new ArgumentException("Invalid operand value");

        if (Unit.GetType() != other.Unit.GetType())
            throw new ArgumentException("Cross-category arithmetic not allowed");

        if (targetUnitRequired && targetUnit == null)
            throw new ArgumentException("Target unit cannot be null");
    }

    private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
    {
        double base1 = ToBaseUnit();
        double base2 = other.ToBaseUnit();

        return operation switch
        {
            ArithmeticOperation.ADD => base1 + base2,
            ArithmeticOperation.SUBTRACT => base1 - base2,
            ArithmeticOperation.DIVIDE => base2 == 0
                ? throw new ArithmeticException("Division by zero")
                : base1 / base2,
            _ => throw new ArgumentException("Invalid operation")
        };
    }

    public Quantity<U> Add(Quantity<U> other)
    {
        return Add(other, Unit);
    }

    public Quantity<U> Add(Quantity<U> other, U targetUnit)
    {
        ValidateArithmeticOperands(other, targetUnit, true);

        double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);

        double result = ConvertFromBase(baseResult, targetUnit);

        return new Quantity<U>(Math.Round(result, 2), targetUnit);
    }

    public Quantity<U> Subtract(Quantity<U> other)
    {
        return Subtract(other, Unit);
    }

    public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
    {
        ValidateArithmeticOperands(other, targetUnit, true);

        double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);

        double result = ConvertFromBase(baseResult, targetUnit);

        return new Quantity<U>(Math.Round(result, 2), targetUnit);
    }

    public double Divide(Quantity<U> other)
    {
        ValidateArithmeticOperands(other, Unit, false);

        return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
    }

    private double ConvertFromBase(double baseValue, U targetUnit)
    {
        if (targetUnit is LengthUnit lu)
            return lu.ConvertFromBaseUnit(baseValue);

        if (targetUnit is WeightUnit wu)
            return wu.ConvertFromBaseUnit(baseValue);

        if (targetUnit is VolumeUnit vu)
            return vu.ConvertFromBaseUnit(baseValue);

        throw new ArgumentException("Unsupported unit type");
    }

    public Quantity<U> ConvertTo(U targetUnit)
    {
        double baseValue = ToBaseUnit();
        double result;

        if (targetUnit is LengthUnit lu)
            result = lu.ConvertFromBaseUnit(baseValue);

        else if (targetUnit is WeightUnit wu)
            result = wu.ConvertFromBaseUnit(baseValue);

        else if (targetUnit is VolumeUnit vu)
            result = vu.ConvertFromBaseUnit(baseValue);

        else
            throw new ArgumentException("Unsupported unit type");

        return new Quantity<U>(Math.Round(result, 4), targetUnit);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        Quantity<U> other = (Quantity<U>)obj;

        return Math.Abs(ToBaseUnit() - other.ToBaseUnit()) < EPSILON;
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