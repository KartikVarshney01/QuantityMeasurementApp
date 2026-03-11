namespace QuantityMeasurementApp;

// Interface that defines behavior for measurable units
public interface IMeasurable
{
    double GetConversionFactor(); // Returns conversion factor to base unit
    double ConvertToBaseUnit(double value); // Converts value to base unit
    double ConvertFromBaseUnit(double baseValue); // Converts base unit value to current unit
    string GetUnitName(); // Returns name of the unit

    bool SupportsArithmetic()
    {
        return true; // By default arithmetic operations are supported
    }

    void ValidateOperationSupport(string operation)
    {
        // Method to validate if a specific operation is supported
    }
}