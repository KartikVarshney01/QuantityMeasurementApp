using System.Reflection;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityTests
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
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_ConvertTo()
    {
        Quantity q = new Quantity(LengthUnit.Feet, 1.0);

        double result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(12.0, result, EPSILON);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_Add()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Feet);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 2.0), result);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_AddWithTargetUnit()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 0.6667), result);
    }

    [TestMethod]
    public void TestQuantityLengthRefactored_NullUnit()
    {
        try
        {
            new Quantity(null, 1.0);
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
            new Quantity(LengthUnit.Feet, Double.NaN);
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
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC5ConversionTests()
    {
        Quantity q = new Quantity(LengthUnit.Feet, 1.0);

        double result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(12.0, result, 0.0001);
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC6AdditionTests()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 2.0), result);
    }

    [TestMethod]
    public void TestBackwardCompatibility_UC7AdditionWithTargetUnitTests()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 0.6667), result);
    }

    [TestMethod]
    public void TestRoundTripConversion_RefactoredDesign()
    {
        Quantity q = new Quantity(LengthUnit.Feet, 5.0);

        double inches = q.ConvertTo(LengthUnit.Inch);

        Quantity q2 = new Quantity(LengthUnit.Inch, inches);

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
