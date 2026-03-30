using QuantityMeasurementAppModelLayer.Enums;

namespace QuantityMeasurementAppBusinessLayer.Extensions;

public static class VolumeUnitExtension
{
    public static double GetConversionFactor(this VolumeUnit unit)
        => unit switch
        {
            VolumeUnit.Litre      => 1.0,
            VolumeUnit.Millilitre => 0.001,
            VolumeUnit.Gallon     => 3.78541,
            _                     => throw new ArgumentException("Invalid VolumeUnit")
        };

    public static double ConvertToBaseUnit(this VolumeUnit unit, double value)
        => value * unit.GetConversionFactor();

    public static double ConvertFromBaseUnit(this VolumeUnit unit, double baseValue)
        => baseValue / unit.GetConversionFactor();

    public static string GetUnitName(this VolumeUnit unit)
        => unit.ToString();
}
