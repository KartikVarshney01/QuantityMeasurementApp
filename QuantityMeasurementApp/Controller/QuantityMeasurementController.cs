using QuantityMeasurementAppModelLayer.Dto;
using QuantityMeasurementAppBusinessLayer.Interface;
using QuantityMeasurementAppBusinessLayer.Exception;
using QuantityMeasurementAppRepoLayer.Interface;
using QuantityMeasurementApp.Interface;

namespace QuantityMeasurementApp.Controller;

// UC15: Controller — owns all console menu interaction
// Calls IQuantityMeasurementService for all operations — no business logic here
public class QuantityMeasurementController : IQuantityMeasurementApp
{
    private readonly IQuantityMeasurementService    _service;
    private readonly IQuantityMeasurementRepository _repository;

    public QuantityMeasurementController(
        IQuantityMeasurementService service,
        IQuantityMeasurementRepository repository)
    {
        _service    = service    ?? throw new ArgumentNullException(nameof(service));
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // ── Main application loop ────────────────────────────────────────

    public void Run()
    {
        Console.WriteLine("╔══════════════════════════════════════╗");
        Console.WriteLine("║   Quantity Measurement Application   ║");
        Console.WriteLine("╚══════════════════════════════════════╝");

        bool running = true;
        while (running)
        {
            Console.WriteLine("\n===== MAIN MENU =====");
            Console.WriteLine("1. Length");
            Console.WriteLine("2. Weight");
            Console.WriteLine("3. Volume");
            Console.WriteLine("4. Temperature");
            Console.WriteLine("5. Operation History");
            Console.WriteLine("0. Exit");
            Console.Write("\nSelect category: ");

            switch (Console.ReadLine()?.Trim())
            {
                case "1": RunLengthMenu();      break;
                case "2": RunWeightMenu();      break;
                case "3": RunVolumeMenu();      break;
                case "4": RunTemperatureMenu(); break;
                case "5": ShowHistory();        break;
                case "0":
                    running = false;
                    Console.WriteLine("Goodbye!");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please enter 1-5 or 0.");
                    break;
            }
        }
    }

    // ── Public Perform* methods (required by UC15 tests) ─────────────

    public string PerformComparison(QuantityDTO q1, QuantityDTO q2)
    {
        try
        {
            var result = _service.Compare(q1, q2);
            bool equal = result.Value == 1;
            return $"Comparison Result: {(equal ? "true" : "false")}";
        }
        catch (QuantityMeasurementException ex)
        {
            return $"[ERROR] {ex.Message}";
        }
    }

    public string PerformConversion(QuantityDTO q1, QuantityDTO targetUnit)
    {
        try
        {
            var result = _service.Convert(q1, targetUnit);
            return $"Conversion Result: {result.Value} {result.UnitName}";
        }
        catch (QuantityMeasurementException ex)
        {
            return $"[ERROR] {ex.Message}";
        }
    }

    public string PerformAddition(QuantityDTO q1, QuantityDTO q2)
    {
        try
        {
            var result = _service.Add(q1, q2);
            return $"Addition Result: {result.Value} {result.UnitName}";
        }
        catch (QuantityMeasurementException ex)
        {
            return $"[ERROR] {ex.Message}";
        }
    }

    public string PerformSubtraction(QuantityDTO q1, QuantityDTO q2)
    {
        try
        {
            var result = _service.Subtract(q1, q2);
            return $"Subtraction Result: {result.Value} {result.UnitName}";
        }
        catch (QuantityMeasurementException ex)
        {
            return $"[ERROR] {ex.Message}";
        }
    }

    public string PerformDivision(QuantityDTO q1, QuantityDTO q2)
    {
        try
        {
            var result = _service.Divide(q1, q2);
            return $"Division Result: {result.Value} (scalar)";
        }
        catch (QuantityMeasurementException ex)
        {
            return $"[ERROR] {ex.Message}";
        }
    }

    // ── LENGTH MENU ──────────────────────────────────────────────────

    private void RunLengthMenu()
    {
        Console.WriteLine("\n--- LENGTH UNITS: Feet | Inch | Yard | Centimeter ---");
        string op = SelectOperation(supportsArithmetic: true);
        if (op == "0") return;

        try
        {
            if (op == "1") // Convert
            {
                var v = ReadValue("Enter value"); var u = ReadUnit("Enter unit (e.g. Feet)"); var t = ReadUnit("Convert to unit");
                Console.WriteLine(PerformConversion(new QuantityDTO(v, u, "LENGTH"), new QuantityDTO(0, t, "LENGTH")));
            }
            else if (op == "2") // Compare
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformComparison(new QuantityDTO(v1, u1, "LENGTH"), new QuantityDTO(v2, u2, "LENGTH")));
            }
            else if (op == "3") // Add
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformAddition(new QuantityDTO(v1, u1, "LENGTH"), new QuantityDTO(v2, u2, "LENGTH")));
            }
            else if (op == "4") // Subtract
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformSubtraction(new QuantityDTO(v1, u1, "LENGTH"), new QuantityDTO(v2, u2, "LENGTH")));
            }
            else if (op == "5") // Divide
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformDivision(new QuantityDTO(v1, u1, "LENGTH"), new QuantityDTO(v2, u2, "LENGTH")));
            }
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // ── WEIGHT MENU ──────────────────────────────────────────────────

    private void RunWeightMenu()
    {
        Console.WriteLine("\n--- WEIGHT UNITS: Kilogram | Gram | Pound ---");
        string op = SelectOperation(supportsArithmetic: true);
        if (op == "0") return;

        try
        {
            if (op == "1")
            {
                var v = ReadValue("Enter value"); var u = ReadUnit("Enter unit (e.g. Kilogram)"); var t = ReadUnit("Convert to unit");
                Console.WriteLine(PerformConversion(new QuantityDTO(v, u, "WEIGHT"), new QuantityDTO(0, t, "WEIGHT")));
            }
            else if (op == "2")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformComparison(new QuantityDTO(v1, u1, "WEIGHT"), new QuantityDTO(v2, u2, "WEIGHT")));
            }
            else if (op == "3")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformAddition(new QuantityDTO(v1, u1, "WEIGHT"), new QuantityDTO(v2, u2, "WEIGHT")));
            }
            else if (op == "4")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformSubtraction(new QuantityDTO(v1, u1, "WEIGHT"), new QuantityDTO(v2, u2, "WEIGHT")));
            }
            else if (op == "5")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformDivision(new QuantityDTO(v1, u1, "WEIGHT"), new QuantityDTO(v2, u2, "WEIGHT")));
            }
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // ── VOLUME MENU ──────────────────────────────────────────────────

    private void RunVolumeMenu()
    {
        Console.WriteLine("\n--- VOLUME UNITS: Litre | Millilitre | Gallon ---");
        string op = SelectOperation(supportsArithmetic: true);
        if (op == "0") return;

        try
        {
            if (op == "1")
            {
                var v = ReadValue("Enter value"); var u = ReadUnit("Enter unit (e.g. Litre)"); var t = ReadUnit("Convert to unit");
                Console.WriteLine(PerformConversion(new QuantityDTO(v, u, "VOLUME"), new QuantityDTO(0, t, "VOLUME")));
            }
            else if (op == "2")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformComparison(new QuantityDTO(v1, u1, "VOLUME"), new QuantityDTO(v2, u2, "VOLUME")));
            }
            else if (op == "3")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformAddition(new QuantityDTO(v1, u1, "VOLUME"), new QuantityDTO(v2, u2, "VOLUME")));
            }
            else if (op == "4")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformSubtraction(new QuantityDTO(v1, u1, "VOLUME"), new QuantityDTO(v2, u2, "VOLUME")));
            }
            else if (op == "5")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformDivision(new QuantityDTO(v1, u1, "VOLUME"), new QuantityDTO(v2, u2, "VOLUME")));
            }
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // ── TEMPERATURE MENU ─────────────────────────────────────────────

    private void RunTemperatureMenu()
    {
        Console.WriteLine("\n--- TEMPERATURE UNITS: Celsius | Fahrenheit | Kelvin ---");
        Console.WriteLine("(Note: Arithmetic operations are NOT supported for Temperature)");
        string op = SelectOperation(supportsArithmetic: false);
        if (op == "0") return;

        try
        {
            if (op == "1")
            {
                var v = ReadValue("Enter value"); var u = ReadUnit("Enter unit (e.g. Celsius)"); var t = ReadUnit("Convert to unit");
                Console.WriteLine(PerformConversion(new QuantityDTO(v, u, "TEMPERATURE"), new QuantityDTO(0, t, "TEMPERATURE")));
            }
            else if (op == "2")
            {
                var v1 = ReadValue("Enter first value"); var u1 = ReadUnit("Enter first unit");
                var v2 = ReadValue("Enter second value"); var u2 = ReadUnit("Enter second unit");
                Console.WriteLine(PerformComparison(new QuantityDTO(v1, u1, "TEMPERATURE"), new QuantityDTO(v2, u2, "TEMPERATURE")));
            }
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // ── HISTORY ──────────────────────────────────────────────────────

    private void ShowHistory()
    {
        Console.WriteLine("\n===== OPERATION HISTORY =====");
        var records = _repository.GetAllMeasurements();

        if (records.Count == 0)
        {
            Console.WriteLine("No operations recorded yet.");
            return;
        }

        foreach (var record in records)
            Console.WriteLine(record);
    }

    // ── INPUT HELPERS — your original logic ──────────────────────────

    private static string SelectOperation(bool supportsArithmetic)
    {
        Console.WriteLine("\nSelect operation:");
        Console.WriteLine("1. Convert");
        Console.WriteLine("2. Compare");
        if (supportsArithmetic)
        {
            Console.WriteLine("3. Add");
            Console.WriteLine("4. Subtract");
            Console.WriteLine("5. Divide");
        }
        Console.WriteLine("0. Back");
        Console.Write("Choice: ");

        string? input = Console.ReadLine()?.Trim();

        if (!supportsArithmetic && (input == "3" || input == "4" || input == "5"))
        {
            Console.WriteLine("Error: Arithmetic is not supported for Temperature.");
            return "0";
        }

        return input ?? "0";
    }

    private static double ReadValue(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            string? input = Console.ReadLine()?.Trim();
            if (double.TryParse(input, out double value))
                return value;
            Console.WriteLine($"'{input}' is not a valid number. Try again.");
        }
    }

    private static string ReadUnit(string prompt)
    {
        while (true)
        {
            Console.Write($"{prompt}: ");
            string? input = Console.ReadLine()?.Trim();
            if (!string.IsNullOrEmpty(input))
                return input;
            Console.WriteLine("Unit cannot be empty. Try again.");
        }
    }
}
