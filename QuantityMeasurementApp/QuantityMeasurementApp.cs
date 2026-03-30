using QuantityMeasurementApp.Controllers;
using QuantityMeasurementApp.Interfaces;
using QuantityMeasurementApp.Menu;
using QuantityMeasurementAppBusinessLayer.Interfaces;
using QuantityMeasurementAppBusinessLayer.Services;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppRepoLayer.Repositories;
using QuantityMeasurementAppRepoLayer.Utilities;

namespace QuantityMeasurementApp;

public class QuantityMeasurementApp
{
    private static QuantityMeasurementApp? _instance;
    private static readonly object _lock = new();

    private readonly QuantityMeasurementController _controller;
    private readonly IMenu _menu;
    private readonly IQuantityMeasurementRepository _repository;
    private readonly string _activeRepositoryType;

    private QuantityMeasurementApp()
    {
        Console.WriteLine("[App] Starting Quantity Measurement Application...");

        ApplicationConfig config = new ApplicationConfig();
        string repoType          = config.GetRepositoryType();
        Console.WriteLine("[App] Configured repository type: " + repoType);

        if (repoType.Equals("database", StringComparison.OrdinalIgnoreCase))
        {
            (_repository, _activeRepositoryType) = TryInitializeDatabaseRepository(config);
        }
        else
        {
            Console.WriteLine("[App] Repository type is cache. Skipping database.");
            _repository           = QuantityMeasurementCacheRepository.GetInstance();
            _activeRepositoryType = "Cache (in-memory + JSON backup)";
            Console.WriteLine("[App] Cache Repository ready.");
        }

        IQuantityMeasurementService service = new QuantityMeasurementServiceImpl(_repository);
        _controller = new QuantityMeasurementController(service);
        _menu       = new QuantityMenu(_controller);

        Console.WriteLine("[App] Initialization complete.");
        Console.WriteLine("[App] Active repository: " + _activeRepositoryType);
        Console.WriteLine();
    }

    private static (IQuantityMeasurementRepository repo, string label) TryInitializeDatabaseRepository(
        ApplicationConfig config)
    {
        Console.WriteLine("[App] Attempting to connect to SQL Server...");
        try
        {
            ConnectionPool pool = ConnectionPool.GetInstance(config);
            var repo = new QuantityMeasurementDatabaseRepository(pool);
            Console.WriteLine("[App] SQL Server connected. Using Database Repository.");
            return (repo, "Database (SQL Server)");
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine("[App] WARNING: SQL Server is not available.");
            Console.WriteLine("[App] Reason : " + ex.Message);
            Console.WriteLine("[App] Automatically switching to Cache Repository...");
            Console.WriteLine();
            var repo = QuantityMeasurementCacheRepository.GetInstance();
            Console.WriteLine("[App] Cache Repository ready.");
            return (repo, "Cache (SQL Server offline - data saved to JSON file)");
        }
    }

    public static QuantityMeasurementApp GetInstance()
    {
        if (_instance == null)
            lock (_lock)
                _instance ??= new QuantityMeasurementApp();
        return _instance;
    }

    public void Start() => _menu.Run();

    public void ReportAllMeasurements()
    {
        Console.WriteLine("\n========== Measurement History ==========");
        Console.WriteLine("Repository : " + _activeRepositoryType);

        List<QuantityMeasurementEntity> all = _repository.GetAll();
        Console.WriteLine("Total records: " + all.Count);

        for (int i = 0; i < all.Count; i++)
            Console.WriteLine((i + 1) + ". " + all[i]);

        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("Pool stats : " + _repository.GetPoolStatistics());
        Console.WriteLine("=========================================\n");
    }

    public void DeleteAllMeasurements()
    {
        Console.WriteLine("[App] Deleting all measurements...");
        _repository.DeleteAll();
        Console.WriteLine("[App] Done. Count now: " + _repository.GetTotalCount());
    }

    public void CloseResources()
    {
        Console.WriteLine("[App] Closing resources...");
        _repository.ReleaseResources();
        Console.WriteLine("[App] Resources closed.");
    }
}
