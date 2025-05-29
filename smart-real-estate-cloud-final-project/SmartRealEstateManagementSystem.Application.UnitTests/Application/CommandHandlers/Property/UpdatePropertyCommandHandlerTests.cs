using Application.CommandHandlers.Property;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using PropertyEntities = Domain.Entities;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Property
{
    public class UpdatePropertyCommandHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly UpdatePropertyCommandHandler handler;

        public UpdatePropertyCommandHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new UpdatePropertyCommandHandler(propertyRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPropertyIsUpdated()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var command = EntityFactory.CreateUpdatePropertyCommand(mockId);
            var existingProperty = EntityFactory.CreateProperty(mockId);

            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<PropertyEntities.Property>.Success(existingProperty));
            propertyRepositoryMock.UpdateAsync(existingProperty).Returns(Result.Success());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPropertyIsNotFound()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var command = EntityFactory.CreateUpdatePropertyCommand(mockId);

            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<PropertyEntities.Property>.Failure("Property not found."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Property not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenUpdateFails()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var command = EntityFactory.CreateUpdatePropertyCommand(mockId);
            var existingProperty = EntityFactory.CreateProperty(mockId);

            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<PropertyEntities.Property>.Success(existingProperty));
            propertyRepositoryMock.UpdateAsync(existingProperty).Returns(Result.Failure("Update failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Update failed.", result.ErrorMessage);
        }
    }
}
