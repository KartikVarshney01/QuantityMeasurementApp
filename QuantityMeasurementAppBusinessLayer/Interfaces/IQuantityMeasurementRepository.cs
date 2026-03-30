using QuantityMeasurementAppModelLayer.Entities;

namespace QuantityMeasurementAppBusinessLayer.Interfaces;

public interface IQuantityMeasurementRepository
{
    void Save(QuantityMeasurementEntity entity);
    List<QuantityMeasurementEntity> GetAll();
    void Clear();

    List<QuantityMeasurementEntity> GetByOperation(string operation);
    List<QuantityMeasurementEntity> GetByMeasurementType(string measurementType);
    int    GetTotalCount();
    void   DeleteAll();
    string GetPoolStatistics();
    void   ReleaseResources();
}
