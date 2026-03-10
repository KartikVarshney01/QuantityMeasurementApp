namespace QuantityMeasurementApp.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public sealed class QuantityTests
{
    [TestMethod]
    public void TestAddition_SameUnit_FeetPlusFeet()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, 2.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 3.0), result);
    }

    [TestMethod]
    public void TestAddition_SameUnit_InchPlusInch()
    {
        Quantity q1 = new Quantity(LengthUnit.Inch, 6.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 6.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Inch, 12.0), result);
    }

    [TestMethod]
    public void TestAddition_CrossUnit_FeetPlusInches()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 2.0), result);
    }

    [TestMethod]
    public void TestAddition_CrossUnit_InchPlusFeet()
    {
        Quantity q1 = new Quantity(LengthUnit.Inch, 12.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, 1.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Inch, 24.0), result);
    }

    [TestMethod]
    public void TestAddition_CrossUnit_YardPlusFeet()
    {
        Quantity q1 = new Quantity(LengthUnit.Yard, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, 3.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Yard, 2.0), result);
    }

    [TestMethod]
    public void TestAddition_CrossUnit_CentimeterPlusInch()
    {
        Quantity q1 = new Quantity(LengthUnit.Centimeter, 2.54);
        Quantity q2 = new Quantity(LengthUnit.Inch, 1.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Centimeter, 5.08), result);
    }

    [TestMethod]
    public void TestAddition_Commutativity()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 12.0);

        Quantity result1 = Quantity.Add(q1, q2);
        Quantity result2 = Quantity.Add(q2, q1);

        Assert.AreEqual(result1, result2);
    }

    [TestMethod]
    public void TestAddition_WithZero()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 5.0);
        Quantity q2 = new Quantity(LengthUnit.Inch, 0.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 5.0), result);
    }

    [TestMethod]
    public void TestAddition_NegativeValues()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 5.0);
        Quantity q2 = new Quantity(LengthUnit.Feet, -2.0);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 3.0), result);
    }

    [TestMethod]
    public void TestAddition_NullSecondOperand()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1.0);

        try
        {
            Quantity.Add(q1, null);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void TestAddition_LargeValues()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 1e6);
        Quantity q2 = new Quantity(LengthUnit.Feet, 1e6);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 2e6), result);
    }

    [TestMethod]
    public void TestAddition_SmallValues()
    {
        Quantity q1 = new Quantity(LengthUnit.Feet, 0.001);
        Quantity q2 = new Quantity(LengthUnit.Feet, 0.002);

        Quantity result = Quantity.Add(q1, q2);

        Assert.AreEqual(new Quantity(LengthUnit.Feet, 0.003), result);
    }
}
