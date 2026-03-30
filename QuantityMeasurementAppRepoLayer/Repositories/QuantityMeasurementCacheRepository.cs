using System.Text.Json;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppBusinessLayer.Interfaces;

namespace QuantityMeasurementAppRepoLayer.Repositories;

public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
{
    private static QuantityMeasurementCacheRepository? _instance;
    private static readonly object _lock = new();

    private readonly List<QuantityMeasurementEntity> _cache = new();

    private static readonly string _filePath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "QuantityMeasurement.json");

    private QuantityMeasurementCacheRepository()
    {
        LoadFromDisk();
        Console.WriteLine("[CacheRepository] Initialized. File: " + _filePath);
    }

    // Singleton — matches reference zip pattern
    public static QuantityMeasurementCacheRepository GetInstance()
    {
        if (_instance == null)
            lock (_lock)
                _instance ??= new QuantityMeasurementCacheRepository();
        return _instance;
    }

    // ── UC15 core ─────────────────────────────────────────────────────

    public void Save(QuantityMeasurementEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        lock (_lock) { _cache.Add(entity); }
        SaveToDisk();
    }

    public List<QuantityMeasurementEntity> GetAll()
    {
        lock (_lock) { return new List<QuantityMeasurementEntity>(_cache); }
    }

    public void Clear()
    {
        lock (_lock) { _cache.Clear(); }
        SaveToDisk();
    }

    // ── UC16 new methods ──────────────────────────────────────────────

    public List<QuantityMeasurementEntity> GetByOperation(string operation)
    {
        lock (_lock)
        {
            return _cache
                .Where(e => e.OperationType != null &&
                            e.OperationType.Equals(operation, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    public List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType)
    {
        lock (_lock)
        {
            return _cache
                .Where(e =>
                    (e.Operand1 != null && e.Operand1.Category.Equals(measurementType, StringComparison.OrdinalIgnoreCase)) ||
                    (e.Operand2 != null && e.Operand2.Category.Equals(measurementType, StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }
    }

    public int GetTotalCount()
    {
        lock (_lock) { return _cache.Count; }
    }

    public void DeleteAll()
    {
        lock (_lock) { _cache.Clear(); }
        SaveToDisk();
        Console.WriteLine("[CacheRepository] All measurements deleted.");
    }

    public string GetPoolStatistics()
        => $"In-memory cache. No connection pool. Records: {_cache.Count}";

    public void ReleaseResources()
        => Console.WriteLine("[CacheRepository] No resources to release.");

    // ── Disk persistence (from reference zip) ─────────────────────────

    private void SaveToDisk()
    {
        try
        {
            // Entity uses QuantityDTO which is not [Serializable] in the JSON sense,
            // so we save a simple summary list instead
            var summaries = _cache.Select(e => e.ToString()).ToList();
            File.WriteAllText(_filePath, JsonSerializer.Serialize(summaries));
        }
        catch (Exception ex)
        {
            Console.WriteLine("[CacheRepository] Warning: could not save: " + ex.Message);
        }
    }

    private void LoadFromDisk()
    {
        // History is rebuilt in memory each run; disk file is for audit only
        if (!File.Exists(_filePath)) return;
        Console.WriteLine("[CacheRepository] Previous history file found at: " + _filePath);
    }
}
