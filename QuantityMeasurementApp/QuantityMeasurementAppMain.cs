namespace QuantityMeasurementApp;

class QuantityMeasurementAppMain
{
    static void Main(string[] args)
    {
        try
        {
            Console.WriteLine("Enter First Quantity");

            Console.Write("Value: ");
            double value1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Unit (Feet/Inch/Yard/Centimeter): ");
            LengthUnit unit1 = ParseUnit(Console.ReadLine() ?? "");

            Console.WriteLine();

            Console.WriteLine("Enter Second Quantity");

            Console.Write("Value: ");
            double value2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Unit (Feet/Inch/Yard/Centimeter): ");
            LengthUnit unit2 = ParseUnit(Console.ReadLine() ?? "");

            Console.WriteLine();

            Console.Write("Enter target unit for result (Feet/Inch/Yard/Centimeter): ");
            LengthUnit targetUnit = ParseUnit(Console.ReadLine() ?? "");

            Quantity q1 = new Quantity(unit1, value1);
            Quantity q2 = new Quantity(unit2, value2);

            Quantity result = Quantity.Add(q1, q2, targetUnit);

            Console.WriteLine();
            Console.WriteLine($"Result: {result.Value} {result.Unit}");
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid number input!");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private static LengthUnit ParseUnit(string input)
    {
        if (Enum.TryParse(input, true, out LengthUnit unit))
            return unit;

        throw new ArgumentException("Invalid unit! Use Feet/Inch/Yard/Centimeter");
    }
}