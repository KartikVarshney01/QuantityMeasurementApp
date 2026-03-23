using QuantityMeasurementAppModelLayer.Entities;

namespace QuantityMeasurementAppRepoLayer.Interface;

// UC15: Repository interface — stores operation history
public interface IQuantityMeasurementRepository
{
    void Save(QuantityMeasurementEntity entity);
    IReadOnlyList<QuantityMeasurementEntity> GetAllMeasurements();
    void Clear();
}
