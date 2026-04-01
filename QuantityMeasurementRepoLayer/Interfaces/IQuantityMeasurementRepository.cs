using QuantityMeasurementAppModelLayer.Entities;

namespace QuantityMeasurementAppRepoLayer.Interfaces;

/// <summary>
/// UC17 repository contract satisfied by both
/// <c>EFCoreQuantityMeasurementRepository</c> (SQL Server) and
/// <c>QuantityMeasurementCacheRepository</c> (in-memory fallback).
/// </summary>
// This is another "Rule Book" for how we save and get measurements from a database or cache.
public interface IQuantityMeasurementRepository
{
    // ── Core CRUD ─────────────────────────────────────────────────────
    void Save(QuantityMeasurementEntity entity);
    List<QuantityMeasurementEntity> GetAll();
    void DeleteAll();

    // ── Filtered queries ──────────────────────────────────────────────
    List<QuantityMeasurementEntity> GetByOperation(string operation);
    List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType);

    // ── Aggregates ────────────────────────────────────────────────────
    int GetTotalCount();

    // ── Lifecycle ─────────────────────────────────────────────────────
    void CloseResources();
}
