using Application.CommandHandlers.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Payment
{
    public class UpdatePaymentCommandHandlerTests
    {
        private readonly IPaymentRepository paymentRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly UpdatePaymentCommandHandler handler;

        public UpdatePaymentCommandHandlerTests()
        {
            paymentRepositoryMock = Substitute.For<IPaymentRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new UpdatePaymentCommandHandler(paymentRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPaymentIsUpdated()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var updatePaymentCommand = EntityFactory.CreateUpdatePaymentCommand(mockId);
            var payment = EntityFactory.CreatePayment(mockId);

            paymentRepositoryMock.GetByIdAsync(Arg.Any<Guid>()).Returns(Result<Domain.Entities.Payment>.Success(payment));
            paymentRepositoryMock.UpdateAsync(payment).Returns(Result<Guid>.Success(mockId));
            mapperMock.Map(updatePaymentCommand, payment).Returns(payment);

            // Act
            var result = await handler.Handle(updatePaymentCommand, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPaymentIsNotUpdated()
        {
            // Arrange
            var updatePaymentCommand = EntityFactory.CreateUpdatePaymentCommand(Guid.NewGuid());

            paymentRepositoryMock.GetByIdAsync(Arg.Any<Guid>()).Returns(Result<Domain.Entities.Payment>.Failure("Payment not found."));

            // Act
            var result = await handler.Handle(updatePaymentCommand, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Payment not found.", result.ErrorMessage);
        }
    }
}