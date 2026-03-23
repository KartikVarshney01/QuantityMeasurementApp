using QuantityMeasurementAppModelLayer.Dto;

namespace QuantityMeasurementAppModelLayer.Entities;

// UC15: Entity for storing one operation record in the repository
public class QuantityMeasurementEntity
{
    public string       OperationType { get; }
    public QuantityDTO? Operand1      { get; }
    public QuantityDTO? Operand2      { get; }
    public QuantityDTO? Result        { get; }
    public bool         HasError      { get; }
    public string       ErrorMessage  { get; }
    public DateTime     Timestamp     { get; }

    // Single operand — Convert
    public QuantityMeasurementEntity(
        string operationType,
        QuantityDTO operand1,
        QuantityDTO? result)
    {
        OperationType = operationType;
        Operand1      = operand1;
        Result        = result;
        HasError      = false;
        ErrorMessage  = string.Empty;
        Timestamp     = DateTime.UtcNow;
    }

    // Two operands — Compare, Add, Subtract, Divide
    public QuantityMeasurementEntity(
        string operationType,
        QuantityDTO operand1,
        QuantityDTO operand2,
        QuantityDTO result)
    {
        OperationType = operationType;
        Operand1      = operand1;
        Operand2      = operand2;
        Result        = result;
        HasError      = false;
        ErrorMessage  = string.Empty;
        Timestamp     = DateTime.UtcNow;
    }

    // Error record
    public QuantityMeasurementEntity(
        string operationType,
        QuantityDTO? operand1,
        QuantityDTO? operand2,
        string errorMessage)
    {
        OperationType = operationType;
        Operand1      = operand1;
        Operand2      = operand2;
        HasError      = true;
        ErrorMessage  = errorMessage;
        Timestamp     = DateTime.UtcNow;
    }

    public override string ToString()
    {
        string time = Timestamp.ToString("HH:mm:ss");

        if (HasError)
            return $"[{time}] {OperationType} " +
                   $"| {Operand1?.Value} {Operand1?.UnitName}" +
                   (Operand2 != null ? $" | {Operand2.Value} {Operand2.UnitName}" : "") +
                   $" => Error: {ErrorMessage}";

        if (OperationType == "CONVERT")
            return $"[{time}] CONVERT " +
                   $"| {Operand1?.Value} {Operand1?.UnitName} " +
                   $"=> {Result?.Value} {Result?.UnitName}";

        if (OperationType == "COMPARE")
            return $"[{time}] COMPARE " +
                   $"| {Operand1?.Value} {Operand1?.UnitName} " +
                   $"vs {Operand2?.Value} {Operand2?.UnitName} " +
                   $"=> {(Result?.Value == 1 ? "EQUAL" : "NOT EQUAL")}";

        if (OperationType == "DIVIDE")
            return $"[{time}] DIVIDE " +
                   $"| {Operand1?.Value} {Operand1?.UnitName} " +
                   $"/ {Operand2?.Value} {Operand2?.UnitName} " +
                   $"=> {Result?.Value} (ratio)";

        return $"[{time}] {OperationType} " +
               $"| {Operand1?.Value} {Operand1?.UnitName} " +
               $", {Operand2?.Value} {Operand2?.UnitName} " +
               $"=> {Result?.Value} {Result?.UnitName}";
    }
}
