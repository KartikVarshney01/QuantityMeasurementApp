using System;
using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp;

public class QuantityMeasurementAppMain
{
    public static void Main(string[] args)
    {
        try
        {
            Console.Write("Enter first value in feet: ");
            double feetValue1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter second value in feet: ");
            double feetValue2 = Convert.ToDouble(Console.ReadLine());

            Feet firstFeet = new Feet(feetValue1);
            Feet secondFeet = new Feet(feetValue2);

            bool result = firstFeet.Equals(secondFeet);
            Console.WriteLine(result ? "Equals (true)" : "Not Equal (false)");
        }
        catch (FormatException)
        {
            Console.WriteLine("Invalid Value! Enter Numeric Values Only");
        }
    }
}