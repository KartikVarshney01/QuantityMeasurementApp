namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class Test1
{
    const double EPS = 0.0001;

    [TestMethod]
    public void TestEquality_KilogramToKilogram_SameValue()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Kilogram, 1.0)
            .Equals(new QuantityWeight(WeightUnit.Kilogram, 1.0)));
    }

    [TestMethod]
    public void TestEquality_KilogramToKilogram_DifferentValue()
    {
        Assert.IsFalse(
            new QuantityWeight(WeightUnit.Kilogram, 1.0)
            .Equals(new QuantityWeight(WeightUnit.Kilogram, 2.0)));
    }

    [TestMethod]
    public void TestEquality_KilogramToGram_EquivalentValue()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Kilogram, 1.0)
            .Equals(new QuantityWeight(WeightUnit.Gram, 1000.0)));
    }

    [TestMethod]
    public void TestEquality_GramToKilogram_EquivalentValue()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Gram, 1000.0)
            .Equals(new QuantityWeight(WeightUnit.Kilogram, 1.0)));
    }

    [TestMethod]
    public void TestEquality_WeightVsLength_Incompatible()
    {
        QuantityWeight w = new QuantityWeight(WeightUnit.Kilogram, 1.0);
        QuantityLength l = new QuantityLength(LengthUnit.Feet, 1.0);

        Assert.IsFalse(w.Equals(l));
    }

    [TestMethod]
    public void TestEquality_NullComparison()
    {
        QuantityWeight w = new QuantityWeight(WeightUnit.Kilogram, 1.0);

        Assert.IsFalse(w.Equals(null));
    }

    [TestMethod]
    public void TestEquality_SameReference()
    {
        QuantityWeight w = new QuantityWeight(WeightUnit.Kilogram, 1.0);

        Assert.IsTrue(w.Equals(w));
    }

    [TestMethod]
    public void TestEquality_NullUnit()
    {
        try
        {
            new QuantityWeight(null, 1.0);
            Assert.Fail("Expected ArgumentException was not thrown");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod]
    public void TestEquality_TransitiveProperty()
    {
        QuantityWeight a = new QuantityWeight(WeightUnit.Kilogram, 1.0);
        QuantityWeight b = new QuantityWeight(WeightUnit.Gram, 1000.0);
        QuantityWeight c = new QuantityWeight(WeightUnit.Kilogram, 1.0);

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(b.Equals(c));
        Assert.IsTrue(a.Equals(c));
    }

    [TestMethod]
    public void TestEquality_ZeroValue()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Kilogram, 0.0)
            .Equals(new QuantityWeight(WeightUnit.Gram, 0.0)));
    }

    [TestMethod]
    public void TestEquality_NegativeWeight()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Kilogram, -1.0)
            .Equals(new QuantityWeight(WeightUnit.Gram, -1000.0)));
    }

    [TestMethod]
    public void TestEquality_LargeWeightValue()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Gram, 1000000.0)
            .Equals(new QuantityWeight(WeightUnit.Kilogram, 1000.0)));
    }

    [TestMethod]
    public void TestEquality_SmallWeightValue()
    {
        Assert.IsTrue(
            new QuantityWeight(WeightUnit.Kilogram, 0.001)
            .Equals(new QuantityWeight(WeightUnit.Gram, 1.0)));
    }

    [TestMethod]
    public void TestConversion_PoundToKilogram()
    {
        QuantityWeight result =
            new QuantityWeight(WeightUnit.Pound, 2.20462)
            .ConvertTo(WeightUnit.Kilogram);

        Assert.AreEqual(1.0, result.Value, EPS);
    }

    [TestMethod]
    public void TestConversion_KilogramToPound()
    {
        QuantityWeight result =
            new QuantityWeight(WeightUnit.Kilogram, 1.0)
            .ConvertTo(WeightUnit.Pound);

        Assert.AreEqual(2.20462, result.Value, EPS);
    }

    [TestMethod]
    public void TestConversion_SameUnit()
    {
        QuantityWeight result =
            new QuantityWeight(WeightUnit.Kilogram, 5.0)
            .ConvertTo(WeightUnit.Kilogram);

        Assert.AreEqual(5.0, result.Value, EPS);
    }

    [TestMethod]
    public void TestConversion_ZeroValue()
    {
        QuantityWeight result =
            new QuantityWeight(WeightUnit.Kilogram, 0.0)
            .ConvertTo(WeightUnit.Gram);

        Assert.AreEqual(0.0, result.Value, EPS);
    }

    [TestMethod]
    public void TestConversion_NegativeValue()
    {
        QuantityWeight result =
            new QuantityWeight(WeightUnit.Kilogram, -1.0)
            .ConvertTo(WeightUnit.Gram);

        Assert.AreEqual(-1000.0, result.Value, EPS);
    }

    [TestMethod]
    public void TestConversion_RoundTrip()
    {
        QuantityWeight q =
            new QuantityWeight(WeightUnit.Kilogram, 1.5)
            .ConvertTo(WeightUnit.Gram)
            .ConvertTo(WeightUnit.Kilogram);

        Assert.AreEqual(1.5, q.Value, EPS);
    }

    [TestMethod]
    public void TestAddition_SameUnit_KilogramPlusKilogram()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 1.0),
                new QuantityWeight(WeightUnit.Kilogram, 2.0));

        Assert.AreEqual(new QuantityWeight(WeightUnit.Kilogram, 3.0), result);
    }

    [TestMethod]
    public void TestAddition_CrossUnit_KilogramPlusGram()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 1.0),
                new QuantityWeight(WeightUnit.Gram, 1000.0));

        Assert.AreEqual(new QuantityWeight(WeightUnit.Kilogram, 2.0), result);
    }

    [TestMethod]
    public void TestLengthUnitEnum_InchesConstant()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 1.0),
                new QuantityWeight(WeightUnit.Gram, 1000.0)
            );

        Assert.AreEqual(new QuantityWeight(WeightUnit.Kilogram, 2.0), result);
    }

    [TestMethod]
    public void TestAddition_CrossUnit_PoundPlusKilogram()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Pound, 2.20462),
                new QuantityWeight(WeightUnit.Kilogram, 1.0));

        Assert.AreEqual(4.40924, result.Value, EPS);
    }

    [TestMethod]
    public void TestAddition_ExplicitTargetUnit_Kilogram()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 1.0),
                new QuantityWeight(WeightUnit.Gram, 1000.0),
                WeightUnit.Gram);

        Assert.AreEqual(new QuantityWeight(WeightUnit.Gram, 2000.0), result);
    }

    [TestMethod]
    public void TestAddition_Commutativity()
    {
        QuantityWeight a = new QuantityWeight(WeightUnit.Kilogram, 1.0);
        QuantityWeight b = new QuantityWeight(WeightUnit.Gram, 1000.0);

        QuantityWeight r1 = QuantityWeight.Add(a, b);
        QuantityWeight r2 = QuantityWeight.Add(b, a);

        Assert.IsTrue(r1.Equals(r2));
    }

    [TestMethod]
    public void TestAddition_WithZero()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 5.0),
                new QuantityWeight(WeightUnit.Gram, 0.0));

        Assert.AreEqual(new QuantityWeight(WeightUnit.Kilogram, 5.0), result);
    }

    [TestMethod]
    public void TestAddition_NegativeValues()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 5.0),
                new QuantityWeight(WeightUnit.Gram, -2000.0));

        Assert.AreEqual(new QuantityWeight(WeightUnit.Kilogram, 3.0), result);
    }

    [TestMethod]
    public void TestAddition_LargeValues()
    {
        QuantityWeight result =
            QuantityWeight.Add(
                new QuantityWeight(WeightUnit.Kilogram, 1e6),
                new QuantityWeight(WeightUnit.Kilogram, 1e6));

        Assert.AreEqual(new QuantityWeight(WeightUnit.Kilogram, 2e6), result);
    }
}
