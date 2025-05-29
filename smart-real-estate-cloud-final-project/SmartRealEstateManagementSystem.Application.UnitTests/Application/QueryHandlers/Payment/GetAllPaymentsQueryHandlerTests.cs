using Application.DTOs;
using Application.QueryHandlers.Payment;
using AutoMapper;
using Domain.Repositories;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using PaymentEntities = Domain.Entities;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.QueryHandlers.Payment
{
    public class GetAllPaymentsQueryHandlerTests
    {
        private readonly IPaymentRepository paymentRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly GetAllPaymentsQueryHandler handler;

        public GetAllPaymentsQueryHandlerTests()
        {
            paymentRepositoryMock = Substitute.For<IPaymentRepository>();
            mapperMock = Substitute.For<IMapper>();
            handler = new GetAllPaymentsQueryHandler(paymentRepositoryMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenPaymentsExist()
        {
            // Arrange
            var mockId = Guid.Parse("a026c5ca-a4d4-4b2c-af7f-615c31e4adc1");
            var payment = EntityFactory.CreatePayment(mockId);
            var payments = new List<PaymentEntities.Payment> { payment };
            var paymentDtos = payments.Select(EntityFactory.CreatePaymentDto).ToList();

            paymentRepositoryMock.GetAllAsync().Returns(payments);
            mapperMock.Map<IEnumerable<PaymentDto>>(payments).Returns(paymentDtos);

            var query = EntityFactory.CreateGetAllPaymentsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(paymentDtos, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenNoPaymentsExist()
        {
            // Arrange
            paymentRepositoryMock.GetAllAsync().Returns((IEnumerable<PaymentEntities.Payment>)null!);

            var query = EntityFactory.CreateGetAllPaymentsQuery();

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("No payments found.", result.ErrorMessage);
        }
    }
}
