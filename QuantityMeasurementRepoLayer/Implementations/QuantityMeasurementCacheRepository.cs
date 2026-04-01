using System.Text.Json;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppRepoLayer.Interfaces;

namespace QuantityMeasurementAppRepoLayer.Implementations;

/// <summary>
/// UC17: In-memory + JSON-file cache repository.
/// Used as automatic fallback when SQL Server is unavailable (Program.cs detects no connection string)
/// and as the backing store for unit tests (no database required).
/// Thread-safe via a private lock object.
/// </summary>
// This file saves measurements in the computer's memory (RAM) instead of a database.
public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
{
    private static QuantityMeasurementCacheRepository? _instance;
    private static readonly object _lock = new();

    private readonly List<QuantityMeasurementEntity> _cache = new();

    private static readonly string _filePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "QuantityMeasurement_cache.json");

    private QuantityMeasurementCacheRepository()
    {
        LoadFromDisk();
        Console.WriteLine("[CacheRepository] Initialized. Backup file: " + _filePath);
    }

    // ── Singleton ─────────────────────────────────────────────────────

    /// <summary>Returns the single shared instance (thread-safe double-checked locking).</summary>
    public static QuantityMeasurementCacheRepository GetInstance()
    {
        if (_instance is null)
            lock (_lock)
                _instance ??= new QuantityMeasurementCacheRepository();
        return _instance;
    }

    /// <summary>Resets the singleton — use only in unit tests.</summary>
    public static void ResetForTesting()
    {
        lock (_lock) { _instance = null; }
    }

    // ── Save ──────────────────────────────────────────────────────────

    public void Save(QuantityMeasurementEntity entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        if (entity.CreatedAt == default)
            entity.CreatedAt = DateTime.UtcNow;

        lock (_lock) { _cache.Add(entity); }
        SaveToDisk();
    }

    // ── GetAll ────────────────────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetAll()
    {
        lock (_lock)
            return _cache.OrderByDescending(e => e.CreatedAt).ToList();
    }

    // ── GetByOperation ────────────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetByOperation(string operation)
    {
        lock (_lock)
            return _cache
                .Where(e => e.Operation.Equals(operation, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
    }

    // ── GetByMeasurementType ──────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
    {
        lock (_lock)
            return _cache
                .Where(e =>
                    (e.Operand1?.ToString()?.Contains(measurementType,
                        StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Operand2?.ToString()?.Contains(measurementType,
                        StringComparison.OrdinalIgnoreCase) ?? false))
                .OrderByDescending(e => e.CreatedAt)
                .ToList();
    }

    // ── GetTotalCount ─────────────────────────────────────────────────

    public int GetTotalCount()
    {
        lock (_lock) return _cache.Count;
    }

    // ── DeleteAll ─────────────────────────────────────────────────────

    public void DeleteAll()
    {
        lock (_lock) { _cache.Clear(); }
        SaveToDisk();
        Console.WriteLine("[CacheRepository] All measurements deleted.");
    }

    // ── CloseResources ────────────────────────────────────────────────

    public void CloseResources()
        => Console.WriteLine("[CacheRepository] No external resources to release.");

    // ── Disk persistence ──────────────────────────────────────────────

    private void SaveToDisk()
    {
        try
        {
            var summaries = _cache.Select(e => e.ToString()).ToList();
            File.WriteAllText(_filePath,
                JsonSerializer.Serialize(summaries,
                    new JsonSerializerOptions { WriteIndented = true }));
        }
        catch (Exception ex)
        {
            Console.WriteLine("[CacheRepository] Warning: could not write to disk: " + ex.Message);
        }
    }

    private void LoadFromDisk()
    {
        if (!File.Exists(_filePath)) return;
        Console.WriteLine("[CacheRepository] Previous history file found: " + _filePath);
    }
}
