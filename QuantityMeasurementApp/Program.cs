using QuantityMeasurementAppBusinessLayer.Interface;
using QuantityMeasurementAppBusinessLayer.Service;
using QuantityMeasurementAppRepoLayer.Interface;
using QuantityMeasurementAppRepoLayer.Services;
using QuantityMeasurementApp.Interface;
using QuantityMeasurementApp.Controller;

// ── Dependency wiring — wire all layers bottom-up ────────────────────
IQuantityMeasurementRepository repository  = QuantityMeasurementCacheRepository.Instance;
IQuantityMeasurementService    service     = new QuantityMeasurementServiceImpl(repository);
IQuantityMeasurementApp        app         = new QuantityMeasurementController(service, repository);

// ── Start the application ────────────────────────────────────────────
app.Run();
