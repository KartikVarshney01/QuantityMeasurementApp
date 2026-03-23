namespace QuantityMeasurementApp.Interface;

// UC15: Application interface — Program.cs depends only on this contract
// Decouples the entry point from the concrete controller implementation
public interface IQuantityMeasurementApp
{
    void Run();
}
