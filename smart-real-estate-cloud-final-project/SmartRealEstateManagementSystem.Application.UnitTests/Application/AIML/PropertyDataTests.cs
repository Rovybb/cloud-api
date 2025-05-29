using Application.AIML;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.AIML
{
    public class PropertyDataTests
    {
        [Fact]
        public void PropertyData_ShouldSetProperties()
        {
            // Arrange
            var propertyData = new PropertyData
            {
                Price = 100000.0f,
                City = "Sample City",
                Location = "Sample Location",
                RoomsNr = 3,
                Surface = 1500.0f
            };

            // Assert
            Assert.Equal(100000.0f, propertyData.Price);
            Assert.Equal("Sample City", propertyData.City);
            Assert.Equal("Sample Location", propertyData.Location);
            Assert.Equal(3, propertyData.RoomsNr);
            Assert.Equal(1500.0f, propertyData.Surface);
        }
    }
}

