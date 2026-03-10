namespace QuantityMeasurementApp;

class QuantityMeasurementAppMain
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("===== LENGTH OPERATIONS =====");

            Quantity<LengthUnit> length1 = new Quantity<LengthUnit>(1.0, LengthUnit.Feet);
            Quantity<LengthUnit> length2 = new Quantity<LengthUnit>(12.0, LengthUnit.Inch);

            DemonstrateEquality(length1, length2);
            DemonstrateConversion(length1, LengthUnit.Inch);
            DemonstrateAddition(length1, length2, LengthUnit.Feet);


            Console.WriteLine("\n===== WEIGHT OPERATIONS =====");

            Quantity<WeightUnit> weight1 = new Quantity<WeightUnit>(1.0, WeightUnit.Kilogram);
            Quantity<WeightUnit> weight2 = new Quantity<WeightUnit>(1000.0, WeightUnit.Gram);

            DemonstrateEquality(weight1, weight2);
            DemonstrateConversion(weight1, WeightUnit.Gram);
            DemonstrateAddition(weight1, weight2, WeightUnit.Kilogram);
            

            Console.WriteLine("\n===== VOLUME OPERATIONS =====");

            Quantity<VolumeUnit> volume1 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Litre);
            Quantity<VolumeUnit> volume2 = new Quantity<VolumeUnit>(1000.0, VolumeUnit.Millilitre);
            Quantity<VolumeUnit> volume3 = new Quantity<VolumeUnit>(1.0, VolumeUnit.Gallon);

            DemonstrateEquality(volume1, volume2);
            DemonstrateConversion(volume1, VolumeUnit.Millilitre);
            DemonstrateAddition(volume1, volume2, VolumeUnit.Litre);

            DemonstrateConversion(volume3, VolumeUnit.Litre);
            DemonstrateAddition(volume1, volume3, VolumeUnit.Millilitre);
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