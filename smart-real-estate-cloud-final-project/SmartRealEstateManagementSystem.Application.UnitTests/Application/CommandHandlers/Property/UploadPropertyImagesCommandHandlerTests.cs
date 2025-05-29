using Application.CommandHandlers.Property;
using Application.Commands.Property;
using Application.Interfaces;
using Domain.Repositories;
using Domain.Utils;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using System.Text.Json.Nodes;
using Domain.Entities;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Property
{
    public class UploadPropertyImagesCommandHandlerTests
    {
        private readonly IPropertyRepository propertyRepositoryMock;
        private readonly IImageStorageService imageStorageServiceMock;
        private readonly UploadPropertyImagesCommandHandler handler;

        public UploadPropertyImagesCommandHandlerTests()
        {
            propertyRepositoryMock = Substitute.For<IPropertyRepository>();
            imageStorageServiceMock = Substitute.For<IImageStorageService>();
            handler = new UploadPropertyImagesCommandHandler(propertyRepositoryMock, imageStorageServiceMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenImagesAreUploaded()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new UploadPropertyImagesCommand
            {
                PropertyId = mockId,
                Files = new List<IFormFile> { CreateMockFormFile() }
            };
            var property = EntityFactory.CreateProperty(mockId);
            var imageUrl = "https://example.com/image.jpg";

            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<Domain.Entities.Property>.Success(property));
            imageStorageServiceMock.UploadAsync(Arg.Any<IFormFile>(), Arg.Any<Guid>()).Returns(imageUrl);
            propertyRepositoryMock.AddImageAsync(mockId, Arg.Any<PropertyImage>()).Returns(Result.Success());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Single(result.Data);
        }

        //[Fact]
        //public async Task Handle_ShouldReturnFailureResult_WhenNoFilesToUpload()
        //{
        //    // Arrange
        //    var mockId = Guid.NewGuid();
        //    var command = new UploadPropertyImagesCommand
        //    {
        //        PropertyId = mockId,
        //        Files = new List<IFormFile>()
        //    };

        //    // Act
        //    var result = await handler.Handle(command, CancellationToken.None);

        //    // Assert
        //    Assert.False(result.IsSuccess);
        //    Assert.Equal("No files to upload.", result.ErrorMessage);
        //}

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenImageUploadFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new UploadPropertyImagesCommand
            {
                PropertyId = mockId,
                Files = new List<IFormFile> { CreateMockFormFile() }
            };
            var property = EntityFactory.CreateProperty(mockId);

            propertyRepositoryMock.GetByIdAsync(mockId).Returns(Result<Domain.Entities.Property>.Success(property));
            imageStorageServiceMock.UploadAsync(Arg.Any<IFormFile>(), Arg.Any<Guid>()).Returns((string)null);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Failed to upload image.", result.ErrorMessage);
        }

        private IFormFile CreateMockFormFile()
        {
            var fileMock = Substitute.For<IFormFile>();
            fileMock.Length.Returns(1);
            return fileMock;
        }
    }
}
