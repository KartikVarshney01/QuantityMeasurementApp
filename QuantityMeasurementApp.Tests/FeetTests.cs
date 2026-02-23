using QuantityMeasurementApp.Models;
namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class FeetTests
{
    [TestMethod]
    public void testFeetEquality_SameValue_ReturnTrue()
    {
        // Arrange
        Feet firstFeetObject = new Feet(1.0);
        Feet secondFeetobject = new Feet(1.0);

        //Act
        bool result = firstFeetObject.Equals(secondFeetobject);

        //Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testFeetEquality_DifferentValue_ReturnFalse()
    {
        //Arrange
        Feet firstFeetObject = new Feet(1.0);
        Feet secondFeetObject = new Feet(2.0);
        
        //Act
        bool result = firstFeetObject.Equals(secondFeetObject);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testFeetEquality_NullComparison_ReturnFalse()
    {
        //Arrange
        Feet firstFeetObject = new Feet(1.0);

        //Act
        bool result = firstFeetObject.Equals(null);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testFeetEquality_NonNumericInput_ReturnFalse()
    {
        //Arrange
        Feet firstFeetObject = new Feet(1.0);

        //Act
        bool result = firstFeetObject.Equals("abc");

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testFeetEquality_SameReference_ReturnTrue()
    {
        //Arrange
        Feet firstFeetObject = new Feet(1.0);

        //Act
        bool result = firstFeetObject.Equals(firstFeetObject);

        //Assert
        Assert.IsTrue(result);
    }
}
