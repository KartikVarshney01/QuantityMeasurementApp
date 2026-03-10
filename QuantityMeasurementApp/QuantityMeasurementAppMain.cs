namespace QuantityMeasurementApp;

class QuantityMeasurementAppMain
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("WEIGHT MEASUREMENT OPERATIONS");

            // ---------- Equality ----------
            
            Console.WriteLine("\nEquality Comparison");

            Console.Write("Enter first value: ");
            double value1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter first unit (Kilogram/Gram/Pound): ");
            WeightUnit unit1 = ParseUnit(Console.ReadLine() ?? "");

            Console.Write("Enter second value: ");
            double value2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter second unit (Kilogram/Gram/Pound): ");
            WeightUnit unit2 = ParseUnit(Console.ReadLine() ?? "");

            QuantityWeight w1 = new QuantityWeight(unit1, value1);
            QuantityWeight w2 = new QuantityWeight(unit2, value2);

            Console.WriteLine($"Equality Result: {w1.Equals(w2)}");

            // ---------- Conversion ----------

            Console.WriteLine("\nWeight Conversion");

            Console.Write("Enter value: ");
            double convertValue = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter source unit (Kilogram/Gram/Pound): ");
            WeightUnit sourceUnit = ParseUnit(Console.ReadLine() ?? "");

            Console.Write("Enter target unit (Kilogram/Gram/Pound): ");
            WeightUnit targetUnit = ParseUnit(Console.ReadLine() ?? "");

            QuantityWeight weight = new QuantityWeight(sourceUnit, convertValue);
            QuantityWeight converted = weight.ConvertTo(targetUnit);

            Console.WriteLine($"Converted Result: {converted.Value} {converted.Unit}");

            // ---------- Addition ----------

            Console.WriteLine("\nWeight Addition");

            Console.Write("Enter first value: ");
            double addValue1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter first unit (Kilogram/Gram/Pound): ");
            WeightUnit addUnit1 = ParseUnit(Console.ReadLine() ?? "");

            Console.Write("Enter second value: ");
            double addValue2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter second unit (Kilogram/Gram/Pound): ");
            WeightUnit addUnit2 = ParseUnit(Console.ReadLine() ?? "");

            Console.Write("Enter target unit for result (Kilogram/Gram/Pound): ");
            WeightUnit addTarget = ParseUnit(Console.ReadLine() ?? "");

            QuantityWeight a = new QuantityWeight(addUnit1, addValue1);
            QuantityWeight b = new QuantityWeight(addUnit2, addValue2);

            QuantityWeight sum = QuantityWeight.Add(a, b, addTarget);

            Console.WriteLine($"Addition Result: {sum.Value} {sum.Unit}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid numeric input!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static WeightUnit ParseUnit(string input)
    {
        if (Enum.TryParse(input, true, out WeightUnit unit))
            return unit;

        throw new ArgumentException("Invalid unit! Use Kilogram/Gram/Pound");
    }
}