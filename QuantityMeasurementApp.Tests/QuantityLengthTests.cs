using System.Reflection;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityLengthTests
{
    private const double EPSILON = 0.0001;

    [TestMethod]
    public void TestLengthUnitEnum_FeetConstant()
    {
        double result = LengthUnit.Feet.ConvertToBaseUnit(1.0);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void TestLengthUnitEnum_InchesConstant()
    {
        double result = LengthUnit.Inch.ConvertToBaseUnit(12.0);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void TestLengthUnitEnum_YardsConstant()
    {
        double result = LengthUnit.Yard.ConvertToBaseUnit(1.0);
        Assert.AreEqual(3.0, result, EPSILON);
    }

    [TestMethod]
    public void TestLengthUnitEnum_CentimetersConstant()
    {
        double result = LengthUnit.Centimeter.ConvertToBaseUnit(30.48);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertToBaseUnit_FeetToFeet()
    {
        double result = LengthUnit.Feet.ConvertToBaseUnit(5.0);
        Assert.AreEqual(5.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertToBaseUnit_InchesToFeet()
    {
        double result = LengthUnit.Inch.ConvertToBaseUnit(12.0);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertToBaseUnit_YardsToFeet()
    {
        double result = LengthUnit.Yard.ConvertToBaseUnit(2.0);
        Assert.AreEqual(6.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertToBaseUnit_CentimetersToFeet()
    {
        double result = LengthUnit.Centimeter.ConvertToBaseUnit(30.48);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertFromBaseUnit_FeetToFeet()
    {
        double result = LengthUnit.Feet.ConvertFromBaseUnit(2.0);
        Assert.AreEqual(2.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertFromBaseUnit_FeetToInches()
    {
        double result = LengthUnit.Inch.ConvertFromBaseUnit(1.0);
        Assert.AreEqual(12.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertFromBaseUnit_FeetToYards()
    {
        double result = LengthUnit.Yard.ConvertFromBaseUnit(3.0);
        Assert.AreEqual(1.0, result, EPSILON);
    }

    [TestMethod]
    public void TestConvertFromBaseUnit_FeetToCentimeters()
    {
        double result = LengthUnit.Centimeter.ConvertFromBaseUnit(1.0);
        Assert.AreEqual(30.48, result, EPSILON);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_Equality()
    {
        QuantityLength q1 = new QuantityLength(LengthUnit.Feet, 1.0);
        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, 12.0);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_ConvertTo()
    {
        QuantityLength q = new QuantityLength(LengthUnit.Feet, 1.0);

        double result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(12.0, result, EPSILON);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_Add()
    {
        QuantityLength q1 = new QuantityLength(LengthUnit.Feet, 1.0);
        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, 12.0);

        QuantityLength result = QuantityLength.Add(q1, q2, LengthUnit.Feet);

        Assert.AreEqual(new QuantityLength(LengthUnit.Feet, 2.0), result);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_AddWithTargetUnit()
    {
        QuantityLength q1 = new QuantityLength(LengthUnit.Feet, 1.0);
        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, 12.0);

        QuantityLength result = QuantityLength.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new QuantityLength(LengthUnit.Yard, 0.6667), result);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_NullUnit()
    {
        try
        {
            new QuantityLength(null, 1.0);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_InvalidValue()
    {
        try
        {
            new QuantityLength(LengthUnit.Feet, Double.NaN);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC1EqualityTests()
    {
        QuantityLength q1 = new QuantityLength(LengthUnit.Feet, 1.0);
        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, 12.0);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC5ConversionTests()
    {
        QuantityLength q = new QuantityLength(LengthUnit.Feet, 1.0);

        double result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(12.0, result, 0.0001);
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC6AdditionTests()
    {
        QuantityLength q1 = new QuantityLength(LengthUnit.Feet, 1.0);
        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, 12.0);

        QuantityLength result = QuantityLength.Add(q1, q2);

        Assert.AreEqual(new QuantityLength(LengthUnit.Feet, 2.0), result);
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC7AdditionWithTargetUnitTests()
    {
        QuantityLength q1 = new QuantityLength(LengthUnit.Feet, 1.0);
        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, 12.0);

        QuantityLength result = QuantityLength.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new QuantityLength(LengthUnit.Yard, 0.6667), result);
    }

    [TestMethod]
    public void TestRoundTripConversion_RefactoredDesign()
    {
        QuantityLength q = new QuantityLength(LengthUnit.Feet, 5.0);

        double inches = q.ConvertTo(LengthUnit.Inch);

        QuantityLength q2 = new QuantityLength(LengthUnit.Inch, inches);

        double result = q2.ConvertTo(LengthUnit.Feet);

        Assert.AreEqual(5.0, result, EPSILON);
    }

    [TestMethod]
    public void TestUnitImmutability()
    {
        LengthUnit unit = LengthUnit.Feet;

        Assert.AreEqual(LengthUnit.Feet, unit);
    }

    [TestMethod]
    public void testArchitectureScalability_MultipleCategories()
    {
        Assert.IsTrue(Enum.IsDefined(typeof(LengthUnit), LengthUnit.Feet));
        Assert.IsTrue(Enum.IsDefined(typeof(LengthUnit), LengthUnit.Inch));
    }
}
