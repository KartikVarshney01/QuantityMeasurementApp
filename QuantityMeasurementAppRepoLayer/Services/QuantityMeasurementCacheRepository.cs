using QuantityMeasurementAppRepoLayer.Interface;
using QuantityMeasurementAppModelLayer.Entities;

namespace QuantityMeasurementAppRepoLayer.Services;

// Singleton in-memory repository — stores operation history for the app lifetime
public class QuantityMeasurementCacheRepository : IQuantityMeasurementRepository
{
    private static readonly Lazy<QuantityMeasurementCacheRepository> _instance
        = new(() => new QuantityMeasurementCacheRepository());

    public static QuantityMeasurementCacheRepository Instance => _instance.Value;

    private readonly List<QuantityMeasurementEntity> _cache = new();
    private readonly object _lock = new();

    private QuantityMeasurementCacheRepository() { }

    public void Save(QuantityMeasurementEntity entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        lock (_lock) { _cache.Add(entity); }
    }

    public IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements()
    {
        lock (_lock) { return _cache.AsReadOnly(); }
    }

    public void Clear()
    {
        lock (_lock) { _cache.Clear(); }
    }
}
