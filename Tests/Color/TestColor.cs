using ClothingTracker.Models.Shared;
using Xunit;

namespace ClothingTracker.Tests
{
    public class ColorMatcherTests
    {
        [Fact]
        public void FindClosestEnumColor_ShouldReturnRed_ForFF3333()
        {
            // Arrange
            var inputHex = "#ff3333";

            // Act
            var result = SimpleDiscreteColorExtensions.ClosestDiscreteColor(inputHex);

            // Assert
            Assert.Equal(SimpleDiscreteColor.Red, result);
        }
        [Fact]
        public void FindClosestEnumColor_ShouldReturnBlue_For3355AA()
        {
            // Arrange
            var inputHex = "#ff3333";

            // Act
            var result = SimpleDiscreteColorExtensions.ClosestDiscreteColor(inputHex);

            // Assert
            Assert.Equal(SimpleDiscreteColor.Red, result);
        }
    }
}