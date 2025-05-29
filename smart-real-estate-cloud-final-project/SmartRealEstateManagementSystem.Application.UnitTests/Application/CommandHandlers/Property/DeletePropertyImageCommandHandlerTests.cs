using Application.CommandHandlers.Property;
using Application.Commands.Property;
using Application.Interfaces;
using Domain.Entities;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Property
{
    public class DeletePropertyImageCommandHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly IImageStorageService imageStorageServiceMock;
        private readonly DeletePropertyImageCommandHandler handler;

        public DeletePropertyImageCommandHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            imageStorageServiceMock = Substitute.For<IImageStorageService>();
            handler = new DeletePropertyImageCommandHandler(propertyRepositoryMock, imageStorageServiceMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenImageIsDeleted()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeletePropertyImageCommand { Id = mockId };
            var propertyImage = new PropertyImage { Id = mockId, Url = "https://example.com/image.jpg" };

            propertyRepositoryMock.GetImageByIdAsync(mockId).Returns(Result<PropertyImage>.Success(propertyImage));
            imageStorageServiceMock.DeleteAsync(propertyImage.Url).Returns(Result.Success());
            propertyRepositoryMock.RemoveImageAsync(mockId).Returns(Result.Success());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenImageNotFound()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeletePropertyImageCommand { Id = mockId };

            propertyRepositoryMock.GetImageByIdAsync(mockId).Returns(Result<PropertyImage>.Failure("Image not found."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Image not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenImageDeletionFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeletePropertyImageCommand { Id = mockId };
            var propertyImage = new PropertyImage { Id = mockId, Url = "https://example.com/image.jpg" };

            propertyRepositoryMock.GetImageByIdAsync(mockId).Returns(Result<PropertyImage>.Success(propertyImage));
            imageStorageServiceMock.DeleteAsync(propertyImage.Url).Returns(Result.Failure("Failed to delete image from storage."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to delete image from storage.", result.ErrorMessage);
        }
    }
}
