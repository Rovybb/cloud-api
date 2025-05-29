using Application.CommandHandlers.Property;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using PropertyEntities = Domain.Entities;


namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Property
{
    public class DeletePropertyCommandHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly DeletePropertyCommandHandler handler;

        public DeletePropertyCommandHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            handler = new DeletePropertyCommandHandler(propertyRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPropertyIsDeleted()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var property = EntityFactory.CreateProperty(mockId);

            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<PropertyEntities.Property>.Success(property));
            propertyRepositoryMock.DeleteAsync(mockId).Returns(Result.Success());

            var command = EntityFactory.CreateDeletePropertyCommand(mockId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPropertyDoesNotExist()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<PropertyEntities.Property>.Failure("Property not found."));

            var command = EntityFactory.CreateDeletePropertyCommand(mockId);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Property not found.", result.ErrorMessage);
        }
    }
}
