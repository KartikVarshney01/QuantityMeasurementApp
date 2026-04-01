using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuantityMeasurementAppBusinessLayer.Exceptions;
using QuantityMeasurementAppBusinessLayer.Interfaces;
using QuantityMeasurementAppBusinessLayer.Services;
using QuantityMeasurementAppModelLayer.DTOs;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppModelLayer.Models;
using QuantityMeasurementAppRepoLayer.Implementations;
using QuantityMeasurementAppRepoLayer.Interfaces;

// These tests check if our main measurement service is working correctly.
namespace QuantityMeasurementApp.Tests.Services;

[TestClass]
public class QuantityMeasurementServiceTests
{
    private IQuantityMeasurementService          _service  = null!;
    private Mock<IQuantityMeasurementRepository> _mockRepo = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IQuantityMeasurementRepository>();
        _mockRepo.Setup(r => r.Save(It.IsAny<QuantityMeasurementEntity>()));
        _mockRepo.Setup(r => r.GetAll()).Returns(new List<QuantityMeasurementEntity>());
        _service  = new QuantityMeasurementServiceImpl(_mockRepo.Object);
    }

    // ── COMPARE ──────────────────────────────────────────────────────

    [TestMethod]
    public void Compare_1Foot_And_12Inches_ShouldBeEqual()
        => Assert.IsTrue(_service.Compare(
            new QuantityDTO(1, "Feet", "LENGTH"),
            new QuantityDTO(12, "Inch", "LENGTH")));

    [TestMethod]
    public void Compare_1Foot_And_1Yard_ShouldNotBeEqual()
        => Assert.IsFalse(_service.Compare(
            new QuantityDTO(1, "Feet", "LENGTH"),
            new QuantityDTO(1, "Yard", "LENGTH")));

    [TestMethod]
    public void Compare_1Kg_And_1000Grams_ShouldBeEqual()
        => Assert.IsTrue(_service.Compare(
            new QuantityDTO(1, "Kilogram", "WEIGHT"),
            new QuantityDTO(1000, "Gram", "WEIGHT")));

    [TestMethod]
    public void Compare_0Celsius_And_32Fahrenheit_ShouldBeEqual()
        => Assert.IsTrue(_service.Compare(
            new QuantityDTO(0, "Celsius", "TEMPERATURE"),
            new QuantityDTO(32, "Fahrenheit", "TEMPERATURE")));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Compare_DifferentCategories_ShouldThrow()
        => _service.Compare(
            new QuantityDTO(1, "Feet",     "LENGTH"),
            new QuantityDTO(1, "Kilogram", "WEIGHT"));

    [TestMethod]
    public void Compare_ShouldPersistToRepository()
    {
        _service.Compare(
            new QuantityDTO(1, "Feet", "LENGTH"),
            new QuantityDTO(1, "Feet", "LENGTH"));
        _mockRepo.Verify(r => r.Save(It.IsAny<QuantityMeasurementEntity>()), Times.Once);
    }

    // ── CONVERT ──────────────────────────────────────────────────────

    [TestMethod]
    public void Convert_1Foot_ToInch_ShouldReturn12()
    {
        var result = _service.Convert(new QuantityDTO(1, "Feet", "LENGTH"), "Inch");
        Assert.AreEqual(12.0, result.Value, 1e-4);
        Assert.AreEqual("Inch", result.UnitName);
    }

    [TestMethod]
    public void Convert_1Litre_ToMillilitre_ShouldReturn1000()
    {
        var result = _service.Convert(new QuantityDTO(1, "Litre", "VOLUME"), "Millilitre");
        Assert.AreEqual(1000.0, result.Value, 1e-4);
    }

    [TestMethod]
    public void Convert_0Celsius_ToFahrenheit_ShouldReturn32()
    {
        var result = _service.Convert(new QuantityDTO(0, "Celsius", "TEMPERATURE"), "Fahrenheit");
        Assert.AreEqual(32.0, result.Value, 1e-4);
    }

    [TestMethod]
    public void Convert_1Kg_ToPound_ShouldReturnCorrectValue()
    {
        var result = _service.Convert(new QuantityDTO(1, "Kilogram", "WEIGHT"), "Pound");
        Assert.AreEqual(2.2046, result.Value, 0.001);
    }

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Convert_UnknownTargetUnit_ShouldThrow()
        => _service.Convert(new QuantityDTO(1, "Feet", "LENGTH"), "Parsec");

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Convert_EmptyTargetUnit_ShouldThrow()
        => _service.Convert(new QuantityDTO(1, "Feet", "LENGTH"), "");

    // ── ADD ──────────────────────────────────────────────────────────

    [TestMethod]
    public void Add_1Foot_And_12Inches_ShouldReturn2Feet()
    {
        var result = _service.Add(
            new QuantityDTO(1, "Feet", "LENGTH"),
            new QuantityDTO(12, "Inch", "LENGTH"));
        Assert.AreEqual(2.0, result.Value, 1e-4);
        Assert.AreEqual("Feet", result.UnitName);
    }

    [TestMethod]
    public void Add_SameUnit_ShouldReturnDirectSum()
    {
        var result = _service.Add(
            new QuantityDTO(5, "Kilogram", "WEIGHT"),
            new QuantityDTO(3, "Kilogram", "WEIGHT"));
        Assert.AreEqual(8.0, result.Value, 1e-4);
    }

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Add_Temperature_ShouldThrow()
        => _service.Add(
            new QuantityDTO(100, "Celsius",    "TEMPERATURE"),
            new QuantityDTO(50,  "Fahrenheit", "TEMPERATURE"));

    // ── SUBTRACT ─────────────────────────────────────────────────────

    [TestMethod]
    public void Subtract_24Inch_Minus_12Inch_ShouldReturn12()
    {
        var result = _service.Subtract(
            new QuantityDTO(24, "Inch", "LENGTH"),
            new QuantityDTO(12, "Inch", "LENGTH"));
        Assert.AreEqual(12.0, result.Value, 1e-4);
    }

    [TestMethod]
    public void Subtract_2Feet_Minus_12Inches_ShouldReturn1Foot()
    {
        var result = _service.Subtract(
            new QuantityDTO(2,  "Feet", "LENGTH"),
            new QuantityDTO(12, "Inch", "LENGTH"));
        Assert.AreEqual(1.0, result.Value, 1e-4);
    }

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Subtract_Temperature_ShouldThrow()
        => _service.Subtract(
            new QuantityDTO(100, "Celsius",    "TEMPERATURE"),
            new QuantityDTO(50,  "Fahrenheit", "TEMPERATURE"));

    // ── DIVIDE ───────────────────────────────────────────────────────

    [TestMethod]
    public void Divide_24Inch_By_12Inch_ShouldReturn2()
        => Assert.AreEqual(2.0,
            _service.Divide(
                new QuantityDTO(24, "Inch", "LENGTH"),
                new QuantityDTO(12, "Inch", "LENGTH")), 1e-9);

    [TestMethod]
    public void Divide_1Litre_By_500ml_ShouldReturn2()
        => Assert.AreEqual(2.0,
            _service.Divide(
                new QuantityDTO(1,   "Litre",      "VOLUME"),
                new QuantityDTO(500, "Millilitre", "VOLUME")), 1e-6);

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Divide_ByZero_ShouldThrow()
        => _service.Divide(
            new QuantityDTO(10, "Feet", "LENGTH"),
            new QuantityDTO(0,  "Feet", "LENGTH"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Divide_Temperature_ShouldThrow()
        => _service.Divide(
            new QuantityDTO(100, "Celsius",    "TEMPERATURE"),
            new QuantityDTO(50,  "Fahrenheit", "TEMPERATURE"));

    // ── HISTORY ──────────────────────────────────────────────────────

    [TestMethod]
    public void GetHistory_ReturnsRepositoryResults()
    {
        var entities = new List<QuantityMeasurementEntity>
        {
            new("Add",     new QuantityModel<object>(10, "Feet"),
                            new QuantityModel<object>(5,  "Feet"), 15.0),
            new("Compare", new QuantityModel<object>(1, "Kg"),
                            new QuantityModel<object>(1000, "Gram"), true)
        };
        _mockRepo.Setup(r => r.GetAll()).Returns(entities);

        Assert.AreEqual(2, _service.GetHistory().Count);
    }

    [TestMethod]
    public void GetCount_ReturnsRepositoryCount()
    {
        _mockRepo.Setup(r => r.GetTotalCount()).Returns(7);
        Assert.AreEqual(7, _service.GetCount());
    }

    // ── Integration test with real Cache repository ───────────────────

    [TestMethod]
    public void Integration_AddThenGetHistory_ReturnsRecord()
    {
        QuantityMeasurementCacheRepository.ResetForTesting();
        var svc = new QuantityMeasurementServiceImpl(
            QuantityMeasurementCacheRepository.GetInstance());

        svc.Add(new QuantityDTO(1,  "Feet", "LENGTH"),
                new QuantityDTO(12, "Inch", "LENGTH"));

        svc.Compare(new QuantityDTO(1, "Kilogram", "WEIGHT"),
                    new QuantityDTO(1000, "Gram",  "WEIGHT"));

        var history = svc.GetHistory();
        Assert.IsTrue(history.Count >= 2);
        Assert.IsTrue(history.Any(h => h.Operation == "Add"));
        Assert.IsTrue(history.Any(h => h.Operation == "Compare"));
    }
}
