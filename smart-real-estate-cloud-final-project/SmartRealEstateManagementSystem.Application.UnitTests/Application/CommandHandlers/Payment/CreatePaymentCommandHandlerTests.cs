using Application.CommandHandlers.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Payment
{
    public class CreatePaymentCommandHandlerTests
    {
        private readonly IPaymentRepository paymentRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly CreatePaymentCommandHandler handler;

        public CreatePaymentCommandHandlerTests()
        {
            paymentRepositoryMock = Substitute.For<IPaymentRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new CreatePaymentCommandHandler(paymentRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPaymentIsCreated()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreatePaymentCommand(mockId);
            var payment = EntityFactory.CreatePayment(mockId);

            mapperMock.Map<Domain.Entities.Payment>(command).Returns(payment);
            paymentRepositoryMock.CreateAsync(payment).Returns(Result<Guid>.Success(mockId));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mockId, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPaymentCreationFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreatePaymentCommand(mockId);
            var payment = EntityFactory.CreatePayment(mockId);

            mapperMock.Map<Domain.Entities.Payment>(command).Returns(payment);
            paymentRepositoryMock.CreateAsync(payment).Returns(Result<Guid>.Failure("Payment creation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Payment creation failed.", result.ErrorMessage);
        }
    }
}
