using Application.DTOs;
using Application.QueryHandlers.Property;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using PropertyEntities = Domain.Entities;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.QueryHandlers.Property
{
    public class GetAllPropertiesQueryHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetAllPropertiesQueryHandler handler;

        public GetAllPropertiesQueryHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetAllPropertiesQueryHandler(propertyRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPropertiesExist()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var properties = new List<PropertyEntities.Property> { EntityFactory.CreateProperty(mockId) };
            var propertyDtos = properties.Select(EntityFactory.CreatePropertyDto).ToList();
            var paginatedProperties = EntityFactory.CreatePaginatedProperties(properties, 1, 10);

            propertyRepositoryMock.GetPropertiesAsync(1, 10, null).Returns(paginatedProperties);
            mapperMock.Map<PaginatedList<PropertyDto>>(paginatedProperties).Returns(new PaginatedList<PropertyDto>(propertyDtos, propertyDtos.Count, 1, 10));

            var query = EntityFactory.CreateGetAllPropertiesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess, $"Expected success but got failure with error message: {result.ErrorMessage}");
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Items);
            Assert.Equal(propertyDtos, result.Data.Items);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenNoPropertiesExist()
        {
            // Arrange
            propertyRepositoryMock.GetPropertiesAsync(1, 10, null).Returns((PaginatedList<PropertyEntities.Property>)null!);

            var query = EntityFactory.CreateGetAllPropertiesQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No properties!", result.ErrorMessage);
        }
    }
}
