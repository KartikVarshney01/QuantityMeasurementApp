namespace QuantityMeasurementAppModelLayer.Entities;

// IMeasurable — entity interface that defines the behavior contract for all measurement units
// All unit types (Length, Weight, Volume, Temperature) must implement this
public interface IMeasurable
{
    double GetConversionFactor();              // Returns factor to convert to base unit
    double ConvertToBaseUnit(double value);    // Converts a value to the base unit
    double ConvertFromBaseUnit(double baseValue); // Converts a base unit value back to this unit
    string GetUnitName();                      // Returns the unit name as a string

    // Default: arithmetic is supported for all units except Temperature
    bool SupportsArithmetic() => true;

    // Default: no validation needed — Temperature overrides this to throw
    void ValidateOperationSupport(string operation) { }
}
    