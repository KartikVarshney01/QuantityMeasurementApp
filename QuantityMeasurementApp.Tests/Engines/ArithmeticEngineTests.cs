using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppBusinessLayer.Engines;
using QuantityMeasurementAppBusinessLayer.Exceptions;

// These tests check if our basic math (like adding numbers) is working correctly.
namespace QuantityMeasurementApp.Tests.Engines;

[TestClass]
public class ArithmeticEngineTests
{
    // ── Add ───────────────────────────────────────────────────────────

    [TestMethod]
    public void Add_Length_ShouldReturnCorrectSum()
        => Assert.AreEqual(15, ArithmeticEngine.Add(10, 5, "LENGTH"));

    [TestMethod]
    public void Add_Weight_ShouldReturnCorrectSum()
        => Assert.AreEqual(5.0, ArithmeticEngine.Add(3.5, 1.5, "WEIGHT"), 1e-9);

    [TestMethod]
    public void Add_Volume_ShouldReturnCorrectSum()
        => Assert.AreEqual(5, ArithmeticEngine.Add(2, 3, "VOLUME"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Add_Temperature_ShouldThrow()
        => ArithmeticEngine.Add(100, 50, "TEMPERATURE");

    // ── Subtract ──────────────────────────────────────────────────────

    [TestMethod]
    public void Subtract_Length_ShouldReturnCorrectDifference()
        => Assert.AreEqual(5, ArithmeticEngine.Subtract(10, 5, "LENGTH"));

    [TestMethod]
    public void Subtract_Weight_ShouldReturnCorrectDifference()
        => Assert.AreEqual(3, ArithmeticEngine.Subtract(5, 2, "WEIGHT"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Subtract_Temperature_ShouldThrow()
        => ArithmeticEngine.Subtract(100, 50, "TEMPERATURE");

    // ── Divide ────────────────────────────────────────────────────────

    [TestMethod]
    public void Divide_Length_ShouldReturnCorrectQuotient()
        => Assert.AreEqual(2, ArithmeticEngine.Divide(10, 5, "LENGTH"));

    [TestMethod]
    public void Divide_ByOne_ShouldReturnSameValue()
        => Assert.AreEqual(10, ArithmeticEngine.Divide(10, 1, "WEIGHT"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Divide_ByZero_ShouldThrow()
        => ArithmeticEngine.Divide(10, 0, "LENGTH");

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void Divide_Temperature_ShouldThrow()
        => ArithmeticEngine.Divide(100, 50, "TEMPERATURE");
}
