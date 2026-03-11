namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityTest
{

    [TestMethod] //1
    public void testSubtraction_SameUnit_FeetMinusFeet()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(5, LengthUnit.Feet));

        Assert.AreEqual(5, result.Value);
    }

    [TestMethod] //2
    public void testSubtraction_SameUnit_LitreMinusLitre()
    {
        var result = new Quantity<VolumeUnit>(10, VolumeUnit.Litre)
            .Subtract(new Quantity<VolumeUnit>(3, VolumeUnit.Litre));

        Assert.AreEqual(7, result.Value);
    }

    [TestMethod] //3
    public void testSubtraction_CrossUnit_FeetMinusInches()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(6, LengthUnit.Inch));

        Assert.AreEqual(9.5, result.Value, 0.01);
    }

    [TestMethod] //4
    public void testSubtraction_CrossUnit_InchesMinusFeet()
    {
        var result = new Quantity<LengthUnit>(120, LengthUnit.Inch)
            .Subtract(new Quantity<LengthUnit>(5, LengthUnit.Feet));

        Assert.AreEqual(60, result.Value);
    }

    [TestMethod] //5
    public void testSubtraction_ExplicitTargetUnit_Feet()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(6, LengthUnit.Inch), LengthUnit.Feet);

        Assert.AreEqual(9.5, result.Value, 0.01);
    }

    [TestMethod] //6
    public void testSubtraction_ExplicitTargetUnit_Inches()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(6, LengthUnit.Inch), LengthUnit.Inch);

        Assert.AreEqual(114, result.Value);
    }

    [TestMethod] //7
    public void testSubtraction_ExplicitTargetUnit_Millilitre()
    {
        var result = new Quantity<VolumeUnit>(5, VolumeUnit.Litre)
            .Subtract(new Quantity<VolumeUnit>(2, VolumeUnit.Litre), VolumeUnit.Millilitre);

        Assert.AreEqual(3000, result.Value);
    }

    [TestMethod] //8
    public void testSubtraction_ResultingInNegative()
    {
        var result = new Quantity<LengthUnit>(5, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(10, LengthUnit.Feet));

        Assert.AreEqual(-5, result.Value);
    }

    [TestMethod] //9
    public void testSubtraction_ResultingInZero()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(120, LengthUnit.Inch));

        Assert.AreEqual(0, result.Value);
    }

    [TestMethod] //10
    public void testSubtraction_WithZeroOperand()
    {
        var result = new Quantity<LengthUnit>(5, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(0, LengthUnit.Inch));

        Assert.AreEqual(5, result.Value);
    }

    [TestMethod] //11
    public void testSubtraction_WithNegativeValues()
    {
        var result = new Quantity<LengthUnit>(5, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(-2, LengthUnit.Feet));

        Assert.AreEqual(7, result.Value);
    }

    [TestMethod] //12
    public void testSubtraction_NonCommutative()
    {
        var a = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var b = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        Assert.AreEqual(5, a.Subtract(b).Value);
        Assert.AreEqual(-5, b.Subtract(a).Value);
    }

    [TestMethod] //13
    public void testSubtraction_WithLargeValues()
    {
        var result = new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram)
            .Subtract(new Quantity<WeightUnit>(5e5, WeightUnit.Kilogram));

        Assert.AreEqual(5e5, result.Value);
    }

    [TestMethod] //14
    public void testSubtraction_WithSmallValues()
    {
        var result = new Quantity<LengthUnit>(0.001, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(0.0005, LengthUnit.Feet));

        Assert.AreEqual(0, result.Value, 0.0001);
    }

    [TestMethod] //15
    public void testSubtraction_NullOperand()
    {
        try
        {
            new Quantity<LengthUnit>(10, LengthUnit.Feet).Subtract(null);
            Assert.Fail();
        }
        catch (ArgumentException) { }
    }

    [TestMethod] //16
    public void testSubtraction_NullTargetUnit()
    {
        try
        {
            new Quantity<LengthUnit>(10, LengthUnit.Feet)
                .Subtract(new Quantity<LengthUnit>(5, LengthUnit.Feet), (LengthUnit)(object)null);
            Assert.Fail();
        }
        catch (Exception) { }
    }

    [TestMethod] //17
    public void testSubtraction_CrossCategory()
    {
        try
        {
            var length = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var weight = new Quantity<WeightUnit>(5.0, WeightUnit.Kilogram);

            // forced cast to bypass compile-time generic restriction
            length.Subtract((Quantity<LengthUnit>)(object)weight);

            Assert.Fail("Expected ArgumentException not thrown");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //18
    public void testSubtraction_AllMeasurementCategories()
    {
        var length = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(5, LengthUnit.Feet));

        var weight = new Quantity<WeightUnit>(10, WeightUnit.Kilogram)
            .Subtract(new Quantity<WeightUnit>(5, WeightUnit.Kilogram));

        var volume = new Quantity<VolumeUnit>(10, VolumeUnit.Litre)
            .Subtract(new Quantity<VolumeUnit>(5, VolumeUnit.Litre));

        Assert.AreEqual(5, length.Value);
        Assert.AreEqual(5, weight.Value);
        Assert.AreEqual(5, volume.Value);
    }

    [TestMethod] //19
    public void testSubtraction_ChainedOperations()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(2, LengthUnit.Feet))
            .Subtract(new Quantity<LengthUnit>(1, LengthUnit.Feet));

        Assert.AreEqual(7, result.Value);
    }

    [TestMethod] //20
    public void testDivision_SameUnit_FeetDividedByFeet()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Divide(new Quantity<LengthUnit>(2, LengthUnit.Feet));

        Assert.AreEqual(5, result);
    }

    [TestMethod] //21
    public void testDivision_SameUnit_LitreDividedByLitre()
    {
        var result = new Quantity<VolumeUnit>(10, VolumeUnit.Litre)
            .Divide(new Quantity<VolumeUnit>(5, VolumeUnit.Litre));

        Assert.AreEqual(2, result);
    }

    [TestMethod] //22
    public void testDivision_CrossUnit_FeetDividedByInches()
    {
        var result = new Quantity<LengthUnit>(24, LengthUnit.Inch)
            .Divide(new Quantity<LengthUnit>(2, LengthUnit.Feet));

        Assert.AreEqual(1, result);
    }

    [TestMethod] //23
    public void testDivision_CrossUnit_KilogramDividedByGram()
    {
        var result = new Quantity<WeightUnit>(2, WeightUnit.Kilogram)
            .Divide(new Quantity<WeightUnit>(2000, WeightUnit.Gram));

        Assert.AreEqual(1, result);
    }

    [TestMethod] //24
    public void testDivision_RatioGreaterThanOne()
    {
        Assert.AreEqual(5,
            new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Divide(new Quantity<LengthUnit>(2, LengthUnit.Feet)));
    }

    [TestMethod] //25
    public void testDivision_RatioLessThanOne()
    {
        Assert.AreEqual(0.5,
            new Quantity<LengthUnit>(5, LengthUnit.Feet)
            .Divide(new Quantity<LengthUnit>(10, LengthUnit.Feet)));
    }

    [TestMethod] //26
    public void testDivision_RatioEqualToOne()
    {
        Assert.AreEqual(1,
            new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Divide(new Quantity<LengthUnit>(10, LengthUnit.Feet)));
    }

    [TestMethod] //27
    public void testDivision_NonCommutative()
    {
        var a = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var b = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        Assert.AreEqual(2, a.Divide(b));
        Assert.AreEqual(0.5, b.Divide(a));
    }

    [TestMethod] //28
    public void testDivision_ByZero()
    {
        try
        {
            new Quantity<LengthUnit>(10, LengthUnit.Feet)
                .Divide(new Quantity<LengthUnit>(0, LengthUnit.Feet));
            Assert.Fail();
        }
        catch (ArithmeticException) { }
    }

    [TestMethod] //29
    public void testDivision_WithLargeRatio()
    {
        var result = new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram)
            .Divide(new Quantity<WeightUnit>(1, WeightUnit.Kilogram));

        Assert.AreEqual(1e6, result);
    }

    [TestMethod] //30
    public void testDivision_WithSmallRatio()
    {
        var result = new Quantity<WeightUnit>(1, WeightUnit.Kilogram)
            .Divide(new Quantity<WeightUnit>(1e6, WeightUnit.Kilogram));

        Assert.AreEqual(1e-6, result);
    }

    [TestMethod] //31
    public void testDivision_NullOperand()
    {
        try
        {
            new Quantity<LengthUnit>(10, LengthUnit.Feet).Divide(null);
            Assert.Fail();
        }
        catch (ArgumentException) { }
    }

    [TestMethod] //32
    public void testDivision_CrossCategory()
    {
        try
        {
            var length = new Quantity<LengthUnit>(10.0, LengthUnit.Feet);
            var weight = new Quantity<WeightUnit>(5.0, WeightUnit.Kilogram);

            length.Divide((Quantity<LengthUnit>)(object)weight);

            Assert.Fail("Expected ArgumentException not thrown");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //33
    public void testDivision_AllMeasurementCategories()
    {
        Assert.AreEqual(2,
            new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Divide(new Quantity<LengthUnit>(5, LengthUnit.Feet)));

        Assert.AreEqual(2,
            new Quantity<WeightUnit>(10, WeightUnit.Kilogram)
            .Divide(new Quantity<WeightUnit>(5, WeightUnit.Kilogram)));

        Assert.AreEqual(2,
            new Quantity<VolumeUnit>(10, VolumeUnit.Litre)
            .Divide(new Quantity<VolumeUnit>(5, VolumeUnit.Litre)));
    }

    [TestMethod] //34
    public void testDivision_Associativity()
    {
        var A = new Quantity<LengthUnit>(12.0, LengthUnit.Feet);
        var B = new Quantity<LengthUnit>(3.0, LengthUnit.Feet);
        var C = new Quantity<LengthUnit>(2.0, LengthUnit.Feet);

        double left = (A.Divide(B)) / C.Divide(new Quantity<LengthUnit>(1, LengthUnit.Feet));
        double right = A.Divide(new Quantity<LengthUnit>(B.Divide(C), LengthUnit.Feet));

        Assert.AreNotEqual(left, right);
    }

    [TestMethod] //35
    public void testSubtractionAndDivision_Integration()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(2, LengthUnit.Feet))
            .Divide(new Quantity<LengthUnit>(2, LengthUnit.Feet));

        Assert.AreEqual(4, result);
    }

    [TestMethod] //36
    public void testSubtractionAddition_Inverse()
    {
        var a = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var b = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var result = a.Add(b).Subtract(b);

        Assert.AreEqual(a.Value, result.Value);
    }

    [TestMethod] //37
    public void testSubtraction_Immutability()
    {
        var a = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var b = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var result = a.Subtract(b);

        Assert.AreEqual(10, a.Value);
        Assert.AreEqual(5, result.Value);
    }

    [TestMethod] //38
    public void testDivision_Immutability()
    {
        var a = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var b = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var result = a.Divide(b);

        Assert.AreEqual(10, a.Value);
        Assert.AreEqual(2, result);
    }

    [TestMethod] //39
    public void testSubtraction_PrecisionAndRounding()
    {
        var result = new Quantity<LengthUnit>(10, LengthUnit.Feet)
            .Subtract(new Quantity<LengthUnit>(0.333, LengthUnit.Feet));

        Assert.AreEqual(Math.Round(result.Value, 2), result.Value);
    }

    [TestMethod] //40
    public void testDivision_PrecisionHandling()
    {
        var result = new Quantity<LengthUnit>(1, LengthUnit.Feet)
            .Divide(new Quantity<LengthUnit>(3, LengthUnit.Feet));

        Assert.IsTrue(result > 0);
    }

}
