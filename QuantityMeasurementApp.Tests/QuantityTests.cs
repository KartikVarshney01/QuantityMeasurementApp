using System.Reflection;
using System.Runtime.CompilerServices;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityTests
{
    private const double EPSILON = 0.0001;

    [TestMethod]
    public void testConversion_FeetToInches()
    {
        double result = Quantity.Convert(1.0, LengthUnit.Feet, LengthUnit.Inch);
        Assert.AreEqual(12.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_InchesToFeet()
    {
        double result = Quantity.Convert(24.0, LengthUnit.Inch, LengthUnit.Feet);
        Assert.AreEqual(2.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_YardsToInches()
    {
        double result = Quantity.Convert(1.0, LengthUnit.Yard, LengthUnit.Inch);
        Assert.AreEqual(36.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_InchesToYards()
    {
        double result = Quantity.Convert(72.0, LengthUnit.Inch, LengthUnit.Yard);
        Assert.AreEqual(2.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_CentimetersToInches()
    {
        double result = Quantity.Convert(2.54, LengthUnit.Centimeter, LengthUnit.Inch);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_FeetToYard()
    {
        double result = Quantity.Convert(6.0, LengthUnit.Feet, LengthUnit.Yard);
        Assert.AreEqual(2.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_RoundTrip_PreservesValue()
    {
        double value = 5.0;

        double converted = Quantity.Convert(value, LengthUnit.Feet, LengthUnit.Inch);
        double result = Quantity.Convert(converted, LengthUnit.Inch, LengthUnit.Feet);

        Assert.AreEqual(value, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_ZeroValue()
    {
        double result = Quantity.Convert(0.0, LengthUnit.Feet, LengthUnit.Inch);
        Assert.AreEqual(0.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_NegativeValue()
    {
        double result = Quantity.Convert(-1.0, LengthUnit.Feet, LengthUnit.Inch);
        Assert.AreEqual(-12.0, result, EPSILON);
    }

    [TestMethod]
    public void testConversion_InvalidUnit_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Quantity.Convert(1.0, (LengthUnit)999, LengthUnit.Feet);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            Quantity.Convert(1.0, LengthUnit.Feet, (LengthUnit)999);
        });
    }

    [TestMethod]
    public void testConversion_NaNOrInfinite_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
        {
            Quantity.Convert(double.NaN, LengthUnit.Feet, LengthUnit.Inch);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            Quantity.Convert(double.PositiveInfinity, LengthUnit.Feet, LengthUnit.Inch);
        });

        Assert.Throws<ArgumentException>(() =>
        {
            Quantity.Convert(double.NegativeInfinity, LengthUnit.Feet, LengthUnit.Inch);
        });
    }

    [TestMethod]
    public void testConversion_PrecisionTolerance()
    {
        double result = Quantity.Convert(1.0, LengthUnit.Centimeter, LengthUnit.Inch);

        Assert.IsTrue(Math.Abs(result - 0.393701) < EPSILON);
    }
}
