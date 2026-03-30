namespace QuantityMeasurementAppBusinessLayer.Interfaces;

// IMeasurable — defines the behaviour contract for all measurement units.
public interface IMeasurable
{
    double GetConversionFactor();
    double ConvertToBaseUnit(double value);
    double ConvertFromBaseUnit(double baseValue);
    string GetUnitName();

    // Default: arithmetic is supported for all units except Temperature
    bool SupportsArithmetic() => true;

    // Default: no validation needed — Temperature overrides this to throw
    void ValidateOperationSupport(string operation) { }
}
