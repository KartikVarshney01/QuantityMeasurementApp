    namespace QuantityMeasurementApp;
    class QuantityMeasurementAppMain
    {
        static void Main(string[] args)
        {
            try
            {
                // Value input
                Console.Write("Enter value: ");
                double value = Convert.ToDouble(Console.ReadLine());

                // Source unit
                Console.Write("Enter source unit (Feet/Inch/Yard/Centimeter): ");
                LengthUnit source = ParseUnit(Console.ReadLine() ?? "");

                // Target unit
                Console.Write("Enter target unit (Feet/Inch/Yard/Centimeter): ");
                LengthUnit target = ParseUnit(Console.ReadLine() ?? "");

                double result = Quantity.Convert(value, source, target);
                Console.WriteLine($"Converted Value: {result}");

                // Using Instance Method
                Quantity q = new Quantity(source, value);
                Console.WriteLine($"(Using ConvertTo) → {q.ConvertTo(target)}");
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

        // Helper method to parse unit safely
        private static LengthUnit ParseUnit(string input)
        {
            if (Enum.TryParse(input, true, out LengthUnit unit))
                return unit;

            throw new ArgumentException("Invalid unit! Use 'Feet' or 'Inch' or 'Yard' or 'Centimeter'");
        }
    }