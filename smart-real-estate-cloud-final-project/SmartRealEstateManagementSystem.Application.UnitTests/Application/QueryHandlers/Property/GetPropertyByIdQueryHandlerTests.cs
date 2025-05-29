using Application.DTOs;
using Application.QueryHandlers.Property;
using AutoMapper;
using Domain.Repositories;
using NSubstitute;
using PropertyEntities = Domain.Entities;
using Domain.Utils;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.QueryHandlers.Property
{
    public class GetPropertyByIdQueryHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetPropertyByIdQueryHandler handler;

        public GetPropertyByIdQueryHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetPropertyByIdQueryHandler(propertyRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPropertyExists()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var property = EntityFactory.CreateProperty(mockId);
            var propertyDto = EntityFactory.CreatePropertyDto(property);

            propertyRepositoryMock.GetByIdAsync(property.Id).Returns(Result<PropertyEntities.Property>.Success(property));
            mapperMock.Map<PropertyDto>(property).Returns(propertyDto);

            var query = EntityFactory.CreateGetPropertyByIdQuery(property.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(propertyDto, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPropertyDoesNotExist()
        {
            // Arrange
            var propertyId = Guid.NewGuid();
            propertyRepositoryMock.GetByIdAsync(propertyId).Returns(Result<PropertyEntities.Property>.Failure("Property not found."));

            var query = EntityFactory.CreateGetPropertyByIdQuery(propertyId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Property not found.", result.ErrorMessage);
        }
    }
}
