namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityTests
{
    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_Feet()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Feet);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 2.0), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_Inches()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Inch);

        Assert.AreEqual(new Quantity(LengthUnit.Inch, 24.0), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_Yards()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 0.6667), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_Centimeters()
    {
        Quantity q1 = new Quantity(LengthUnit.Inch, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 1.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Centimeter);

        Assert.AreEqual(new Quantity(LengthUnit.Centimeter, 5.08), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_SameAsFirstOperand()
    {
        Quantity q1 = new Quantity(LengthUnit.Yard, 2.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, 3.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 3.0), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_SameAsSecondOperand()
    {
        Quantity q1 = new Quantity(LengthUnit.Yard, 2.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, 3.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Feet);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 9.0), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_Commutativity()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result1 = Quantity.Add(q1, q2, LengthUnit.Yard);
        Quantity result2 = Quantity.Add(q2, q1, LengthUnit.Yard);

        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_WithZero()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 5.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 0.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 1.6667), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_NegativeValues()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 5.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, -2.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Inch);

        Assert.AreEqual(new Quantity(LengthUnit.Inch, 36.0), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_NullTargetUnit()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        try
        {
            Quantity.Add(q1, q2, null);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_LargeToSmallScale()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1000.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, 500.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Inch);

        Assert.AreEqual(new Quantity(LengthUnit.Inch, 18000.0), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_SmallToLargeScale()
    {
        Quantity q1 = new Quantity(LengthUnit.Inch, 12.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Yard);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 0.6667), result);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_AllUnitCombinations()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity r1 = Quantity.Add(q1, q2, LengthUnit.Feet);
        Quantity r2 = Quantity.Add(q1, q2, LengthUnit.Inch);
        Quantity r3 = Quantity.Add(q1, q2, LengthUnit.Yard);
        Quantity r4 = Quantity.Add(q1, q2, LengthUnit.Centimeter);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 2.0), r1);
        Assert.AreEqual(new Quantity(LengthUnit.Inch, 24.0), r2);
        Assert.AreEqual(new Quantity(LengthUnit.Yard, 0.6667), r3);
        Assert.AreEqual(new Quantity(LengthUnit.Centimeter, 60.96), r4);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_PrecisionTolerance()
    {
        Quantity q1 = new Quantity(LengthUnit.Centimeter, 2.54);
        Quantity q2 = new Quantity(LengthUnit.Inch, 1.0);

        Quantity result = Quantity.Add(q1, q2, LengthUnit.Centimeter);

        Assert.AreEqual(new Quantity(LengthUnit.Centimeter, 5.08), result);
    }
}
