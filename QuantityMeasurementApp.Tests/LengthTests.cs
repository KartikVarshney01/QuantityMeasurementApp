namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class LengthTests
{
    [TestMethod]
    public void testEquality_FeetToFeet_SameValue_ReturnTrue()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Feet, 1.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testEquality_InchToInch_SameValue_ReturnTrue()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Inch, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Inch, 1.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testEquality_FeetToInch_EquivalentValue_ReturnsTrue()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Inch, 12.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testEquality_InchToFeet_EquivalentValue_ReturnTrue()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Inch, 12.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Feet, 1.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testEquality_FeetToFeet_DifferentValue_ReturnsFalse()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Feet, 2.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testEquality_InchToInch_DifferentValue_ReturnsFalse()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Inch, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Inch, 2.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testEquality_NullComparison_ReturnsFalse()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 1.0);

        // Act
        bool result = lengthFirst.Equals(null);

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testEquality_InvalidUnit_ThrowsException()
    {
        Assert.Throws<Exception>(() =>
        {
            var invalid = new Quantity((LengthUnit)10, 5.0);
        });
    }

    [TestMethod]
    public void testEquality_SameReference_ReturnsTrue()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 1.0);

        // Act
        bool result = lengthFirst.Equals(lengthFirst);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testEquality_NonLengthObject_ReturnsFalse()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 1.0);

        // Act
        bool result = lengthFirst.Equals("abc");

        // Assert
        Assert.IsFalse(result);
    }
}
