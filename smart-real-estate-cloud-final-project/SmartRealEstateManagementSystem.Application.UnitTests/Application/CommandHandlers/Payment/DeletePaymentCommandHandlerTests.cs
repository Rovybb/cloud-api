using Application.CommandHandlers.Payment;
using Application.Commands.Payment;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Payment
{
    public class DeletePaymentCommandHandlerTests
    {
        private readonly IPaymentRepository paymentRepositoryMock;
        private readonly DeletePaymentCommandHandler handler;

        public DeletePaymentCommandHandlerTests()
        {
            paymentRepositoryMock = Substitute.For<IPaymentRepository>();
            handler = new DeletePaymentCommandHandler(paymentRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPaymentIsDeleted()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var payment = EntityFactory.CreatePayment(mockId);
            paymentRepositoryMock.GetByIdAsync(mockId).Returns(Result<Domain.Entities.Payment>.Success(payment));
            paymentRepositoryMock.DeleteAsync(mockId).Returns(Result.Success());

            var command = new DeletePaymentCommand { Id = mockId };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            paymentRepositoryMock.GetByIdAsync(paymentId).Returns(Result<Domain.Entities.Payment>.Failure("Payment not found."));

            var command = new DeletePaymentCommand { Id = paymentId };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Payment not found.", result.ErrorMessage);
        }
    }
}
