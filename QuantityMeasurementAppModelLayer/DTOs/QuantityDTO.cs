namespace QuantityMeasurementAppModelLayer.DTOs;

// QuantityDTO — plain data carrier passed between layers.
// UnitName  : e.g. "Feet", "Kilogram", "Celsius"
// Category  : e.g. "LENGTH", "WEIGHT", "VOLUME", "TEMPERATURE"
public class QuantityDTO
{
    public double Value    { get; }
    public string UnitName { get; }
    public string Category { get; }

    public QuantityDTO(double value, string unitName, string category)
    {
        if (string.IsNullOrWhiteSpace(unitName))
            throw new ArgumentException("UnitName cannot be empty");

        if (string.IsNullOrWhiteSpace(category))
            throw new ArgumentException("Category cannot be empty");

        Value    = value;
        UnitName = unitName.Trim();
        Category = category.Trim().ToUpperInvariant();
    }

    public override string ToString()
        => $"QuantityDTO({Value}, {UnitName}, {Category})";
}
