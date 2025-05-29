using MediatR;
using Domain.Utils;

namespace Application.Commands.Inquiry
{
    public class DeleteInquiryCommand : IRequest<Result>
    {
        public Guid Id { get; set; }
    }
}
