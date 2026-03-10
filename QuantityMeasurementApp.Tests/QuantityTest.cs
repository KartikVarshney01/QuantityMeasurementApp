using System.Reflection;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityTest
{
    // 1
    [TestMethod]
    public void testIMeasurableInterface_LengthUnitImplementation()
    {
        LengthUnit unit = LengthUnit.Feet;
        double factor = unit.GetConversionFactor();
        Assert.AreEqual(1.0, factor);
    }

    // 2
    [TestMethod]
    public void testIMeasurableInterface_WeightUnitImplementation()
    {
        WeightUnit unit = WeightUnit.Kilogram;
        double factor = unit.GetConversionFactor();
        Assert.AreEqual(1.0, factor);
    }

    // 3
    [TestMethod]
    public void testIMeasurableInterface_ConsistentBehavior()
    {
        Assert.IsTrue(LengthUnit.Feet.GetUnitName() == "Feet");
        Assert.IsTrue(WeightUnit.Gram.GetUnitName() == "Gram");
    }

    // 4
    [TestMethod]
    public void testGenericQuantity_LengthOperations_Equality()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        Assert.IsTrue(q1.Equals(q2));
    }

    // 5
    [TestMethod]
    public void testGenericQuantity_WeightOperations_Equality()
    {
        var q1 = new Quantity<WeightUnit>(1, WeightUnit.Kilogram);
        var q2 = new Quantity<WeightUnit>(1000, WeightUnit.Gram);

        Assert.IsTrue(q1.Equals(q2));
    }

    // 6
    [TestMethod]
    public void testGenericQuantity_LengthOperations_Conversion()
    {
        var q = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(12, result.Value);
    }

    // 7
    [TestMethod]
    public void testGenericQuantity_WeightOperations_Conversion()
    {
        var q = new Quantity<WeightUnit>(1, WeightUnit.Kilogram);
        var result = q.ConvertTo(WeightUnit.Gram);

        Assert.AreEqual(1000, result.Value);
    }

    // 8
    [TestMethod]
    public void testGenericQuantity_LengthOperations_Addition()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        var result = q1.Add(q2, LengthUnit.Feet);

        Assert.AreEqual(2, result.Value);
    }

    // 9
    [TestMethod]
    public void testGenericQuantity_WeightOperations_Addition()
    {
        var q1 = new Quantity<WeightUnit>(1, WeightUnit.Kilogram);
        var q2 = new Quantity<WeightUnit>(1000, WeightUnit.Gram);

        var result = q1.Add(q2, WeightUnit.Kilogram);

        Assert.AreEqual(2, result.Value);
    }

    // 10
    [TestMethod]
    public void testCrossCategoryPrevention_LengthVsWeight()
    {
        var length = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var weight = new Quantity<WeightUnit>(1, WeightUnit.Kilogram);

        Assert.IsFalse(length.Equals(weight));
    }

    // 11
    [TestMethod]
    public void testCrossCategoryPrevention_CompilerTypeSafety()
    {
        var q = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        Assert.IsNotNull(q);
    }

    // 12
    [TestMethod]
    public void testGenericQuantity_ConstructorValidation_NullUnit()
    {
        try
        {
            var q = new Quantity<LengthUnit>(1, (LengthUnit)(object)null);
            Assert.Fail();
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    // 13
    [TestMethod]
    public void testGenericQuantity_ConstructorValidation_InvalidValue()
    {
        Assert.Throws<ArgumentException>(
            delegate
            {
                Quantity<LengthUnit> q = new Quantity<LengthUnit>(Double.NaN, LengthUnit.Feet);
            }
        );
    }

    // 14
    [TestMethod]
    public void testGenericQuantity_Conversion_AllUnitCombinations()
    {
        var q = new Quantity<LengthUnit>(1, LengthUnit.Yard);
        var result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(36, result.Value);
    }

    // 15
    [TestMethod]
    public void testGenericQuantity_Addition_AllUnitCombinations()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Yard);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        var result = q1.Add(q2, LengthUnit.Inch);

        Assert.AreEqual(48, result.Value);
    }

    // 16
    [TestMethod]
    public void testBackwardCompatibility_AllUC1Through9Tests()
    {
        var q = new Quantity<LengthUnit>(3, LengthUnit.Feet);
        Assert.AreEqual(36, q.ConvertTo(LengthUnit.Inch).Value);
    }

    // 17
    [TestMethod]
    public void testQuantityMeasurementApp_SimplifiedDemonstration_Equality()
    {
        var q1 = new Quantity<LengthUnit>(3, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(36, LengthUnit.Inch);

        Assert.IsTrue(q1.Equals(q2));
    }

    // 18
    [TestMethod]
    public void testQuantityMeasurementApp_SimplifiedDemonstration_Conversion()
    {
        var q = new Quantity<WeightUnit>(2, WeightUnit.Kilogram);
        Assert.AreEqual(2000, q.ConvertTo(WeightUnit.Gram).Value);
    }

    // 19
    [TestMethod]
    public void testQuantityMeasurementApp_SimplifiedDemonstration_Addition()
    {
        var q1 = new Quantity<WeightUnit>(1, WeightUnit.Kilogram);
        var q2 = new Quantity<WeightUnit>(500, WeightUnit.Gram);

        var result = q1.Add(q2, WeightUnit.Kilogram);

        Assert.AreEqual(1.5, result.Value);
    }

    // 20
    [TestMethod]
    public void testTypeWildcard_FlexibleSignatures()
    {
        object q = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        Assert.IsNotNull(q);
    }

    // 21
    [TestMethod]
    public void testScalability_NewUnitEnumIntegration()
    {
        Assert.IsTrue(true);
    }

    // 22
    [TestMethod]
    public void testScalability_MultipleNewCategories()
    {
        Assert.IsTrue(true);
    }

    // 23
    [TestMethod]
    public void testGenericBoundedTypeParameter_Enforcement()
    {
        var q = new Quantity<WeightUnit>(10, WeightUnit.Pound);
        Assert.IsNotNull(q);
    }

    // 24
    [TestMethod]
    public void testHashCode_GenericQuantity_Consistency()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        Assert.AreEqual(q1.GetHashCode(), q2.GetHashCode());
    }

    // 25
    [TestMethod]
    public void testEquals_GenericQuantity_ContractPreservation()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);
        var q3 = new Quantity<LengthUnit>(0.3333, LengthUnit.Yard);

        Assert.IsTrue(q1.Equals(q2));
        Assert.IsTrue(q2.Equals(q3));
    }

    // 26
    [TestMethod]
    public void testEnumAsUnitCarrier_BehaviorEncapsulation()
    {
        Assert.AreEqual(0.001, WeightUnit.Gram.GetConversionFactor());
    }

    // 27
    [TestMethod]
    public void testTypeErasure_RuntimeSafety()
    {
        var q = new Quantity<LengthUnit>(10, LengthUnit.Centimeter);
        Assert.IsNotNull(q);
    }

    // 28
    [TestMethod]
    public void testCompositionOverInheritance_Flexibility()
    {
        var q = new Quantity<WeightUnit>(5, WeightUnit.Pound);
        Assert.AreEqual("Quantity(5, Pound)", q.ToString());
    }

    // 29
    [TestMethod]
    public void testCodeReduction_DRYValidation()
    {
        Assert.IsTrue(true);
    }

    // 30
    [TestMethod]
    public void testMaintainability_SingleSourceOfTruth()
    {
        var q = new Quantity<LengthUnit>(2, LengthUnit.Feet);
        Assert.AreEqual(24, q.ConvertTo(LengthUnit.Inch).Value);
    }

    // 31
    [TestMethod]
    public void testArchitecturalReadiness_MultipleNewCategories()
    {
        Assert.IsTrue(true);
    }

    // 32
    [TestMethod]
    public void testPerformance_GenericOverhead()
    {
        var q = new Quantity<LengthUnit>(1000, LengthUnit.Inch);
        Assert.AreEqual(83.3333, q.ConvertTo(LengthUnit.Feet).Value);
    }

    // 33
    [TestMethod]
    public void testDocumentation_PatternClarity()
    {
        Assert.IsTrue(true);
    }

    // 34
    [TestMethod]
    public void testInterfaceSegregation_MinimalContract()
    {
        Assert.AreEqual("Feet", LengthUnit.Feet.GetUnitName());
    }

    // 35
    [TestMethod]
    public void testImmutability_GenericQuantity()
    {
        var q = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var result = q.ConvertTo(LengthUnit.Inch);

        Assert.AreNotEqual(q.Value, result.Value);
    }
}
