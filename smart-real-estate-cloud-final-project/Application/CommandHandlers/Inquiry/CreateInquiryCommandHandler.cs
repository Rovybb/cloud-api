using Application.Commands.Inquiry;
using Application.Interfaces;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Inquiry
{
    public class CreateInquiryCommandHandler : IRequestHandler<CreateInquiryCommand, Result<Guid>>
    {
        private readonly IInquiryRepository inquiryRepository;
        private readonly IUserInformationRepository userInformationRepository;
        private readonly IMapper mapper;
        private readonly IEmailService emailService;

        public CreateInquiryCommandHandler(
            IInquiryRepository inquiryRepository,
            IMapper mapper,
            IEmailService emailService,
            IUserInformationRepository userInformationRepository)
        {
            this.inquiryRepository = inquiryRepository;
            this.mapper = mapper;
            this.emailService = emailService;
            this.userInformationRepository = userInformationRepository;
        }

        public async Task<Result<Guid>> Handle(CreateInquiryCommand request, CancellationToken cancellationToken)
        {
            var result = await inquiryRepository.CreateAsync(mapper.Map<Domain.Entities.Inquiry>(request));

            if (result.IsSuccess)
            {
                var agentResult = await userInformationRepository.GetByIdAsync(request.AgentId);
                var clientResult = await userInformationRepository.GetByIdAsync(request.ClientId);

                if (agentResult.IsSuccess && clientResult.IsSuccess)
                {
                    var agent = agentResult.Data;
                    var client = clientResult.Data;

                    var plainTextContent = GetEmailPlainText(agent, client, request);
                    var htmlContent = GetEmailHtmlContent(agent, client, request);

                    await emailService.SendEmailAsync(
                        agent.Email,
                        "New Inquiry",
                        plainTextContent,
                        htmlContent
                    );
                }

                return Result<Guid>.Success(result.Data);
            }

            return Result<Guid>.Failure(result.ErrorMessage);
        }

        private string GetEmailPlainText(Domain.Entities.UserInformation agent, Domain.Entities.UserInformation client, CreateInquiryCommand request)
        {
            return $@"
Dear {agent.FirstName} {agent.LastName},

You have received an inquiry from {client.FirstName} {client.LastName} (Email: {client.Email}).

Message: {request.Message}

Please respond to the inquiry as soon as possible.

Best regards,
Smart Real Estate Management System
";
        }

        private string GetEmailHtmlContent(Domain.Entities.UserInformation agent, Domain.Entities.UserInformation client, CreateInquiryCommand request)
        {
            var propertyUrl = $"https://localhost:4200/properties/property-details/{request.PropertyId}";

            return $@"
<p>Dear {agent.FirstName} {agent.LastName},</p>
<p>
    You have received an inquiry from <strong>{client.FirstName} {client.LastName}</strong> 
    (Email: <strong>{client.Email}</strong>).
</p>
<p><strong>Message:</strong> {request.Message}</p>
<p>
    <strong>Property reference:</strong><br />
    <a href=""{propertyUrl}"" 
       style=""display:inline-block; padding:10px 20px; background-color:#007BFF; color:#ffffff; text-decoration:none; 
              border-radius:5px; font-weight:bold;"">
        See property
    </a>
</p>
<p>Please respond to the inquiry as soon as possible.</p>
<p>Best regards,<br />
Smart Real Estate Management System</p>
";
        }
    }
}
