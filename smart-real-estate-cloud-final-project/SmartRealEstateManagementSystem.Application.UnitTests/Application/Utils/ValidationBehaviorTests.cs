using FluentValidation;
using MediatR;
using NSubstitute;
using Application.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.Utils
{
    public class ValidationBehaviorTests
    {
        private readonly IEnumerable<IValidator<TestRequest>> validators;
        private readonly ValidationBehavior<TestRequest, TestResponse> behavior;

        public ValidationBehaviorTests()
        {
            validators = new List<IValidator<TestRequest>> { new TestRequestValidator() };
            behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        }

        [Fact]
        public async Task Handle_ShouldCallNext_WhenValidationPasses()
        {
            // Arrange
            var request = new TestRequest { Name = "Valid Name" };
            var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
            next().Returns(new TestResponse());

            // Act
            var response = await behavior.Handle(request, next, CancellationToken.None);

            // Assert
            await next.Received(1)();
            Assert.NotNull(response);
        }

        [Fact]
        public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
        {
            // Arrange
            var request = new TestRequest { Name = "" };
            var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => behavior.Handle(request, next, CancellationToken.None));
            await next.DidNotReceive()();
        }
    }

    public class TestRequest : IRequest<TestResponse>
    {
        public string Name { get; set; }
    }

    public class TestResponse
    {
    }

    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        }
    }
}

