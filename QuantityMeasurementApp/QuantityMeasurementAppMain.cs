namespace QuantityMeasurementApp;

class QuantityMeasurementAppMain
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("===== LENGTH =====");

            var length1 = new Quantity<LengthUnit>(10, LengthUnit.Feet);
            var length2 = new Quantity<LengthUnit>(6, LengthUnit.Inch);

            Console.WriteLine(length1.Subtract(length2));
            Console.WriteLine(length1.Divide(length2));


            Console.WriteLine("\n===== WEIGHT =====");

            var weight1 = new Quantity<WeightUnit>(10, WeightUnit.Kilogram);
            var weight2 = new Quantity<WeightUnit>(5000, WeightUnit.Gram);

            Console.WriteLine(weight1.Subtract(weight2));
            Console.WriteLine(weight1.Divide(weight2));


            Console.WriteLine("\n===== VOLUME =====");

            var volume1 = new Quantity<VolumeUnit>(5, VolumeUnit.Litre);
            var volume2 = new Quantity<VolumeUnit>(500, VolumeUnit.Millilitre);

            Console.WriteLine(volume1.Subtract(volume2));
            Console.WriteLine(volume1.Divide(volume2));
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
}