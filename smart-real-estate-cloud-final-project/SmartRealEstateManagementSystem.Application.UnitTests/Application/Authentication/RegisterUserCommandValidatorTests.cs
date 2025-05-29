using Application.Commands.User;
using FluentValidation.TestHelper;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.Authentication
{
    public class RegisterUserCommandValidatorTests
    {
        private readonly RegisterUserCommandValidator validator;

        public RegisterUserCommandValidatorTests()
        {
            validator = new RegisterUserCommandValidator();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var command = new RegisterUserCommand { Email = "", Password = "ValidPassword123" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid()
        {
            var command = new RegisterUserCommand { Email = "invalid-email", Password = "ValidPassword123" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Email);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            var command = new RegisterUserCommand { Email = "user@example.com", Password = "" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Have_Error_When_Password_Is_Too_Short()
        {
            var command = new RegisterUserCommand { Email = "user@example.com", Password = "short" };
            var result = validator.TestValidate(command);
            result.ShouldHaveValidationErrorFor(x => x.Password);
        }

        [Fact]
        public void Should_Not_Have_Error_When_Command_Is_Valid()
        {
            var command = new RegisterUserCommand { Email = "user@example.com", Password = "ValidPassword123" };
            var result = validator.TestValidate(command);
            result.ShouldNotHaveValidationErrorFor(x => x.Email);
            result.ShouldNotHaveValidationErrorFor(x => x.Password);
        }
    }
}


