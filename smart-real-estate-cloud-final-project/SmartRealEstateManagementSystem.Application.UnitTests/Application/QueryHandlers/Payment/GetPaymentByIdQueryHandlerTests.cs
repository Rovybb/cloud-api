using Application.DTOs;
using Application.QueryHandlers.Payment;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using PaymentEntities = Domain.Entities;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.QueryHandlers.Payment
{
    public class GetPaymentByIdQueryHandlerTests
    {
        private readonly IPaymentRepository paymentRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetPaymentByIdQueryHandler handler;

        public GetPaymentByIdQueryHandlerTests()
        {
            paymentRepositoryMock = Substitute.For<IPaymentRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetPaymentByIdQueryHandler(paymentRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPaymentExists()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var payment = EntityFactory.CreatePayment(mockId);
            var paymentDto = EntityFactory.CreatePaymentDto(payment);

            paymentRepositoryMock.GetByIdAsync(payment.Id).Returns(Result<PaymentEntities.Payment>.Success(payment));
            mapperMock.Map<PaymentDto>(payment).Returns(paymentDto);

            var query = EntityFactory.CreateGetPaymentByIdQuery(payment.Id);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(paymentDto, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPaymentDoesNotExist()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            paymentRepositoryMock.GetByIdAsync(paymentId).Returns(Result<PaymentEntities.Payment>.Failure("Payment not found."));

            var query = EntityFactory.CreateGetPaymentByIdQuery(paymentId);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Payment not found.", result.ErrorMessage);
        }
    }
}
