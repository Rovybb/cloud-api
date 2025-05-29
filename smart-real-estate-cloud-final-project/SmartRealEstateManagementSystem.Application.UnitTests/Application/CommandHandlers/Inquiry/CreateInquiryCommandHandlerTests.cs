using Application.CommandHandlers.Inquiry;
using Application.Commands.Inquiry;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using NSubstitute;
using SmartRealEstateManagementSystem.Application.UnitTests.Utils;
using Xunit;

namespace SmartRealEstateManagementSystem.Application.UnitTests.Application.CommandHandlers.Inquiry
{
    public class CreateInquiryCommandHandlerTests
    {
        private readonly IInquiryRepository inquiryRepositoryMock;
        private readonly IUserInformationRepository userInformationRepositoryMock;
        private readonly IMapper mapperMock;
        private readonly IEmailService emailServiceMock;
        private readonly CreateInquiryCommandHandler handler;

        public CreateInquiryCommandHandlerTests()
        {
            inquiryRepositoryMock = Substitute.For<IInquiryRepository>();
            userInformationRepositoryMock = Substitute.For<IUserInformationRepository>();
            mapperMock = Substitute.For<IMapper>();
            emailServiceMock = Substitute.For<IEmailService>();
            handler = new CreateInquiryCommandHandler(inquiryRepositoryMock, mapperMock, emailServiceMock, userInformationRepositoryMock);
        }

        [Fact]
        public async Task Handle_ShouldReturnSuccessResult_WhenInquiryIsCreated()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateInquiryCommand(mockId);
            var inquiry = EntityFactory.CreateInquiry(mockId);

            mapperMock.Map<Domain.Entities.Inquiry>(command).Returns(inquiry);
            inquiryRepositoryMock.CreateAsync(inquiry).Returns(Result<Guid>.Success(mockId));
            userInformationRepositoryMock.GetByIdAsync(command.AgentId).Returns(Result<Domain.Entities.UserInformation>.Success(EntityFactory.CreateUserInformation(mockId, "agent", "agent@example.com", "Agent", "User", "123 Agent St", "123-456-7890", "Agent Nationality")));
            userInformationRepositoryMock.GetByIdAsync(command.ClientId).Returns(Result<Domain.Entities.UserInformation>.Success(EntityFactory.CreateUserInformation(mockId, "client", "client@example.com", "Client", "User", "456 Client St", "987-654-3210", "Client Nationality")));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(mockId, result.Data);
        }

        [Fact]
        public async Task Handle_ShouldReturnFailureResult_WhenInquiryCreationFails()
        {
            // Arrange
            var mockId = Guid.NewGuid();
            var command = EntityFactory.CreateInquiryCommand(mockId);
            var inquiry = EntityFactory.CreateInquiry(mockId);

            mapperMock.Map<Domain.Entities.Inquiry>(command).Returns(inquiry);
            inquiryRepositoryMock.CreateAsync(inquiry).Returns(Result<Guid>.Failure("Inquiry creation failed."));

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Inquiry creation failed.", result.ErrorMessage);
        }
    }
}

