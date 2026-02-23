using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace QuantityMeasurementApp.Tests;

[TestClass]
public sealed class LengthTests
{
    [TestMethod]
    public void testEquality_YardToYard_SameValue_ReturnTrue()
    {
        // Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Yard, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Yard, 1.0);

        // Act
        bool result = lengthFirst.Equals(lengthSecond);

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void testEquality_YardToYard_DifferentValue_ReturnFalse()
    {
        //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Yard, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Yard, 2.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void testEquality_YardToFeet_EquivalentValue_ReturnTrue()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Yard, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Feet, 3.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_FeetToYard_EquivalentValue_ReturnTrue()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Feet, 3.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Yard, 1.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_YardToInches_EquivalentValue_ReturnTrue()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Yard, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Inch, 36.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_InchesToYard_EquivalentValue_ReturnTrue()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Inch, 36.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Yard, 1.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_YardToFeet_NonEquivalentValue_ReturnFalse()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Yard, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Feet, 2.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsFalse(result);
    } 

    [TestMethod]
    public void testEquality_centimetersToInches_EquivalentValue_ReturnTrue()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Centimeter, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Inch, 0.393701);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_centimetersToFeet_NonEquivalentValue_ReturnFalse()
    {
         //Arrange
        Quantity lengthFirst = new Quantity(LengthUnit.Centimeter, 1.0);
        Quantity lengthSecond = new Quantity(LengthUnit.Feet, 1.0);

        //Act
        bool result = lengthFirst.Equals(lengthSecond);

        //Assert
        Assert.IsFalse(result);
    } 

    [TestMethod]
    public void testEquality_MultiUnit_TransitiveProperty_ReturnTrue()
    {
        //Arrange
        Quantity yard = new Quantity(LengthUnit.Yard, 1.0);
        Quantity feet = new Quantity(LengthUnit.Feet, 3.0);
        Quantity inches = new Quantity(LengthUnit.Inch, 36.0);

        //Act
        bool check = yard.Equals(feet) && feet.Equals(inches) && yard.Equals(inches);

        //Assert
        Assert.IsTrue(check);

    }

    [TestMethod]
    public void testEquality_YardWithNullUnit_ReturnFalse()
    {
         //Arrange
        Quantity yard = new Quantity(LengthUnit.Yard, 1.0);
        Quantity nullunit = new Quantity(null, 1.0);

        //Act
        bool result = yard.Equals(nullunit);

        //Assert
        Assert.IsFalse(result);
    } 

    [TestMethod]
    public void testEquality_YardSameReference_ReturnTrue()
    {
         //Arrange
        Quantity yard = new Quantity(LengthUnit.Yard, 1.0);

        //Act
        bool result = yard.Equals(yard);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_YardNullComparison_ReturnFalse()
    {
         //Arrange
        Quantity yard = new Quantity(LengthUnit.Yard, 1.0);

        //Act
        bool result = yard.Equals(null);

        //Assert
        Assert.IsFalse(result);
    } 

    [TestMethod]
    public void testEquality_CentimetersWithNullUnit_ReturnFalse()
    {
         //Arrange
        Quantity centimeter = new Quantity(LengthUnit.Centimeter, 1.0);
        Quantity nullunit = new Quantity(null, 1.0);
        
        //Act
        bool result = centimeter.Equals(null);

        //Assert
        Assert.IsFalse(result);
    } 

    [TestMethod]
    public void testEquality_CentimetersSameReference_ReturnTrue()
    {
         //Arrange
        Quantity Centimeter = new Quantity(LengthUnit.Centimeter, 1.0);

        //Act
        bool result = Centimeter.Equals(Centimeter);

        //Assert
        Assert.IsTrue(result);
    } 

    [TestMethod]
    public void testEquality_CentimetersNullComparison_ReturnFalse()
    {
         //Arrange
        Quantity centimeter = new Quantity(LengthUnit.Centimeter, 1.0);

        //Act
        bool result = centimeter.Equals(null);

        //Assert
        Assert.IsFalse(result);
    } 

    [TestMethod]
    public void testEquality_AllUnits_ComplexScenario_ReturnTrue()
    {
        //Arrange
        Quantity yard = new Quantity(LengthUnit.Yard, 2.0);
        Quantity feet = new Quantity(LengthUnit.Feet, 6.0);
        Quantity inches = new Quantity(LengthUnit.Inch, 72.0);

        //Act
        bool check = yard.Equals(feet) && feet.Equals(inches) && yard.Equals(inches);

        //Assert
        Assert.IsTrue(check);

    }
}
