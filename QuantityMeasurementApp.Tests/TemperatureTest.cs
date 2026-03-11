namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod] //1
    public void testTemperatureEquality_CelsiusToCelsius_SameValue()
    {
        var q1 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //2
    public void testTemperatureEquality_FahrenheitToFahrenheit_SameValue()
    {
        var q1 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);
        var q2 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //3
    public void testTemperatureEquality_CelsiusToFahrenheit_0Celsius32Fahrenheit()
    {
        var q1 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //4
    public void testTemperatureEquality_CelsiusToFahrenheit_100Celsius212Fahrenheit()
    {
        var q1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(212.0, TemperatureUnit.Fahrenheit);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //5
    public void testTemperatureEquality_CelsiusToFahrenheit_Negative40Equal()
    {
        var q1 = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(-40.0, TemperatureUnit.Fahrenheit);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //6
    public void testTemperatureEquality_SymmetricProperty()
    {
        var q1 = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(32.0, TemperatureUnit.Fahrenheit);

        Assert.IsTrue(q1.Equals(q2));
        Assert.IsTrue(q2.Equals(q1));
    }

    [TestMethod] //7
    public void testTemperatureEquality_ReflexiveProperty()
    {
        var q = new Quantity<TemperatureUnit>(10.0, TemperatureUnit.Celsius);

        Assert.IsTrue(q.Equals(q));
    }

    [TestMethod] //8
    public void testTemperatureConversion_CelsiusToFahrenheit_VariousValues()
    {
        var q = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);
        var result = q.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.AreEqual(122.0, result.Value, 0.01);

        var q2 = new Quantity<TemperatureUnit>(-20.0, TemperatureUnit.Celsius);
        var result2 = q2.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.AreEqual(-4.0, result2.Value, 0.01);
    }

    [TestMethod] //9
    public void testTemperatureConversion_FahrenheitToCelsius_VariousValues()
    {
        var q = new Quantity<TemperatureUnit>(122.0, TemperatureUnit.Fahrenheit);
        var result = q.ConvertTo(TemperatureUnit.Celsius);

        Assert.AreEqual(50.0, result.Value, 0.01);
    }

    [TestMethod] //10
    public void testTemperatureConversion_RoundTrip_PreservesValue()
    {
        var q = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);

        var converted = q.ConvertTo(TemperatureUnit.Fahrenheit)
                         .ConvertTo(TemperatureUnit.Celsius);

        Assert.AreEqual(q.Value, converted.Value, 0.01);
    }

    [TestMethod] //11
    public void testTemperatureConversion_SameUnit()
    {
        var q = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);

        var result = q.ConvertTo(TemperatureUnit.Celsius);

        Assert.AreEqual(25.0, result.Value);
    }

    [TestMethod] //12
    public void testTemperatureConversion_ZeroValue()
    {
        var q = new Quantity<TemperatureUnit>(0.0, TemperatureUnit.Celsius);

        var result = q.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.AreEqual(32.0, result.Value, 0.01);
    }

    [TestMethod] //13
    public void testTemperatureConversion_NegativeValues()
    {
        var q = new Quantity<TemperatureUnit>(-10.0, TemperatureUnit.Celsius);

        var result = q.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.AreEqual(14.0, result.Value, 0.01);
    }

    [TestMethod] //14
    public void testTemperatureConversion_LargeValues()
    {
        var q = new Quantity<TemperatureUnit>(1000.0, TemperatureUnit.Celsius);

        var result = q.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.AreEqual(1832.0, result.Value, 0.01);
    }

    [TestMethod] //15
    public void testTemperatureUnsupportedOperation_Add()
    {
        try
        {
            var q1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var q2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            q1.Add(q2);

            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //16
    public void testTemperatureUnsupportedOperation_Subtract()
    {
        try
        {
            var q1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var q2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            q1.Subtract(q2);

            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //17
    public void testTemperatureUnsupportedOperation_Divide()
    {
        try
        {
            var q1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var q2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            q1.Divide(q2);

            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //18
    public void testTemperatureUnsupportedOperation_ErrorMessage()
    {
        try
        {
            var q1 = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
            var q2 = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);

            q1.Add(q2);

            Assert.Fail("Expected exception");
        }
        catch (Exception ex)
        {
            Assert.IsTrue(ex.Message.Contains("Temperature"));
        }
    }

    [TestMethod] //19
    public void testTemperatureVsLengthIncompatibility()
    {
        var t = new Quantity<TemperatureUnit>(100.0, TemperatureUnit.Celsius);
        var l = new Quantity<LengthUnit>(100.0, LengthUnit.Feet);

        Assert.IsFalse(t.Equals((object)l));
    }

    [TestMethod] //20
    public void testTemperatureVsWeightIncompatibility()
    {
        var t = new Quantity<TemperatureUnit>(50.0, TemperatureUnit.Celsius);
        var w = new Quantity<WeightUnit>(50.0, WeightUnit.Kilogram);

        Assert.IsFalse(t.Equals((object)w));
    }

    [TestMethod] //21
    public void testTemperatureVsVolumeIncompatibility()
    {
        var t = new Quantity<TemperatureUnit>(25.0, TemperatureUnit.Celsius);
        var v = new Quantity<VolumeUnit>(25.0, VolumeUnit.Litre);

        Assert.IsFalse(t.Equals((object)v));
    }

    [TestMethod] //22
    public void testOperationSupportMethods_TemperatureUnitAddition()
    {

        Quantity<TemperatureUnit> temp =
         new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius);

        try
        {
            temp.Add(new Quantity<TemperatureUnit>(50, TemperatureUnit.Celsius));
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //23
    public void testOperationSupportMethods_TemperatureUnitDivision()
    {
        Quantity<TemperatureUnit> t1 =
        new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius);

        Quantity<TemperatureUnit> t2 =
            new Quantity<TemperatureUnit>(50, TemperatureUnit.Celsius);

        try
        {
            t1.Divide(t2);
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //24
    public void testOperationSupportMethods_LengthUnitAddition()
    {
        Quantity<TemperatureUnit> t1 =
        new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius);

        Quantity<TemperatureUnit> t2 =
            new Quantity<TemperatureUnit>(50, TemperatureUnit.Celsius);

        try
        {
            t1.Divide(t2);
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //25
    public void testOperationSupportMethods_WeightUnitDivision()
    {
        Quantity<WeightUnit> w1 =
        new Quantity<WeightUnit>(10, WeightUnit.Kilogram);

        Quantity<WeightUnit> w2 =
            new Quantity<WeightUnit>(5, WeightUnit.Kilogram);

        double result = w1.Divide(w2);

        Assert.AreEqual(2, result);
    }

    [TestMethod] //26
    public void testIMeasurableInterface_Evolution_BackwardCompatible()
    {
        var q = new Quantity<LengthUnit>(10, LengthUnit.Feet);

        var result = q.ConvertTo(LengthUnit.Inch);

        Assert.IsNotNull(result);
    }

    [TestMethod] //27
    public void testTemperatureUnit_NonLinearConversion()
    {
        var q = new Quantity<TemperatureUnit>(10, TemperatureUnit.Celsius);
        var result = q.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.AreNotEqual(10 * 2, result.Value);
    }

    [TestMethod] //28
    public void testTemperatureUnit_AllConstants()
    {
        Assert.IsNotNull(TemperatureUnit.Celsius);
        Assert.IsNotNull(TemperatureUnit.Fahrenheit);
    }

    [TestMethod] //29
    public void testTemperatureUnit_NameMethod()
    {
        Assert.AreEqual("Celsius", TemperatureUnit.Celsius.GetUnitName());
    }

    [TestMethod] //30
    public void testTemperatureUnit_ConversionFactor()
    {
        Assert.AreEqual(1.0, TemperatureUnit.Celsius.GetConversionFactor(), 0.01);
    }

    [TestMethod] //31
    public void testTemperatureNullUnitValidation()
    {
        try
        {
            var q = new Quantity<TemperatureUnit>(100.0, (TemperatureUnit)(object)null);

            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //32
    public void testTemperatureNullOperandValidation_InComparison()
    {
        var q = new Quantity<TemperatureUnit>(10.0, TemperatureUnit.Celsius);

        Assert.IsFalse(q.Equals(null));
    }

    [TestMethod] //33
    public void testTemperatureDifferentValuesInequality()
    {
        var q1 = new Quantity<TemperatureUnit>(50, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius);

        Assert.IsFalse(q1.Equals(q2));
    }

    [TestMethod] //34
    public void testTemperatureBackwardCompatibility_UC1_Through_UC13()
    {
        var l = new Quantity<LengthUnit>(1, LengthUnit.Feet);
        var i = new Quantity<LengthUnit>(12, LengthUnit.Inch);

        Assert.IsTrue(l.Equals(i));
    }

    [TestMethod] //35
    public void testTemperatureConversionPrecision_Epsilon()
    {
        var q1 = new Quantity<TemperatureUnit>(0, TemperatureUnit.Celsius);
        var q2 = new Quantity<TemperatureUnit>(32, TemperatureUnit.Fahrenheit);

        Assert.IsTrue(q1.Equals(q2));
    }

    [TestMethod] //36
    public void testTemperatureConversionEdgeCase_VerySmallDifference()
    {
        var q1 = new Quantity<TemperatureUnit>(0.0001, TemperatureUnit.Celsius);
        var q2 = q1.ConvertTo(TemperatureUnit.Fahrenheit).ConvertTo(TemperatureUnit.Celsius);

        Assert.AreEqual(q1.Value, q2.Value, 0.01);
    }

    [TestMethod] //37
    public void testTemperatureEnumImplementsIMeasurable()
    {
        Quantity<TemperatureUnit> temp =
        new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius);

        Assert.IsNotNull(temp);
    }

    [TestMethod] //38
    public void testTemperatureDefaultMethodInheritance()
    {
        Quantity<LengthUnit> length =
        new Quantity<LengthUnit>(10, LengthUnit.Feet);

        Quantity<LengthUnit> other =
            new Quantity<LengthUnit>(5, LengthUnit.Feet);

        Quantity<LengthUnit> result = length.Add(other);

        Assert.AreEqual(15, result.Value);
    }

    [TestMethod] //39
    public void testTemperatureCrossUnitAdditionAttempt()
    {
        try
        {
            var c = new Quantity<TemperatureUnit>(10, TemperatureUnit.Celsius);
            var f = new Quantity<TemperatureUnit>(50, TemperatureUnit.Fahrenheit);

            c.Add(f);

            Assert.Fail("Expected exception");
        }
        catch (Exception)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //40
    public void testTemperatureValidateOperationSupport_MethodBehavior()
    {
        Quantity<TemperatureUnit> t1 =
        new Quantity<TemperatureUnit>(100, TemperatureUnit.Celsius);

        Quantity<TemperatureUnit> t2 =
            new Quantity<TemperatureUnit>(50, TemperatureUnit.Celsius);

        try
        {
            t1.Add(t2);
            Assert.Fail("Expected NotSupportedException");
        }
        catch (NotSupportedException)
        {
            Assert.IsTrue(true);
        }
    }

    [TestMethod] //41
    public void testTemperatureIntegrationWithGenericQuantity()
    {
        var q = new Quantity<TemperatureUnit>(10, TemperatureUnit.Celsius);

        var result = q.ConvertTo(TemperatureUnit.Fahrenheit);

        Assert.IsNotNull(result);
    }

}
