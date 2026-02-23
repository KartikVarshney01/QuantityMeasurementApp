using System;
namespace QuantityMeasurementApp.Models;

public class Inch
{
    // Immutable property
    private readonly double Value;

    // Constructor
    public Inch(double value)
    {
        Value = value;
    }

    // Override Equals - UC-1
    public override bool Equals(object obj)
    {
        // Reflexive
        if(this == obj) return true;

        // Null + Type safety
        if(obj == null || GetType() != obj.GetType()) return false;
        
        Inch other = (Inch)obj;

        // Floating point comparison
        return Value.Equals(other.Value);  
    }

    // Must override when Equals is overridden
    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }
}