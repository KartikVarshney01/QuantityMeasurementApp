namespace QuantityMeasurementAppBusinessLayer.Exception;

// UC15: Custom exception for all quantity measurement domain errors
public class QuantityMeasurementException : System.Exception
{
    public QuantityMeasurementException(string message) : base(message) { }

    public QuantityMeasurementException(string message, System.Exception inner)
        : base(message, inner) { }
}
