using Application.CommandHandlers.Inquiry;
using Application.Commands.Inquiry;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Inquiry
{
    public class DeleteInquiryCommandHandlerTests
    {
        private readonly IInquiryRepository inquiryRepositoryMock;
        private readonly DeleteInquiryCommandHandler handler;

        public DeleteInquiryCommandHandlerTests()
        {
            inquiryRepositoryMock = Substitute.For<IInquiryRepository>();
            handler = new DeleteInquiryCommandHandler(inquiryRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenInquiryIsDeleted()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeleteInquiryCommand { Id = mockId };

            inquiryRepositoryMock.DeleteAsync(mockId).Returns(Result.Success());

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenInquiryDeletionFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = new DeleteInquiryCommand { Id = mockId };

            inquiryRepositoryMock.DeleteAsync(mockId).Returns(Result.Failure("Inquiry deletion failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Inquiry deletion failed.", result.ErrorMessage);
        }
    }
}

