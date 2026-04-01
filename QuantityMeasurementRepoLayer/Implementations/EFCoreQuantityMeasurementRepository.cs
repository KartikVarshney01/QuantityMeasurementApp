using Microsoft.EntityFrameworkCore;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppRepoLayer.Data;
using QuantityMeasurementAppRepoLayer.Interfaces;

namespace QuantityMeasurementAppRepoLayer.Implementations;

/// <summary>
/// UC17: Entity Framework Core repository.
/// Replaces the UC16 ADO.NET <c>QuantityMeasurementDatabaseRepository</c>:
/// no stored procedures, no manual SQL, no ConnectionPool.
/// EF Core generates all SQL; <see cref="ApplicationDbContext"/> owns the
/// connection lifetime (Scoped via ASP.NET Core DI).
/// </summary>
// This file handles saving measurements to a real SQL database.
public class EFCoreQuantityMeasurementRepository : IQuantityMeasurementRepository
{
    private readonly ApplicationDbContext _context;

    public EFCoreQuantityMeasurementRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        Console.WriteLine("[EFCoreRepository] Initialized — using Entity Framework Core.");
    }

    // ── Save ──────────────────────────────────────────────────────────

    /// <summary>Persists a <see cref="QuantityMeasurementEntity"/> to the database.</summary>
    public void Save(QuantityMeasurementEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        try
        {
            if (entity.CreatedAt == default)
                entity.CreatedAt = DateTime.UtcNow;

            _context.QuantityMeasurements.Add(entity);
            _context.SaveChanges();
        }
        catch (DbUpdateException ex)
        {
            throw new Exception("EFCoreRepository: error saving entity to database.", ex);
        }
    }

    // ── GetAll ────────────────────────────────────────────────────────

    /// <summary>Returns all records ordered by <c>CreatedAt</c> descending.</summary>
    public List<QuantityMeasurementEntity> GetAll()
    {
        try
        {
            return _context.QuantityMeasurements
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception("EFCoreRepository: error retrieving all records.", ex);
        }
    }

    // ── GetByOperation ────────────────────────────────────────────────

    /// <summary>
    /// Returns records where <c>Operation</c> matches <paramref name="operation"/>
    /// (case-insensitive), ordered by <c>CreatedAt</c> descending.
    /// </summary>
    public List<QuantityMeasurementEntity> GetByOperation(string operation)
    {
        if (string.IsNullOrWhiteSpace(operation))
            throw new ArgumentException("Operation filter cannot be null or empty.", nameof(operation));

        try
        {
            string upper = operation.ToUpperInvariant();
            return _context.QuantityMeasurements
                .Where(e => e.Operation.ToUpper() == upper)
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"EFCoreRepository: error filtering by operation '{operation}'.", ex);
        }
    }

    // ── GetByMeasurementType ──────────────────────────────────────────

    /// <summary>
    /// Returns records where either operand's JSON contains the
    /// <paramref name="measurementType"/> string (case-insensitive).
    /// Evaluated in-memory because EF Core cannot translate JSON column content
    /// to a LIKE predicate for arbitrary types without DB-specific JSON functions.
    /// </summary>
    public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
    {
        if (string.IsNullOrWhiteSpace(measurementType))
            throw new ArgumentException("MeasurementType filter cannot be null or empty.",
                                        nameof(measurementType));

        try
        {
            string upper = measurementType.ToUpperInvariant();
            return _context.QuantityMeasurements
                .AsEnumerable()                       // switch to client-side evaluation
                .Where(e =>
                    (e.Operand1 != null &&
                     e.Operand1.ToString()!.Contains(upper, StringComparison.OrdinalIgnoreCase)) ||
                    (e.Operand2 != null &&
                     e.Operand2.ToString()!.Contains(upper, StringComparison.OrdinalIgnoreCase)))
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
        }
        catch (Exception ex)
        {
            throw new Exception(
                $"EFCoreRepository: error filtering by measurementType '{measurementType}'.", ex);
        }
    }

    // ── GetTotalCount ─────────────────────────────────────────────────

    /// <summary>Returns the total number of records in the table.</summary>
    public int GetTotalCount()
    {
        try { return _context.QuantityMeasurements.Count(); }
        catch (Exception ex)
        {
            throw new Exception("EFCoreRepository: error counting records.", ex);
        }
    }

    // ── DeleteAll ─────────────────────────────────────────────────────

    /// <summary>Deletes every record from the table.</summary>
    public void DeleteAll()
    {
        try
        {
            _context.QuantityMeasurements.RemoveRange(_context.QuantityMeasurements);
            _context.SaveChanges();
            Console.WriteLine("[EFCoreRepository] All measurements deleted.");
        }
        catch (Exception ex)
        {
            throw new Exception("EFCoreRepository: error deleting all records.", ex);
        }
    }

    // ── CloseResources ────────────────────────────────────────────────

    /// <summary>
    /// No-op: EF Core <c>DbContext</c> lifetime is managed by ASP.NET Core DI (Scoped).
    /// The container disposes the context automatically at the end of each HTTP request.
    /// </summary>
    public void CloseResources()
        => Console.WriteLine("[EFCoreRepository] CloseResources — EF Core manages lifecycle.");
}
