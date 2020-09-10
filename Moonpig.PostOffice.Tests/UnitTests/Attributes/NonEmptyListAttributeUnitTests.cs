namespace Moonpig.PostOffice.Tests.UnitTests.Attributes
{
    using System.Collections.Generic;
    using Moonpig.PostOffice.Api.Attributes;
    using Shouldly;
    using Xunit;

    public class NonEmptyListAttributeUnitTests
    {
        NonEmptyListAttribute attribute;
        public NonEmptyListAttributeUnitTests()
        {
            attribute = new NonEmptyListAttribute();
        }

        [Fact]
        public void Test_IsValid_ReturnsFalse_ForNullInput()
        {
            //Arrange

            //Act
            var isValid = attribute.IsValid(null);

            //Assert
            isValid.ShouldBeFalse();
        }

        [Fact]
        public void Test_IsValid_ReturnsFalse_ForEmptyListInput()
        {
            //Arrange

            //Act
            var isValid = attribute.IsValid(new List<int>());

            //Assert
            isValid.ShouldBeFalse();
        }

        [Fact]
        public void Test_IsValid_ReturnsTrue_ForNonEmptyListInput()
        {
            //Arrange

            //Act
            var isValid = attribute.IsValid(new List<int>(){1});

            //Assert
            isValid.ShouldBeFalse();
        }
    }
}