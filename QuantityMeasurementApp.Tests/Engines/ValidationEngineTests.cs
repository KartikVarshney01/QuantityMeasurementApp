using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppBusinessLayer.Engines;
using QuantityMeasurementAppBusinessLayer.Exceptions;
using QuantityMeasurementAppModelLayer.DTOs;

// These tests check if our validation logic (like blocking invalid unit mixes) works.
namespace QuantityMeasurementApp.Tests.Engines;

[TestClass]
public class ValidationEngineTests
{
    [TestMethod]
    public void ValidateSameMeasurement_SameCategory_ShouldPass()
    {
        ValidationEngine.ValidateSameMeasurement(
            new QuantityDTO(1,  "Feet", "LENGTH"),
            new QuantityDTO(12, "Inch", "LENGTH"));
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void ValidateSameMeasurement_CaseInsensitive_ShouldPass()
    {
        ValidationEngine.ValidateSameMeasurement(
            new QuantityDTO(1, "Feet",     "LENGTH"),
            new QuantityDTO(1, "Kilogram", "length"));   // lowercase — still same
        Assert.IsTrue(true);
    }

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ValidateSameMeasurement_LengthVsWeight_ShouldThrow()
        => ValidationEngine.ValidateSameMeasurement(
            new QuantityDTO(1, "Feet",     "LENGTH"),
            new QuantityDTO(1, "Kilogram", "WEIGHT"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ValidateSameMeasurement_LengthVsTemperature_ShouldThrow()
        => ValidationEngine.ValidateSameMeasurement(
            new QuantityDTO(1,   "Feet",    "LENGTH"),
            new QuantityDTO(100, "Celsius", "TEMPERATURE"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ValidateTargetUnit_NullString_ShouldThrow()
        => ValidationEngine.ValidateTargetUnit(null);

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ValidateTargetUnit_EmptyString_ShouldThrow()
        => ValidationEngine.ValidateTargetUnit("   ");

    [TestMethod]
    public void ValidateTargetUnit_ValidUnit_ShouldPass()
    {
        ValidationEngine.ValidateTargetUnit("Inch");
        Assert.IsTrue(true);
    }
}
