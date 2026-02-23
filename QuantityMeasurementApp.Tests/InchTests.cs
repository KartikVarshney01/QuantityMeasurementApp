using QuantityMeasurementApp.Models;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class InchTests
{
    [TestMethod]
    public void testFeetEquality_SameValue_ReturnTrue()
    {
        // Arrange
        Inch firstInchObject = new Inch(1.0);
        Inch secondInchobject = new Inch(1.0);

        //Act
        bool result = firstInchObject.Equals(secondInchobject);

        //Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testFeetEquality_DifferentValue_ReturnFalse()
    {
        //Arrange
        Inch firstInchObject = new Inch(1.0);
        Inch secondInchObject = new Inch(2.0);
        
        //Act
        bool result = firstInchObject.Equals(secondInchObject);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testFeetEquality_NullComparison_ReturnFalse()
    {
        //Arrange
        Inch firstInchObject = new Inch(1.0);

        //Act
        bool result = firstInchObject.Equals(null);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testFeetEquality_NonNumericInput_ReturnFalse()
    {
        //Arrange
        Inch firstInchObject = new Inch(1.0);

        //Act
        bool result = firstInchObject.Equals("abc");

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testFeetEquality_SameReference_ReturnTrue()
    {
        //Arrange
        Inch firstInchObject = new Inch(1.0);

        //Act
        bool result = firstInchObject.Equals(firstInchObject);

        //Assert
        Assert.IsTrue(result);
    }
}
