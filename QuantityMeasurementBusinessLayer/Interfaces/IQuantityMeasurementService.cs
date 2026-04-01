using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Entities;

namespace QuantityMeasurementAppBusinessLayer.Interfaces;

/// <summary>
/// UC17 service contract.
/// All mutation operations return a <see cref="QuantityDTO"/> for a structured result.
/// History/query methods return <see cref="QuantityMeasurementEntity"/> lists for
/// mapping to <see cref="QuantityMeasurementDTO"/> in the controller layer.
/// </summary>
// This file is like a "Rule Book" that lists all the things our measurement service must be able to do.
public interface IQuantityMeasurementService
{
    // ── Core operations ───────────────────────────────────────────────

    /// <summary>Compares two quantities in base units. Returns true when equal.</summary>
    bool Compare(QuantityDTO q1, QuantityDTO q2);

    /// <summary>Converts q1 to <paramref name="targetUnit"/>. Returns the result DTO.</summary>
    QuantityDTO Convert(QuantityDTO q1, string targetUnit);

    /// <summary>Adds q1 and q2. Result is expressed in q1's unit.</summary>
    QuantityDTO Add(QuantityDTO q1, QuantityDTO q2);

    /// <summary>Subtracts q2 from q1. Result is expressed in q1's unit.</summary>
    QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2);

    /// <summary>Divides q1 by q2. Returns a dimensionless scalar ratio.</summary>
    double Divide(QuantityDTO q1, QuantityDTO q2);

    // ── History / query methods (UC17 additions) ──────────────────────

    /// <summary>Returns all persisted measurement records ordered by CreatedAt descending.</summary>
    List<QuantityMeasurementEntity> GetHistory();

    /// <summary>Returns records filtered by operation type (e.g. "Add", "Compare").</summary>
    List<QuantityMeasurementEntity> GetByOperation(string operation);

    /// <summary>Returns records filtered by measurement category (e.g. "LENGTH", "WEIGHT").</summary>
    List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType);

    /// <summary>Returns the total number of persisted records.</summary>
    int GetCount();
}
