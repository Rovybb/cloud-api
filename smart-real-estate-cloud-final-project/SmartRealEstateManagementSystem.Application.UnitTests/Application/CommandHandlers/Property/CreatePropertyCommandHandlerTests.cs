using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Application.CommandHandlers.Property;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using PropertyEntities = Domain.Entities;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Property
{
    public class CreatePropertyCommandHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly CreatePropertyCommandHandler handler;

        public CreatePropertyCommandHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new CreatePropertyCommandHandler(propertyRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPropertyIsCreated()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var command = EntityFactory.CreatePropertyCommand(mockId);
            var property = EntityFactory.CreateProperty(mockId);

            mapperMock.Map<PropertyEntities.Property>(command).Returns(property);
            propertyRepositoryMock.CreateAsync(property).Returns(Result<Guid>.Success(property.Id));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(property.Id, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPropertyCreationFails()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var command = EntityFactory.CreatePropertyCommand(mockId);
            var property = EntityFactory.CreateProperty(mockId);

            mapperMock.Map<PropertyEntities.Property>(command).Returns(property);
            propertyRepositoryMock.CreateAsync(property).Returns(Result<Guid>.Failure("Creation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Creation failed.", result.ErrorMessage);
        }
    }
}