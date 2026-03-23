using QuantityMeasurementAppModelLayer.Dto;

namespace QuantityMeasurementAppBusinessLayer.Interface;

// Service contract — all operations accept and return QuantityDTO
// This keeps the service layer-agnostic with no dependency on unit enums
public interface IQuantityMeasurementService
{
    // Compare two quantities — returns QuantityDTO with Value 1 (equal) or 0 (not equal)
    QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2);

    // Convert q1 to the unit specified inside targetUnit DTO
    QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnit);

    // Add two quantities — result returned in q1's unit
    QuantityDTO Add(QuantityDTO q1, QuantityDTO q2);

    // Subtract q2 from q1 — result returned in q1's unit
    QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2);

    // Divide q1 by q2 — returns a scalar ratio DTO with Category "SCALAR"
    QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2);
}
