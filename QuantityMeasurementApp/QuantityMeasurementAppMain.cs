    namespace QuantityMeasurementApp;
    class QuantityMeasurementAppMain
    {
        static void Main(string[] args)
        {
            try
            {
                // First input
                Console.Write("Enter first value: ");
                double value1 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter first unit (Feet/Inch): ");
                LengthUnit unit1 = ParseUnit(Console.ReadLine());

                // Second input
                Console.Write("Enter second value: ");
                double value2 = Convert.ToDouble(Console.ReadLine());

                Console.Write("Enter second unit (Feet/Inch): ");
                LengthUnit unit2 = ParseUnit(Console.ReadLine());

                // Create objects
                Quantity quantity1 = new Quantity(unit1,value1);
                Quantity quantity2 = new Quantity(unit2,value2);

                // Compare
                bool result = quantity1.Equals(quantity2);

                Console.WriteLine(result ? "Equal (true)" : "Not Equal (false)");
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

            throw new ArgumentException("Invalid unit! Use 'Feet' or 'Inch'");
        }
    }