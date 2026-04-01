namespace QuantityMeasurementAppModelLayer.Models;

/// <summary>
/// Lightweight value-holder for one operand: its numeric value and unit enum.
/// Stored as a JSON column inside <c>QuantityMeasurementEntity</c>.
/// </summary>
// This is a simple model that holds a value and its unit (like 10 and Feet).
public class QuantityModel<T>
{
    /// <summary>Numeric magnitude of the quantity.</summary>
    public double Value { get; set; }

    /// <summary>Unit enum value (e.g. LengthUnit.Feet).</summary>
    public T Unit { get; set; } = default!;

    /// <summary>Parameterless constructor required for JSON deserialisation.</summary>
    public QuantityModel() { }

    /// <summary>Initialises a new quantity model with the given value and unit.</summary>
    public QuantityModel(double value, T unit)
    {
        Value = value;
        Unit  = unit;
    }

    public override string ToString() => $"{Value} {Unit}";
}
