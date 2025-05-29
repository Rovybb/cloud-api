using Application.CommandHandlers.Inquiry;
using Application.Commands.Inquiry;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Inquiry
{
    public class UpdateInquiryCommandHandlerTests
    {
        private readonly IInquiryRepository inquiryRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly UpdateInquiryCommandHandler handler;

        public UpdateInquiryCommandHandlerTests()
        {
            inquiryRepositoryMock = Substitute.For<IInquiryRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new UpdateInquiryCommandHandler(inquiryRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenInquiryIsUpdated()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUpdateInquiryCommand(mockId);
            var inquiry = EntityFactory.CreateInquiry(mockId);

            inquiryRepositoryMock.GetByIdAsync(mockId).Returns(Result<Domain.Entities.Inquiry>.Success(inquiry));
            inquiryRepositoryMock.UpdateAsync(inquiry).Returns(Result.Success());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenInquiryNotFound()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUpdateInquiryCommand(mockId);

            inquiryRepositoryMock.GetByIdAsync(mockId).Returns(Result<Domain.Entities.Inquiry>.Failure("Inquiry not found."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Inquiry not found.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenInquiryUpdateFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateUpdateInquiryCommand(mockId);
            var inquiry = EntityFactory.CreateInquiry(mockId);

            inquiryRepositoryMock.GetByIdAsync(mockId).Returns(Result<Domain.Entities.Inquiry>.Success(inquiry));
            inquiryRepositoryMock.UpdateAsync(inquiry).Returns(Result.Failure("Inquiry update failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Inquiry update failed.", result.ErrorMessage);
        }
    }
}

