using QuantityMeasurementAppModelLayer.Enum;
using QuantityMeasurementAppBusinessLayer.Extensions;

namespace QuantityMeasurementAppBusinessLayer;

// Generic Quantity class where U must be an Enum
public class Quantity<U> where U : Enum
{
    private const double EPSILON = 0.0001; // Tolerance for floating point comparison

    public readonly double Value; // Stores quantity value
    public readonly U Unit; // Stores unit type

    // Constructor to initialize value and unit
    public Quantity(double value, U unit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Invalid value"); // Reject invalid numbers

        if (unit == null)
            throw new ArgumentException("Unit cannot be null"); // Ensure unit is provided

        Value = value;
        Unit = unit;
    }

    // Enum representing supported arithmetic operations
    private enum ArithmeticOperation
    {
        ADD,
        SUBTRACT,
        DIVIDE
    }

    // Converts current quantity value to base unit
    private double ToBaseUnit()
    {
        switch (Unit)
        {
            case LengthUnit lu:
                return lu.ConvertToBaseUnit(Value); // Convert length to base

            case WeightUnit wu:
                return wu.ConvertToBaseUnit(Value); // Convert weight to base

            case VolumeUnit vu:
                return vu.ConvertToBaseUnit(Value); // Convert volume to base

            case TemperatureUnit tu:
                return tu.ConvertToBaseUnit(Value); // Convert temperature to base

            default:
                throw new ArgumentException("Unsupported unit type");
        }
    }

    // Converts base value to the specified target unit
    private double ConvertFromBase(double baseValue, U targetUnit)
    {
        switch (targetUnit)
        {
            case LengthUnit lu:
                return lu.ConvertFromBaseUnit(baseValue); // Convert base to length

            case WeightUnit wu:
                return wu.ConvertFromBaseUnit(baseValue); // Convert base to weight

            case VolumeUnit vu:
                return vu.ConvertFromBaseUnit(baseValue); // Convert base to volume

            case TemperatureUnit tu:
                return tu.ConvertFromBaseUnit(baseValue); // Convert base to temperature

            default:
                throw new ArgumentException("Unsupported unit type");
        }
    }

    // Validates operands before performing arithmetic
    private void ValidateArithmeticOperands(Quantity<U> other, U targetUnit, bool targetRequired)
    {
        if (other == null)
            throw new ArgumentException("Operand cannot be null"); // Ensure second operand exists

        if (Unit.GetType() != other.Unit.GetType())
            throw new ArgumentException("Cross-category arithmetic not allowed"); // Prevent mixing units

        if (targetRequired && targetUnit == null)
            throw new ArgumentException("Target unit cannot be null"); // Ensure target unit if required
    }

    // Performs arithmetic operation after converting both quantities to base unit
    private double PerformBaseArithmetic(Quantity<U> other, ArithmeticOperation operation)
    {
        double base1 = ToBaseUnit(); // Convert first value to base
        double base2 = other.ToBaseUnit(); // Convert second value to base

        switch (operation)
        {
            case ArithmeticOperation.ADD:
                return base1 + base2; // Add base values

            case ArithmeticOperation.SUBTRACT:
                return base1 - base2; // Subtract base values

            case ArithmeticOperation.DIVIDE:
                if (Math.Abs(base2) < EPSILON)
                    throw new ArithmeticException("Division by zero"); // Prevent division by zero
                return base1 / base2; // Divide base values

            default:
                throw new ArgumentException("Invalid operation");
        }
    }

    // Adds two quantities using the current unit
    public Quantity<U> Add(Quantity<U> other)
    {
        return Add(other, Unit);
    }

    // Adds two quantities and converts result to target unit
    public Quantity<U> Add(Quantity<U> other, U targetUnit)
    {
        if (Unit is TemperatureUnit tu)
            tu.ValidateOperationSupport("ADD"); // Validate temperature addition

        ValidateArithmeticOperands(other, targetUnit, true);

        double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.ADD);

        double result = ConvertFromBase(baseResult, targetUnit);

        return new Quantity<U>(Math.Round(result, 2), targetUnit); // Return rounded result
    }

    // Subtracts two quantities using the current unit
    public Quantity<U> Subtract(Quantity<U> other)
    {
        return Subtract(other, Unit);
    }

    // Subtracts two quantities and converts result to target unit
    public Quantity<U> Subtract(Quantity<U> other, U targetUnit)
    {
        if (Unit is TemperatureUnit tu)
            tu.ValidateOperationSupport("SUBTRACT"); // Validate temperature subtraction

        ValidateArithmeticOperands(other, targetUnit, true);

        double baseResult = PerformBaseArithmetic(other, ArithmeticOperation.SUBTRACT);

        double result = ConvertFromBase(baseResult, targetUnit);

        return new Quantity<U>(Math.Round(result, 2), targetUnit); // Return rounded result
    }

    // Divides two quantities and returns ratio
    public double Divide(Quantity<U> other)
    {
        if (Unit is TemperatureUnit tu)
            tu.ValidateOperationSupport("DIVIDE"); // Validate temperature division

        ValidateArithmeticOperands(other, Unit, false);

        return PerformBaseArithmetic(other, ArithmeticOperation.DIVIDE);
    }

    // Converts quantity to another unit
    public Quantity<U> ConvertTo(U targetUnit)
    {
        double baseValue = ToBaseUnit(); // Convert to base

        double result = ConvertFromBase(baseValue, targetUnit); // Convert base to target

        return new Quantity<U>(Math.Round(result, 4), targetUnit); // Return converted quantity
    }

    // Checks equality by comparing base unit values
    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        Quantity<U> other = (Quantity<U>)obj;

        if (Unit.GetType() != other.Unit.GetType())
            return false;

        return Math.Abs(ToBaseUnit() - other.ToBaseUnit()) < EPSILON; // Compare within tolerance
    }

    // Generates hash code based on base unit value
    public override int GetHashCode()
    {
        return ToBaseUnit().GetHashCode();
    }

    // Returns readable string representation
    public override string ToString()
    {
        return $"Quantity({Value}, {Unit})";
    }
}