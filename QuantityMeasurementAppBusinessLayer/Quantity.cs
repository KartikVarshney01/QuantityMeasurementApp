using QuantityMeasurementAppModelLayer.Enums;
using QuantityMeasurementAppBusinessLayer.Extensions;

namespace QuantityMeasurementAppBusinessLayer;

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
        Unit  = unit;
    }

    private enum ArithmeticOperation { ADD, SUBTRACT, DIVIDE }

    // Converts current quantity value to base unit
    private double ToBaseUnit()
    {
        switch (Unit)
        {
            case LengthUnit lu:      return lu.ConvertToBaseUnit(Value);
            case WeightUnit wu:      return wu.ConvertToBaseUnit(Value);
            case VolumeUnit vu:      return vu.ConvertToBaseUnit(Value);
            case TemperatureUnit tu: return tu.ConvertToBaseUnit(Value);
            default: throw new ArgumentException("Unsupported unit type");
        }
    }

    // Converts base value to the specified target unit
    private double ConvertFromBase(double baseValue, U targetUnit)
    {
        switch (targetUnit)
        {
            case LengthUnit lu:      return lu.ConvertFromBaseUnit(baseValue);
            case WeightUnit wu:      return wu.ConvertFromBaseUnit(baseValue);
            case VolumeUnit vu:      return vu.ConvertFromBaseUnit(baseValue);
            case TemperatureUnit tu: return tu.ConvertFromBaseUnit(baseValue);
            default: throw new ArgumentException("Unsupported unit type");
        }
    }

    private void ValidateArithmeticOperands(Quantity<U> other, U targetUnit, bool targetRequired)
    {
        if (other == null)
            throw new ArgumentException("Operand cannot be null");

        if (Unit.GetType() != other.Unit.GetType())
            throw new ArgumentException("Cross-category arithmetic not allowed");

        if (targetRequired && targetUnit == null)
            throw new ArgumentException("Target unit cannot be null");
    }

    private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
    {
        double base1 = ToBaseUnit();
        double base2 = other.ToBaseUnit();

        switch (operation)
        {
            case ArithmeticOperation.ADD:      return base1 + base2;
            case ArithmeticOperation.SUBTRACT: return base1 - base2;
            case ArithmeticOperation.DIVIDE:
                if (Math.Abs(base2) < EPSILON)
                    throw new ArithmeticException("Division by zero");
                return base1 / base2;
            default: throw new ArgumentException("Invalid operation");
        }
    }

    public Quantity<U> Add(Quantity<U> other) => Add(other, Unit);

    public Quantity<U> Add(Quantity<U> other, U targetUnit)
    {
        if (Unit is TemperatureUnit tu)
            tu.ValidateOperationSupport("ADD");

        ValidateArithmeticOperands(other, targetUnit, true);
        double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);
        double result     = ConvertFromBase(baseResult, targetUnit);
        return new Quantity<U>(Math.Round(result, 2), targetUnit);
    }

    public Quantity<U> Subtract(Quantity<U> other) => Subtract(other, Unit);

    public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
    {
        if (Unit is TemperatureUnit tu)
            tu.ValidateOperationSupport("SUBTRACT");

        ValidateArithmeticOperands(other, targetUnit, true);
        double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);
        double result     = ConvertFromBase(baseResult, targetUnit);
        return new Quantity<U>(Math.Round(result, 2), targetUnit);
    }

    public double Divide(Quantity<U> other)
    {
        if (Unit is TemperatureUnit tu)
            tu.ValidateOperationSupport("DIVIDE");

        ValidateArithmeticOperands(other, Unit, false);
        return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
    }

    public Quantity<U> ConvertTo(U targetUnit)
    {
        double baseValue = ToBaseUnit();
        double result    = ConvertFromBase(baseValue, targetUnit);
        return new Quantity<U>(Math.Round(result, 4), targetUnit);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType()) return false;

        Quantity<U> other = (Quantity<U>)obj;

        if (Unit.GetType() != other.Unit.GetType()) return false;

        return Math.Abs(ToBaseUnit() - other.ToBaseUnit()) < EPSILON;
    }

    public override int GetHashCode() => ToBaseUnit().GetHashCode();

    public override string ToString() => $"Quantity({Value}, {Unit})";
}
