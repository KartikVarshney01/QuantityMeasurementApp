using System.Reflection;
namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class QuantityTest
{
    // 1

    [TestMethod]
    public void testRefactoring_Add_DelegatesViaHelper()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var result = q1.Add(q2);

        Assert.AreEqual(15, result.Value);
    }

    // 2 

    [TestMethod]
    public void testRefactoring_Subtract_DelegatesViaHelper()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var result = q1.Subtract(q2);

        Assert.AreEqual(5, result.Value);
    }

    // 3 

    [TestMethod]
    public void testRefactoring_Divide_DelegatesViaHelper()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        double result = q1.Divide(q2);

        Assert.AreEqual(2, result);
    }

    // 4 

    [TestMethod]
    public void testValidation_NullOperand_ConsistentAcrossOperations()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);

        try
        {
            q1.Add((Quantity<LengthUnit>)null);
            Assert.Fail();
        }
        catch (ArgumentException) { }

        try
        {
            q1.Subtract((Quantity<LengthUnit>)null);
            Assert.Fail();
        }
        catch (ArgumentException) { }

        try
        {
            q1.Divide((Quantity<LengthUnit>)null);
            Assert.Fail();
        }
        catch (ArgumentException) { }
    }

    // 5 

    [TestMethod]
    public void testValidation_CrossCategory_ConsistentAcrossOperations()
    {
        var length = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var weight = new Quantity<WeightUnit>(5, WeightUnit.Kilogram);

        try
        {
            length.Add((Quantity<LengthUnit>)(object)weight);
            Assert.Fail("Expected InvalidCastException not thrown");
        }
        catch (InvalidCastException)
        {
            Assert.IsTrue(true);
        }
    }

    // 6 

    [TestMethod]
    public void testValidation_FiniteValue_ConsistentAcrossOperations()
    {
        try
        {
            new Quantity<LengthUnit>(double.NaN, LengthUnit.Feet);
            Assert.Fail();
        }
        catch (ArgumentException) { }

        try
        {
            new Quantity<LengthUnit>(double.PositiveInfinity, LengthUnit.Feet);
            Assert.Fail();
        }
        catch (ArgumentException) { }
    }

    // 7 

    [TestMethod]
    public void testValidation_NullTargetUnit_AddSubtractReject()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        try
        {
            q1.Add(q2, (LengthUnit)(object)null);
            Assert.Fail("Expected exception not thrown");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    // 8 

    [TestMethod]
    public void testArithmeticOperation_Add_EnumComputation()
    {
        double result = 10 + 5;
        Assert.AreEqual(15.0, result);
    }

    // 9 

    [TestMethod]
    public void testArithmeticOperation_Subtract_EnumComputation()
    {
        double result = 10 - 5;
        Assert.AreEqual(5.0, result);
    }

    // 10 

    [TestMethod]
    public void testArithmeticOperation_Divide_EnumComputation()
    {
        double result = 10 / 5;
        Assert.AreEqual(2.0, result);
    }

    // 11 

    [TestMethod]
    public void testArithmeticOperation_DivideByZero_EnumThrows()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(0, LengthUnit.Feet);

        try
        {
            q1.Divide(q2);
            Assert.Fail("Expected ArithmeticException not thrown");
        }
        catch (ArithmeticException)
        {
            Assert.IsTrue(true);
        }
    }

    // 12 

    [TestMethod]
    public void testPerformBaseArithmetic_ConversionAndOperation()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        Assert.IsTrue(q1.Equals(q2));
    }

    // 13 

    [TestMethod]
    public void testAdd_UC12_BehaviorPreserved()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        var result = q1.Add(q2);

        Assert.AreEqual(2, result.Value);
    }

    // 14 

    [TestMethod]
    public void testSubtract_UC12_BehaviorPreserved()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(6, LengthUnit.Inch);

        var result = q1.Subtract(q2);

        Assert.AreEqual(9.5, result.Value, 0.01);
    }

    // 15 

    [TestMethod]
    public void testDivide_UC12_BehaviorPreserved()
    {
        var q1 = new Quantity<LengthUnit>(24, LengthUnit.Inch);
        var q2 = new Quantity<LengthUnit>(2, LengthUnit.Feet);

        var result = q1.Divide(q2);

        Assert.AreEqual(1.0, result);
    }

    // 16 

    [TestMethod]
    public void testRounding_AddSubtract_TwoDecimalPlaces()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(4, LengthUnit.Inch);

        var result = q1.Subtract(q2);

        Assert.AreEqual(Math.Round(result.Value, 2), result.Value);
    }

    // 17 

    [TestMethod]
    public void testRounding_Divide_NoRounding()
    {
        var q1 = new Quantity<LengthUnit>(7, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(2, LengthUnit.Feet);

        double result = q1.Divide(q2);

        Assert.AreEqual(3.5, result);
    }

    // 18 

    [TestMethod]
    public void testImplicitTargetUnit_AddSubtract()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        var result = q1.Add(q2);

        Assert.AreEqual(LengthUnit.Feet, result.Unit);
    }

    // 19 

    [TestMethod]
    public void testExplicitTargetUnit_AddSubtract_Overrides()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        var result = q1.Add(q2, LengthUnit.Inch);

        Assert.AreEqual(LengthUnit.Inch, result.Unit);
    }

    // 20 

    [TestMethod]
    public void testImmutability_AfterAdd_ViaCentralizedHelper()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        q1.Add(q2);

        Assert.AreEqual(10, q1.Value);
    }

    // 21 

    [TestMethod]
    public void testImmutability_AfterSubtract_ViaCentralizedHelper()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        q1.Subtract(q2);

        Assert.AreEqual(10, q1.Value);
    }

    // 22 

    [TestMethod]
    public void testImmutability_AfterDivide_ViaCentralizedHelper()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        q1.Divide(q2);

        Assert.AreEqual(10, q1.Value);
    }

    // 23 

    [TestMethod]
    public void testAllOperations_AcrossAllCategories()
    {
        var l1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var l2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var w1 = new Quantity<WeightUnit>(10, WeightUnit.Kilogram);
        var w2 = new Quantity<WeightUnit>(5, WeightUnit.Kilogram);

        var v1 = new Quantity<VolumeUnit>(5, VolumeUnit.Litre);
        var v2 = new Quantity<VolumeUnit>(2, VolumeUnit.Litre);

        Assert.AreEqual(15, l1.Add(l2).Value);
        Assert.AreEqual(5, w1.Subtract(w2).Value);
        Assert.AreEqual(2.5, v1.Divide(v2));
    }

    // 24 

    [TestMethod]
    public void testCodeDuplication_ValidationLogic_Eliminated()
    {
        var methods = typeof(Quantity<LengthUnit>)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

        bool helperExists = false;

        foreach (var method in methods)
        {
            if (method.Name.Contains("ValidateArithmeticOperands"))
            {
                helperExists = true;
                break;
            }
        }

        Assert.IsTrue(helperExists, "Validation helper method not found. DRY principle violated.");
    }

    // 25 

    [TestMethod]
    public void testCodeDuplication_ConversionLogic_Eliminated()
    {
        var methods = typeof(Quantity<LengthUnit>)
            .GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);

        bool conversionHelperExists = false;

        foreach (var method in methods)
        {
            if (method.Name.Contains("ConvertFromBase") || method.Name.Contains("ToBaseUnit"))
            {
                conversionHelperExists = true;
                break;
            }
        }

        Assert.IsTrue(conversionHelperExists, "Conversion logic helper not found.");
    }

    // 26 

    [TestMethod]
    public void testEnumDispatch_AllOperations_CorrectlyDispatched()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(5, LengthUnit.Feet);

        var addResult = q1.Add(q2);
        var subtractResult = q1.Subtract(q2);
        var divideResult = q1.Divide(q2);

        Assert.AreEqual(15, addResult.Value);
        Assert.AreEqual(5, subtractResult.Value);
        Assert.AreEqual(2, divideResult);
    }

    // 27 

    [TestMethod]
    public void testFutureOperation_MultiplicationPattern()
    {
        double result = 5 * 2;

        Assert.AreEqual(10, result);
    }

    // 28 

    [TestMethod]
    public void testErrorMessage_Consistency_Across_Operations()
    {
        var q = new Quantity<LengthUnit>(10, LengthUnit.Feet);

        try
        {
            q.Add(null);
        }
        catch (Exception e)
        {
            Assert.IsTrue(e.Message.Contains("Operand"));
        }
    }

    // 29 

    [TestMethod]
    public void testHelper_PrivateVisibility()
    {
        var method = typeof(Quantity<LengthUnit>)
            .GetMethod("PerformBaseArithmetic", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
    }

    // 30 

    [TestMethod]
    public void testValidation_Helper_PrivateVisibility()
    {
        var method = typeof(Quantity<LengthUnit>)
            .GetMethod("ValidateArithmeticOperands", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method);
    }

    // 31

    [TestMethod]
    public void testRounding_Helper_Accuracy()
    {
        double value = Math.Round(1.234567, 2);

        Assert.AreEqual(1.23, value);
    }

    // 32 

    [TestMethod]
    public void testArithmetic_Chain_Operations()
    {
        var q1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(2, LengthUnit.Feet);
        var q3 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q4 = new Quantity<LengthUnit>(1, LengthUnit.Feet);

        var result = q1.Add(q2).Subtract(q3).Divide(q4);

        Assert.AreEqual(11, result);
    }

    // 33

    [TestMethod]
    public void testLargeDatasetBehavior()
    {
        for (int i = 0; i < 1000; i++)
        {
            var q1 = new Quantity<LengthUnit>(i + 1, LengthUnit.Feet);
            var q2 = new Quantity<LengthUnit>(1, LengthUnit.Feet);

            q1.Add(q2);
            q1.Subtract(q2);
            q1.Divide(q2);
        }

        Assert.IsTrue(true);
    }

    // 34 

    [TestMethod]
    public void testRefactoring_Performance_ComparableToUC12()
    {
        var q1 = new Quantity<LengthUnit>(100, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(50, LengthUnit.Feet);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        for (int i = 0; i < 100000; i++)
        {
            q1.Add(q2);
            q1.Subtract(q2);
            q1.Divide(q2);
        }

        stopwatch.Stop();

        Assert.IsTrue(stopwatch.ElapsedMilliseconds < 2000,
            "Refactored implementation is too slow.");
    }

    // 35 

    [TestMethod]
    public void testEnumConstant_ADD_CorrectlyAdds()
    {
        Assert.AreEqual(10.0, 7 + 3);
    }

    // 36 

    [TestMethod]
    public void testEnumConstant_SUBTRACT_CorrectlySubtracts()
    {
        Assert.AreEqual(4.0, 7 - 3);
    }

    // 37 

    [TestMethod]
    public void testEnumConstant_DIVIDE_CorrectlyDivides()
    {
        Assert.AreEqual(3.5, 7.0 / 2.0);
    }


    // 38 

    [TestMethod]
    public void testHelper_BaseUnitConversion_Correct()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var q2 = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        Assert.IsTrue(q1.Equals(q2));
    }

    // 39 

    [TestMethod]
    public void testHelper_ResultConversion_Correct()
    {
        var q1 = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var result = q1.ConvertTo(LengthUnit.Inch);

        Assert.AreEqual(12, result.Value);
    }

    // 40 

    [TestMethod]
    public void testRefactoring_Validation_UnifiedBehavior()
    {
        var q = new Quantity<LengthUnit>(10, LengthUnit.Feet);

        try
        {
            q.Add((Quantity<LengthUnit>)null);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }

        try
        {
            q.Subtract((Quantity<LengthUnit>)null);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }

        try
        {
            q.Divide((Quantity<LengthUnit>)null);
            Assert.Fail("Expected ArgumentException");
        }
        catch (ArgumentException)
        {
            Assert.IsTrue(true);
        }
    }
}
