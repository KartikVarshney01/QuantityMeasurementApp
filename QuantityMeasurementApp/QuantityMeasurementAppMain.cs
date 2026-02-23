using System;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp;

class QuantityMeasurementAppMain
{
    static void Main(string[] args)
    {
        // Hardcoded values
        CheckFeetEquality(1.0, 1.0);
        CheckInchEquality(1.0, 1.0);
    }

        // Separate method → reduces dependency on Main
    public static void CheckFeetEquality(double v1, double v2)
    {
        Feet firstFeetObject = new Feet(v1);
        Feet secondFeetObject = new Feet(v2);

        Console.WriteLine($"Feet: {v1} vs {v2} → {(firstFeetObject.Equals(secondFeetObject) ? "Equal" : "Not Equal")}");
    }

    public static void CheckInchEquality(double v1, double v2)
    {
        Inch firstInchObject = new Inch(v1);
        Inch secondInchObject = new Inch(v2);

        Console.WriteLine($"Inch: {v1} vs {v2} → {(firstInchObject.Equals(secondInchObject) ? "Equal" : "Not Equal")}");
    }
}