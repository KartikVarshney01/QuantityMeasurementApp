using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppModelLayer.Models;
using QuantityMeasurementAppRepoLayer.Implementations;
using QuantityMeasurementAppRepoLayer.Interfaces;

// These tests check if our database/cache is saving and loading records correctly.
namespace QuantityMeasurementApp.Tests.Repository;

[TestClass]
public class QuantityMeasurementRepositoryTests
{
    private IQuantityMeasurementRepository _repo = null!;

    [TestInitialize]
    public void Setup()
    {
        QuantityMeasurementCacheRepository.ResetForTesting();
        _repo = QuantityMeasurementCacheRepository.GetInstance();
    }

    // ── Singleton ─────────────────────────────────────────────────────

    [TestMethod]
    public void GetInstance_CalledTwice_ReturnsSameObject()
    {
        var r1 = QuantityMeasurementCacheRepository.GetInstance();
        var r2 = QuantityMeasurementCacheRepository.GetInstance();
        Assert.AreSame(r1, r2);
    }

    // ── Save / GetAll ─────────────────────────────────────────────────

    [TestMethod]
    public void Save_ValidEntity_IncreasesCount()
    {
        int before = _repo.GetTotalCount();
        _repo.Save(MakeEntity("Add", "LENGTH"));
        Assert.AreEqual(before + 1, _repo.GetTotalCount());
    }

    [TestMethod]
    public void GetAll_AfterSave_ContainsSavedEntity()
    {
        _repo.Save(MakeEntity("Compare", "WEIGHT"));
        var all = _repo.GetAll();
        Assert.IsTrue(all.Count > 0);
        Assert.AreEqual("Compare", all[0].Operation);
    }

    [TestMethod]
    public void GetAll_ReturnsNewestFirst()
    {
        _repo.Save(MakeEntity("Add",      "LENGTH"));
        _repo.Save(MakeEntity("Subtract", "LENGTH"));
        Assert.AreEqual("Subtract", _repo.GetAll()[0].Operation);
    }

    // ── GetByOperation ────────────────────────────────────────────────

    [TestMethod]
    public void GetByOperation_ReturnsMatchingRecords()
    {
        _repo.Save(MakeEntity("Add",     "LENGTH"));
        _repo.Save(MakeEntity("Convert", "WEIGHT"));
        _repo.Save(MakeEntity("Add",     "VOLUME"));

        var adds = _repo.GetByOperation("Add");
        Assert.AreEqual(2, adds.Count);
        Assert.IsTrue(adds.All(e => e.Operation == "Add"));
    }

    [TestMethod]
    public void GetByOperation_CaseInsensitive_ReturnsResults()
    {
        _repo.Save(MakeEntity("Compare", "LENGTH"));
        Assert.IsTrue(_repo.GetByOperation("compare").Count >= 1);
    }

    [TestMethod]
    public void GetByOperation_NoMatch_ReturnsEmpty()
    {
        _repo.Save(MakeEntity("Add", "LENGTH"));
        Assert.AreEqual(0, _repo.GetByOperation("Divide").Count);
    }

    // ── GetTotalCount ─────────────────────────────────────────────────

    [TestMethod]
    public void GetTotalCount_ReflectsAllSaves()
    {
        _repo.Save(MakeEntity("Add",      "LENGTH"));
        _repo.Save(MakeEntity("Subtract", "WEIGHT"));
        _repo.Save(MakeEntity("Compare",  "VOLUME"));
        Assert.AreEqual(3, _repo.GetTotalCount());
    }

    // ── DeleteAll ─────────────────────────────────────────────────────

    [TestMethod]
    public void DeleteAll_LeavesZeroRecords()
    {
        _repo.Save(MakeEntity("Add", "LENGTH"));
        _repo.Save(MakeEntity("Add", "LENGTH"));
        _repo.DeleteAll();
        Assert.AreEqual(0, _repo.GetTotalCount());
    }

    // ── Error entity ──────────────────────────────────────────────────

    [TestMethod]
    public void Save_ErrorEntity_PersistedWithErrorFlag()
    {
        var error = new QuantityMeasurementEntity("Add", "Unit mismatch error");
        _repo.Save(error);

        var all = _repo.GetAll();
        Assert.IsTrue(all.Any(e => e.HasError && e.ErrorMessage == "Unit mismatch error"));
    }

    // ── QuantityMeasurementDTO factory methods ────────────────────────

    [TestMethod]
    public void FromEntity_MapsAllFields()
    {
        var entity = MakeEntity("Add", "LENGTH");
        entity.Result = 42.0;

        var dto = QuantityMeasurementAppModelLayer.DTOs.QuantityMeasurementDTO.FromEntity(entity);

        Assert.AreEqual("Add",  dto.Operation);
        Assert.AreEqual(42.0,   dto.Result);
        Assert.IsFalse(dto.HasError);
    }

    [TestMethod]
    public void FromEntityList_MapsAllEntities()
    {
        var entities = new List<QuantityMeasurementEntity>
        {
            MakeEntity("Add",     "LENGTH"),
            MakeEntity("Compare", "WEIGHT")
        };

        var dtos = QuantityMeasurementAppModelLayer.DTOs.QuantityMeasurementDTO.FromEntityList(entities);

        Assert.AreEqual(2, dtos.Count);
        Assert.AreEqual("Add",     dtos[0].Operation);
        Assert.AreEqual("Compare", dtos[1].Operation);
    }

    // ── Helper ────────────────────────────────────────────────────────

    private static QuantityMeasurementEntity MakeEntity(string op, string category)
        => new QuantityMeasurementEntity(
            op,
            new QuantityModel<object>(10, category + "_UNIT1"),
            new QuantityModel<object>(5,  category + "_UNIT2"),
            result: 15.0);
}
