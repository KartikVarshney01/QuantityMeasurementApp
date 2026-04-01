using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementAppBusinessLayer.Engines;
using QuantityMeasurementAppBusinessLayer.Exceptions;
using QuantityMeasurementAppModelLayer.DTOs;

// These tests check if our unit conversions (like Feet to Inches) are accurate.
namespace QuantityMeasurementApp.Tests.Engines;

[TestClass]
public class ConversionEngineTests
{
    // ── ConvertToBase — Length ────────────────────────────────────────

    [TestMethod]
    public void ConvertToBase_1Foot_ShouldBe1()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(1, "Feet", "LENGTH")), 1e-6);

    [TestMethod]
    public void ConvertToBase_12Inches_ShouldBe1Foot()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(12, "Inch", "LENGTH")), 1e-6);

    [TestMethod]
    public void ConvertToBase_1Yard_ShouldBe3Feet()
        => Assert.AreEqual(3.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(1, "Yard", "LENGTH")), 1e-6);

    [TestMethod]
    public void ConvertToBase_3048Centimeters_ShouldBe1Foot()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(30.48, "Centimeter", "LENGTH")), 1e-4);

    // ── ConvertToBase — Weight ────────────────────────────────────────

    [TestMethod]
    public void ConvertToBase_1Kilogram_ShouldBe1()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(1, "Kilogram", "WEIGHT")), 1e-9);

    [TestMethod]
    public void ConvertToBase_1000Grams_ShouldBe1Kilogram()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(1000, "Gram", "WEIGHT")), 1e-9);

    [TestMethod]
    public void ConvertToBase_1Pound_ShouldBeCorrect()
        => Assert.AreEqual(0.453592,
            ConversionEngine.ConvertToBase(new QuantityDTO(1, "Pound", "WEIGHT")), 1e-4);

    // ── ConvertToBase — Volume ────────────────────────────────────────

    [TestMethod]
    public void ConvertToBase_1Litre_ShouldBe1()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(1, "Litre", "VOLUME")), 1e-9);

    [TestMethod]
    public void ConvertToBase_1000Millilitres_ShouldBe1Litre()
        => Assert.AreEqual(1.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(1000, "Millilitre", "VOLUME")), 1e-9);

    [TestMethod]
    public void ConvertToBase_1Gallon_ShouldBeCorrect()
        => Assert.AreEqual(3.78541,
            ConversionEngine.ConvertToBase(new QuantityDTO(1, "Gallon", "VOLUME")), 1e-4);

    // ── ConvertToBase — Temperature ───────────────────────────────────

    [TestMethod]
    public void ConvertToBase_0Celsius_ShouldBe0()
        => Assert.AreEqual(0.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(0, "Celsius", "TEMPERATURE")), 1e-9);

    [TestMethod]
    public void ConvertToBase_32Fahrenheit_ShouldBe0Celsius()
        => Assert.AreEqual(0.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(32, "Fahrenheit", "TEMPERATURE")), 1e-6);

    [TestMethod]
    public void ConvertToBase_27315Kelvin_ShouldBe0Celsius()
        => Assert.AreEqual(0.0,
            ConversionEngine.ConvertToBase(new QuantityDTO(273.15, "Kelvin", "TEMPERATURE")), 1e-4);

    // ── ConvertFromBase ───────────────────────────────────────────────

    [TestMethod]
    public void ConvertFromBase_1FootToInch_ShouldBe12()
        => Assert.AreEqual(12.0, ConversionEngine.ConvertFromBase("LENGTH", "Inch", 1.0), 1e-6);

    [TestMethod]
    public void ConvertFromBase_1KgToGram_ShouldBe1000()
        => Assert.AreEqual(1000.0, ConversionEngine.ConvertFromBase("WEIGHT", "Gram", 1.0), 1e-6);

    [TestMethod]
    public void ConvertFromBase_1LitreToMillilitre_ShouldBe1000()
        => Assert.AreEqual(1000.0, ConversionEngine.ConvertFromBase("VOLUME", "Millilitre", 1.0), 1e-6);

    [TestMethod]
    public void ConvertFromBase_0CelsiusToFahrenheit_ShouldBe32()
        => Assert.AreEqual(32.0, ConversionEngine.ConvertFromBase("TEMPERATURE", "Fahrenheit", 0.0), 1e-6);

    // ── Alias resolution ──────────────────────────────────────────────

    [TestMethod]
    public void ParseLength_FeetAliases_ShouldMatch()
        => Assert.AreEqual(ConversionEngine.ParseLength("Feet"), ConversionEngine.ParseLength("FT"));

    [TestMethod]
    public void ParseLength_InchAliases_ShouldMatch()
        => Assert.AreEqual(ConversionEngine.ParseLength("Inch"), ConversionEngine.ParseLength("IN"));

    [TestMethod]
    public void ParseWeight_KgAliases_ShouldMatch()
        => Assert.AreEqual(ConversionEngine.ParseWeight("Kilogram"), ConversionEngine.ParseWeight("KG"));

    [TestMethod]
    public void ParseVolume_LitreAliases_ShouldMatch()
        => Assert.AreEqual(ConversionEngine.ParseVolume("Litre"), ConversionEngine.ParseVolume("L"));

    // ── Error cases ───────────────────────────────────────────────────

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ConvertToBase_UnknownCategory_ShouldThrow()
        => ConversionEngine.ConvertToBase(new QuantityDTO(1, "Feet", "UNKNOWN"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ConvertToBase_UnknownLengthUnit_ShouldThrow()
        => ConversionEngine.ConvertToBase(new QuantityDTO(1, "Parsec", "LENGTH"));

    [TestMethod]
    [ExpectedException(typeof(QuantityMeasurementException))]
    public void ConvertFromBase_UnknownWeightUnit_ShouldThrow()
        => ConversionEngine.ConvertFromBase("WEIGHT", "Stone", 1.0);
}
