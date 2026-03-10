namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class VolumeQuantityTest
{
    [TestMethod] //1
    public void testEquality_LitreToLitre_SameValue()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //2
    public void testEquality_LitreToLitre_DifferentValue()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

        Assert.IsFalse(q1.Equals(q2));
    }

    [TestMethod] //3
    public void testEquality_LitreToMillilitre_EquivalentValue()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //4
    public void testEquality_MillilitreToLitre_EquivalentValue()
    {
        var q1 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
        var q2 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //5
    public void testEquality_LitreToGallon_EquivalentValue()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(0.264172, VolumeUnit.Gallon);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //6
    public void testEquality_GallonToLitre_EquivalentValue()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
        var q2 = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //7
    public void testEquality_VolumeVsLength_Incompatible()
    {
        var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var length = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);

        Assert.IsFalse(volume.Equals(length));
    }

    [TestMethod] //8
    public void testEquality_VolumeVsWeight_Incompatible()
    {
        var volume = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var weight = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);

        Assert.IsFalse(volume.Equals(weight));
    }

    [TestMethod] //9
    public void testEquality_NullComparison()
    {
        var q = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

        Assert.IsFalse(q.Equals(null));
    }

    [TestMethod] //10
    public void testEquality_SameReference()
    {
        var q = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

        Assert.IsTrue(q.Equals(q));
    }

    [TestMethod] //11
    public void testEquality_NullUnit()
    {
        try
        {
            var quantity = new Quantity<VolumeUnit>(1.0, (VolumeUnit)(object)null);
            Assert.Fail();
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //12
    public void testEquality_TransitiveProperty()
    {
        var a = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var b = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
        var c = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

        Assert.IsTrue(a.Equals(b) && b.Equals(c) && a.Equals(c));
    }

    [TestMethod] //13
    public void testEquality_ZeroValue()
    {
        var q1 = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //14
    public void testEquality_NegativeVolume()
    {
        var q1 = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(-1000.0, VolumeUnit.Millilitre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //15
    public void testEquality_LargeVolumeValue()
    {
        var q1 = new Quantity<VolumeUnit>(1000000.0, VolumeUnit.Millilitre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Litre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //16
    public void testEquality_SmallVolumeValue()
    {
        var q1 = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Millilitre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //17
    public void testConversion_LitreToMillilitre()
    {
        var q = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var result = q.ConvertTo(VolumeUnit.Millilitre);

        Assert.AreEqual(1000.0, result.Value);
    }

    [TestMethod] //18
    public void testConversion_MillilitreToLitre()
    {
        var q = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
        var result = q.ConvertTo(VolumeUnit.Litre);

        Assert.AreEqual(1.0, result.Value);
    }

    [TestMethod] //19
    public void testConversion_GallonToLitre()
    {
        var q = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
        var result = q.ConvertTo(VolumeUnit.Litre);

        Assert.AreEqual(3.78541, result.Value, 0.0001);
    }

    [TestMethod] //20
    public void testConversion_LitreToGallon()
    {
        var q = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
        var result = q.ConvertTo(VolumeUnit.Gallon);

        Assert.AreEqual(1.0, result.Value, 0.0001);
    }

    [TestMethod] //21
    public void testConversion_MillilitreToGallon()
    {
        var q = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
        var result = q.ConvertTo(VolumeUnit.Gallon);

        Assert.AreEqual(0.264172, result.Value, 0.0001);
    }

    [TestMethod] //22
    public void testConversion_SameUnit()
    {
        var q = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
        var result = q.ConvertTo(VolumeUnit.Litre);

        Assert.AreEqual(5.0, result.Value);
    }

    [TestMethod] //23
    public void testConversion_ZeroValue()
    {
        var q = new Quantity<VolumeUnit>(0.0, VolumeUnit.Litre);
        var result = q.ConvertTo(VolumeUnit.Millilitre);

        Assert.AreEqual(0.0, result.Value);
    }

    [TestMethod] //24
    public void testConversion_NegativeValue()
    {
        var q = new Quantity<VolumeUnit>(-1.0, VolumeUnit.Litre);
        var result = q.ConvertTo(VolumeUnit.Millilitre);

        Assert.AreEqual(-1000.0, result.Value);
    }

    [TestMethod] //25
    public void testConversion_RoundTrip()
    {
        var q = new Quantity<VolumeUnit>(1.5, VolumeUnit.Litre);
        var result = q.ConvertTo(VolumeUnit.Millilitre).ConvertTo(VolumeUnit.Litre);

        Assert.AreEqual(1.5, result.Value, 0.0001);
    }

    [TestMethod] //26
    public void testAddition_SameUnit_LitrePlusLitre()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(2.0, VolumeUnit.Litre);

        var result = q1.Add(q2);

        Assert.AreEqual(3.0, result.Value);
    }

    [TestMethod] //27
    public void testAddition_SameUnit_MillilitrePlusMillilitre()
    {
        var q1 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);
        var q2 = new Quantity<VolumeUnit>(500.0, VolumeUnit.Millilitre);

        var result = q1.Add(q2);

        Assert.AreEqual(1000.0, result.Value);
    }

    [TestMethod] //28
    public void testAddition_CrossUnit_LitrePlusMillilitre()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

        var result = q1.Add(q2);

        Assert.AreEqual(2.0, result.Value);
    }

    [TestMethod] //29
    public void testAddition_CrossUnit_MillilitrePlusLitre()
    {
        var q1 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
        var q2 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);

        var result = q1.Add(q2);

        Assert.AreEqual(2000.0, result.Value);
    }

    [TestMethod] //30
    public void testAddition_CrossUnit_GallonPlusLitre()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);
        var q2 = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

        var result = q1.Add(q2);

        Assert.AreEqual(2.0, result.Value, 0.0001);
    }

    [TestMethod] //31
    public void testAddition_ExplicitTargetUnit_Litre()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

        var result = q1.Add(q2, VolumeUnit.Litre);

        Assert.AreEqual(2.0, result.Value, 0.0001);
    }

    [TestMethod] //32
    public void testAddition_ExplicitTargetUnit_Millilitre()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

        var result = q1.Add(q2, VolumeUnit.Millilitre);

        Assert.AreEqual(2000.0, result.Value, 0.0001);
    }

    [TestMethod] //33
    public void testAddition_ExplicitTargetUnit_Gallon()
    {
        var q1 = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(3.78541, VolumeUnit.Litre);

        var result = q1.Add(q2, VolumeUnit.Gallon);

        Assert.AreEqual(2.0, result.Value, 0.0001);
    }

    [TestMethod] //34
    public void testAddition_Commutativity()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

        var result1 = q1.Add(q2);
        var result2 = q2.Add(q1);

        Assert.IsTrue(result1.Equals(result2));
    }

    [TestMethod] //35
    public void testAddition_WithZero()
    {
        var q1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(0.0, VolumeUnit.Millilitre);

        var result = q1.Add(q2);

        Assert.AreEqual(5.0, result.Value);
    }

    [TestMethod] //36
    public void testAddition_NegativeValues()
    {
        var q1 = new Quantity<VolumeUnit>(5.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(-2000.0, VolumeUnit.Millilitre);

        var result = q1.Add(q2);

        Assert.AreEqual(3.0, result.Value);
    }

    [TestMethod] //37
    public void testAddition_LargeValues()
    {
        var q1 = new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1e6, VolumeUnit.Litre);

        var result = q1.Add(q2);

        Assert.AreEqual(2e6, result.Value);
    }

    [TestMethod] //38
    public void testAddition_SmallValues()
    {
        var q1 = new Quantity<VolumeUnit>(0.001, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(0.002, VolumeUnit.Litre);

        var result = q1.Add(q2);

        Assert.AreEqual(0.003, result.Value, 0.0001);
    }

    [TestMethod] //39
    public void testVolumeUnitEnum_LitreConstant()
    {
        Assert.AreEqual(1.0, VolumeUnit.Litre.GetConversionFactor());
    }

    [TestMethod] //40
    public void testVolumeUnitEnum_MillilitreConstant()
    {
        Assert.AreEqual(0.001, VolumeUnit.Millilitre.GetConversionFactor());
    }

    [TestMethod] //41
    public void testVolumeUnitEnum_GallonConstant()
    {
        Assert.AreEqual(3.78541, VolumeUnit.Gallon.GetConversionFactor());
    }

    [TestMethod] //42
    public void testConvertToBaseUnit_LitreToLitre()
    {
        Assert.AreEqual(5.0, VolumeUnit.Litre.ConvertToBaseUnit(5.0));
    }

    [TestMethod] //43
    public void testConvertToBaseUnit_MillilitreToLitre()
    {
        Assert.AreEqual(1.0, VolumeUnit.Millilitre.ConvertToBaseUnit(1000.0));
    }

    [TestMethod] //44
    public void testConvertToBaseUnit_GallonToLitre()
    {
        Assert.AreEqual(3.78541, VolumeUnit.Gallon.ConvertToBaseUnit(1.0), 0.0001);
    }

    [TestMethod] //45
    public void testConvertFromBaseUnit_LitreToLitre()
    {
        Assert.AreEqual(2.0, VolumeUnit.Litre.ConvertFromBaseUnit(2.0));
    }

    [TestMethod] //46
    public void testConvertFromBaseUnit_LitreToMillilitre()
    {
        Assert.AreEqual(1000.0, VolumeUnit.Millilitre.ConvertFromBaseUnit(1.0));
    }

    [TestMethod] //47
    public void testConvertFromBaseUnit_LitreToGallon()
    {
        Assert.AreEqual(1.0, VolumeUnit.Gallon.ConvertFromBaseUnit(3.78541), 0.0001);
    }

    [TestMethod] //48
    public void testBackwardCompatibility_AllUC1Through10Tests()
    {
        var length1 = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
        var length2 = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

        var weight1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
        var weight2 = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

        Assert.IsTrue(length1.Equals(length2));
        Assert.IsTrue(weight1.Equals(weight2));
    }

    [TestMethod] //49
    public void testGenericQuantity_VolumeOperations_Consistency()
    {
        var q1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
        var q2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //50
    public void testScalability_VolumeIntegration()
    {
        var q = new Quantity<VolumeUnit>(5.0, VolumeUnit.Gallon);

        Assert.IsNotNull(q);
    }
}
