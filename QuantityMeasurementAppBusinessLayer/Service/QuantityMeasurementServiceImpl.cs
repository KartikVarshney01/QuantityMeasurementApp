using QuantityMeasurementAppModelLayer.Dto;
using QuantityMeasurementAppModelLayer.Enum;
using QuantityMeasurementAppModelLayer.Entities;
using QuantityMeasurementAppBusinessLayer.Interface;
using QuantityMeasurementAppBusinessLayer.Exception;
using QuantityMeasurementAppRepoLayer.Interface;
using QuantityMeasurementAppRepoLayer;

namespace QuantityMeasurementAppBusinessLayer.Service;

// UC15 Service implementation
// Data flow: QuantityDTO in → resolve to enum → use Quantity<U> logic → QuantityDTO out
// Saves every operation (success or error) to repository
public class QuantityMeasurementServiceImpl : IQuantityMeasurementService
{
    private readonly IQuantityMeasurementRepository _repository;

    public QuantityMeasurementServiceImpl(IQuantityMeasurementRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    // ── COMPARE ──────────────────────────────────────────────────────

    public QuantityDTO Compare(QuantityDTO q1, QuantityDTO q2)
    {
        ValidateNotNull(q1, q2);
        ValidateSameCategory(q1, q2, "Compare");
        try
        {
            bool equal  = CompareByCategory(q1, q2);
            var  result = new QuantityDTO(equal ? 1 : 0, equal ? "EQUAL" : "NOT_EQUAL", "RESULT");
            _repository.Save(new QuantityMeasurementEntity("COMPARE", q1, q2, result));
            return result;
        }
        catch (System.Exception ex) when (ex is not QuantityMeasurementException)
        {
            _repository.Save(new QuantityMeasurementEntity("COMPARE", q1, q2, ex.Message));
            throw new QuantityMeasurementException($"Compare failed: {ex.Message}", ex);
        }
    }

    // ── CONVERT ──────────────────────────────────────────────────────

    public QuantityDTO Convert(QuantityDTO q1, QuantityDTO targetUnit)
    {
        ValidateNotNull(q1, targetUnit);
        try
        {
            var result = ConvertByCategory(q1, targetUnit.UnitName);
            _repository.Save(new QuantityMeasurementEntity("CONVERT", q1, result));
            return result;
        }
        catch (System.Exception ex) when (ex is not QuantityMeasurementException)
        {
            _repository.Save(new QuantityMeasurementEntity("CONVERT", q1, null, ex.Message));
            throw new QuantityMeasurementException($"Convert failed: {ex.Message}", ex);
        }
    }

    // ── ADD ──────────────────────────────────────────────────────────

    public QuantityDTO Add(QuantityDTO q1, QuantityDTO q2)
    {
        ValidateNotNull(q1, q2);
        ValidateSameCategory(q1, q2, "Add");
        try
        {
            var result = ArithmeticByCategory(q1, q2, "ADD");
            _repository.Save(new QuantityMeasurementEntity("ADD", q1, q2, result));
            return result;
        }
        catch (NotSupportedException)
        {
            string msg = "Temperature does not support Add.";
            _repository.Save(new QuantityMeasurementEntity("ADD", q1, q2, msg));
            throw new QuantityMeasurementException(msg);
        }
        catch (System.Exception ex) when (ex is not QuantityMeasurementException)
        {
            _repository.Save(new QuantityMeasurementEntity("ADD", q1, q2, ex.Message));
            throw new QuantityMeasurementException($"Add failed: {ex.Message}", ex);
        }
    }

    // ── SUBTRACT ─────────────────────────────────────────────────────

    public QuantityDTO Subtract(QuantityDTO q1, QuantityDTO q2)
    {
        ValidateNotNull(q1, q2);
        ValidateSameCategory(q1, q2, "Subtract");
        try
        {
            var result = ArithmeticByCategory(q1, q2, "SUBTRACT");
            _repository.Save(new QuantityMeasurementEntity("SUBTRACT", q1, q2, result));
            return result;
        }
        catch (NotSupportedException)
        {
            string msg = "Temperature does not support Subtract.";
            _repository.Save(new QuantityMeasurementEntity("SUBTRACT", q1, q2, msg));
            throw new QuantityMeasurementException(msg);
        }
        catch (System.Exception ex) when (ex is not QuantityMeasurementException)
        {
            _repository.Save(new QuantityMeasurementEntity("SUBTRACT", q1, q2, ex.Message));
            throw new QuantityMeasurementException($"Subtract failed: {ex.Message}", ex);
        }
    }

    // ── DIVIDE ───────────────────────────────────────────────────────

    public QuantityDTO Divide(QuantityDTO q1, QuantityDTO q2)
    {
        ValidateNotNull(q1, q2);
        ValidateSameCategory(q1, q2, "Divide");
        try
        {
            var result = ArithmeticByCategory(q1, q2, "DIVIDE");
            _repository.Save(new QuantityMeasurementEntity("DIVIDE", q1, q2, result));
            return result;
        }
        catch (NotSupportedException)
        {
            string msg = "Temperature does not support Divide.";
            _repository.Save(new QuantityMeasurementEntity("DIVIDE", q1, q2, msg));
            throw new QuantityMeasurementException(msg);
        }
        catch (System.Exception ex) when (ex is not QuantityMeasurementException)
        {
            _repository.Save(new QuantityMeasurementEntity("DIVIDE", q1, q2, ex.Message));
            throw new QuantityMeasurementException($"Divide failed: {ex.Message}", ex);
        }
    }

    // ── PRIVATE HELPERS ──────────────────────────────────────────────

    private bool CompareByCategory(QuantityDTO q1, QuantityDTO q2)
        => q1.Category switch
        {
            "LENGTH"      => BuildLength(q1).Equals(BuildLength(q2)),
            "WEIGHT"      => BuildWeight(q1).Equals(BuildWeight(q2)),
            "VOLUME"      => BuildVolume(q1).Equals(BuildVolume(q2)),
            "TEMPERATURE" => BuildTemperature(q1).Equals(BuildTemperature(q2)),
            _ => throw new QuantityMeasurementException($"Unknown category: {q1.Category}")
        };

    private QuantityDTO ConvertByCategory(QuantityDTO q1, string targetUnitName)
        => q1.Category switch
        {
            "LENGTH"      => ToDTO(BuildLength(q1).ConvertTo(ResolveLength(targetUnitName)),           "LENGTH"),
            "WEIGHT"      => ToDTO(BuildWeight(q1).ConvertTo(ResolveWeight(targetUnitName)),           "WEIGHT"),
            "VOLUME"      => ToDTO(BuildVolume(q1).ConvertTo(ResolveVolume(targetUnitName)),           "VOLUME"),
            "TEMPERATURE" => ToDTO(BuildTemperature(q1).ConvertTo(ResolveTemperature(targetUnitName)),"TEMPERATURE"),
            _ => throw new QuantityMeasurementException($"Unknown category: {q1.Category}")
        };

    private QuantityDTO ArithmeticByCategory(QuantityDTO q1, QuantityDTO q2, string op)
        => q1.Category switch
        {
            "LENGTH"      => ApplyOp(BuildLength(q1),      BuildLength(q2),      op, "LENGTH"),
            "WEIGHT"      => ApplyOp(BuildWeight(q1),      BuildWeight(q2),      op, "WEIGHT"),
            "VOLUME"      => ApplyOp(BuildVolume(q1),      BuildVolume(q2),      op, "VOLUME"),
            "TEMPERATURE" => throw new NotSupportedException("Temperature arithmetic not supported"),
            _ => throw new QuantityMeasurementException($"Unknown category: {q1.Category}")
        };

    private static QuantityDTO ApplyOp<U>(Quantity<U> a, Quantity<U> b, string op, string category)
        where U : Enum
        => op switch
        {
            "ADD"      => ToDTO(a.Add(b),      category),
            "SUBTRACT" => ToDTO(a.Subtract(b), category),
            "DIVIDE"   => new QuantityDTO(a.Divide(b), "RATIO", "SCALAR"),
            _ => throw new ArgumentException($"Unknown operation: {op}")
        };

    // ── Quantity builders ─────────────────────────────────────────────

    private static Quantity<LengthUnit>      BuildLength(QuantityDTO dto)      => new(dto.Value, ResolveLength(dto.UnitName));
    private static Quantity<WeightUnit>      BuildWeight(QuantityDTO dto)      => new(dto.Value, ResolveWeight(dto.UnitName));
    private static Quantity<VolumeUnit>      BuildVolume(QuantityDTO dto)      => new(dto.Value, ResolveVolume(dto.UnitName));
    private static Quantity<TemperatureUnit> BuildTemperature(QuantityDTO dto) => new(dto.Value, ResolveTemperature(dto.UnitName));

    private static QuantityDTO ToDTO<U>(Quantity<U> q, string category) where U : Enum
        => new(q.Value, q.Unit.ToString()!, category);

    // ── Unit resolvers — string → enum (matches test unit strings) ───

    private static LengthUnit ResolveLength(string name)
        => name.Trim().ToUpperInvariant() switch
        {
            "FEET" or "FOOT" or "FT"               => LengthUnit.Feet,
            "INCHES" or "INCH" or "IN"              => LengthUnit.Inch,
            "YARDS" or "YARD" or "YD"               => LengthUnit.Yard,
            "CENTIMETERS" or "CENTIMETER" or "CM"   => LengthUnit.Centimeter,
            _ => throw new QuantityMeasurementException($"Unknown length unit: '{name}'")
        };

    private static WeightUnit ResolveWeight(string name)
        => name.Trim().ToUpperInvariant() switch
        {
            "KILOGRAM" or "KILOGRAMS" or "KG" => WeightUnit.Kilogram,
            "GRAM" or "GRAMS" or "G"          => WeightUnit.Gram,
            "POUND" or "POUNDS" or "LB"       => WeightUnit.Pound,
            _ => throw new QuantityMeasurementException($"Unknown weight unit: '{name}'")
        };

    private static VolumeUnit ResolveVolume(string name)
        => name.Trim().ToUpperInvariant() switch
        {
            "LITRE" or "LITER" or "L"           => VolumeUnit.Litre,
            "MILLILITRE" or "MILLILITER" or "ML" => VolumeUnit.Millilitre,
            "GALLON" or "GAL"                    => VolumeUnit.Gallon,
            _ => throw new QuantityMeasurementException($"Unknown volume unit: '{name}'")
        };

    private static TemperatureUnit ResolveTemperature(string name)
        => name.Trim().ToUpperInvariant() switch
        {
            "CELSIUS" or "C"    => TemperatureUnit.Celsius,
            "FAHRENHEIT" or "F" => TemperatureUnit.Fahrenheit,
            "KELVIN" or "K"     => TemperatureUnit.Kelvin,
            _ => throw new QuantityMeasurementException($"Unknown temperature unit: '{name}'")
        };

    // ── Validation ────────────────────────────────────────────────────

    private static void ValidateNotNull(QuantityDTO? q1, QuantityDTO? q2)
    {
        if (q1 == null) throw new QuantityMeasurementException("First quantity cannot be null");
        if (q2 == null) throw new QuantityMeasurementException("Second quantity cannot be null");
    }

    private static void ValidateSameCategory(QuantityDTO q1, QuantityDTO q2, string op)
    {
        if (!string.Equals(q1.Category, q2.Category, StringComparison.OrdinalIgnoreCase))
            throw new QuantityMeasurementException(
                $"Cannot {op} across different categories: {q1.Category} and {q2.Category}");
    }
}
