using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.CommandHandlers.Payment;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Payment
{
    public class CreateCheckoutCommandHandlerTests
    {
        private readonly IPaymentRepository paymentRepositoryMock;
        private readonly ICheckoutService checkoutServiceMock;
        private readonly IMapper mapperMock;
        private readonly CreateStripeCheckoutCommandHandler handler;

        public CreateCheckoutCommandHandlerTests()
        {
            paymentRepositoryMock = Substitute.For<IPaymentRepository>();
            checkoutServiceMock = Substitute.For<ICheckoutService>();
            mapperMock = Substitute.For<IMapper>();
            handler = new CreateStripeCheckoutCommandHandler(paymentRepositoryMock, checkoutServiceMock, mapperMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenCheckoutSessionIsCreated()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateCheckoutCommand(mockId);
            var payment = EntityFactory.CreatePayment(mockId);
            var checkoutUrl = "https://checkout.stripe.com/pay/success";

            mapperMock.Map<Domain.Entities.Payment>(command).Returns(payment);
            paymentRepositoryMock.CreateAsync(payment).Returns(Result<Guid>.Success(mockId));
            checkoutServiceMock.CreateCheckoutSessionAsync(command.Price, "usd", command.SuccessUrl, command.CancelUrl).Returns(checkoutUrl);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(checkoutUrl, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenPaymentCreationFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateCheckoutCommand(mockId);
            var payment = EntityFactory.CreatePayment(mockId);

            mapperMock.Map<Domain.Entities.Payment>(command).Returns(payment);
            paymentRepositoryMock.CreateAsync(payment).Returns(Result<Guid>.Failure("Payment creation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Payment creation failed.", result.ErrorMessage);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenCheckoutSessionCreationFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateCheckoutCommand(mockId);
            var payment = EntityFactory.CreatePayment(mockId);

            mapperMock.Map<Domain.Entities.Payment>(command).Returns(payment);
            paymentRepositoryMock.CreateAsync(payment).Returns(Result<Guid>.Success(mockId));
            checkoutServiceMock.CreateCheckoutSessionAsync(command.Price, "usd", command.SuccessUrl, command.CancelUrl).Throws(new Exception("Checkout session creation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Checkout session creation failed.", result.ErrorMessage);
        }
    }
}
