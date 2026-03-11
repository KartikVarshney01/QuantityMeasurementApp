namespace QuantityMeasurementApp;

class QuantityMeasurementAppMain
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("===== LENGTH OPERATIONS =====");

            var length1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
            var length2 = new Quantity<LengthUnit>(6, LengthUnit.Inch);

            DemonstrateEquality(length1, length2);
            DemonstrateConversion(length1, LengthUnit.Inch);
            DemonstrateAddition(length1, length2, LengthUnit.Feet);
            DemonstrateSubtraction(length1, length2, LengthUnit.Feet);
            DemonstrateDivision(length1, length2);


            Console.WriteLine("\n===== WEIGHT OPERATIONS =====");

            var weight1 = new Quantity<WeightUnit>(10, WeightUnit.Kilogram);
            var weight2 = new Quantity<WeightUnit>(5000, WeightUnit.Gram);

            DemonstrateEquality(weight1, weight2);
            DemonstrateConversion(weight1, WeightUnit.Gram);
            DemonstrateAddition(weight1, weight2, WeightUnit.Kilogram);
            DemonstrateSubtraction(weight1, weight2, WeightUnit.Kilogram);
            DemonstrateDivision(weight1, weight2);


            Console.WriteLine("\n===== VOLUME OPERATIONS =====");

            var volume1 = new Quantity<VolumeUnit>(5, VolumeUnit.Litre);
            var volume2 = new Quantity<VolumeUnit>(500, VolumeUnit.Millilitre);

            DemonstrateEquality(volume1, volume2);
            DemonstrateConversion(volume1, VolumeUnit.Millilitre);
            DemonstrateAddition(volume1, volume2, VolumeUnit.Litre);
            DemonstrateSubtraction(volume1, volume2, VolumeUnit.Litre);
            DemonstrateDivision(volume1, volume2);


            Console.WriteLine("\n===== TEMPERATURE OPERATIONS =====");

            var temp1 = new Quantity<TemperatureUnit>(0, TemperatureUnit.Celsius);
            var temp2 = new Quantity<TemperatureUnit>(32, TemperatureUnit.Fahrenheit);

            DemonstrateEquality(temp1, temp2);
            DemonstrateConversion(temp1, TemperatureUnit.Fahrenheit);

            // Arithmetic should fail
            try
            {
                DemonstrateAddition(temp1, temp2, TemperatureUnit.Celsius);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Addition Error: {ex.Message}");
            }

            try
            {
                DemonstrateSubtraction(temp1, temp2, TemperatureUnit.Celsius);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Subtraction Error: {ex.Message}");
            }

            try
            {
                DemonstrateDivision(temp1, temp2);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Division Error: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    static void DemonstrateEquality<U>(Quantity<U> q1, Quantity<U> q2)
        where U : Enum
    {
        Console.WriteLine($"Equality: {q1} == {q2} → {q1.Equals(q2)}");
    }

    static void DemonstrateConversion<U>(Quantity<U> quantity, U targetUnit)
        where U : Enum
    {
        var result = quantity.ConvertTo(targetUnit);
        Console.WriteLine($"Conversion: {quantity} → {result}");
    }

    static void DemonstrateAddition<U>(Quantity<U> q1, Quantity<U> q2, U targetUnit)
        where U : Enum
    {
        var result = q1.Add(q2, targetUnit);
        Console.WriteLine($"Addition: {q1} + {q2} → {result}");
    }

    static void DemonstrateSubtraction<U>(Quantity<U> q1, Quantity<U> q2, U targetUnit)
        where U : Enum
    {
        var result = q1.Subtract(q2, targetUnit);
        Console.WriteLine($"Subtraction: {q1} - {q2} → {result}");
    }

    static void DemonstrateDivision<U>(Quantity<U> q1, Quantity<U> q2)
        where U : Enum
    {
        var result = q1.Divide(q2);
        Console.WriteLine($"Division: {q1} / {q2} → {result}");
    }
}