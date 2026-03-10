using System.Net;

namespace QuantityMeasurementApp;
public class Quantity
{
    public readonly double Value;
    public readonly LengthUnit? Unit;

    public Quantity(LengthUnit? unit, double value)
    {
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

    public static double Convert(double value, LengthUnit? source, LengthUnit? target)
    {
        if(source == null || target == null) throw new ArgumentException("Unit Can not be null");
        if(Double.IsNaN(value) || Double.IsInfinity(value)) throw new ArgumentException("Invalid Value");
        if(!Enum.IsDefined(typeof(LengthUnit),source) || !Enum.IsDefined(typeof(LengthUnit), target))
        {
            throw new ArgumentException("Invalid Unit");
        }

        // Convert source → feet
        double valueInFeet = source.Value.ToFeet(value);

        // Convert source → feet
        double result = target.Value switch
        {
            LengthUnit.Feet => valueInFeet,
            LengthUnit.Inch => valueInFeet * 12.0,
            LengthUnit.Yard => valueInFeet / 3.0,
            LengthUnit.Centimeter => valueInFeet * 12.0 / 0.393701,
            _ => throw new ArgumentException("Invalid target unit")
        };
        return result;
    }

    public static Quantity Add(Quantity q1, Quantity q2)
    {
        if(q1==null || q2==null) throw new ArgumentException("Operands can not be null");
        if(q1.Unit==null || q2.Unit==null) throw new ArgumentException("Units can not be null");
        // Convert both quantities to base unit (Feet)
        double base1 = q1.ToBaseUnit();
        double base2 = q2.ToBaseUnit();

        // Add values in base unit
        double sumInFeet = base1 + base2;

        // Convert result to unit of first operand
        double result = Convert(sumInFeet, LengthUnit.Feet, q1.Unit);

        // Return new Quantity
        return new Quantity(q1.Unit, result);
    }

    public double ConvertTo(LengthUnit target)
    {
        return Convert(this.Value, this.Unit, target);
    }
}