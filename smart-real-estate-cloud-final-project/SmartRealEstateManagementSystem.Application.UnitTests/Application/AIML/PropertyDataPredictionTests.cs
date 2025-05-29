using Application.AIML;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.AIML
{
    public class PropertyDataPredictionTests
    {
        [Fact]
        public void PropertyDataPrediction_ShouldSetPredictedPrice()
        {
            // Arrange
            var prediction = new PropertyDataPrediction
            {
                PredictedPrice = 1000.0f
            };

            // Assert
            Assert.Equal(1000.0f, prediction.PredictedPrice);
        }
    }
}

