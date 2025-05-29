using Application.Commands.Property;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Property
{
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, Result<Guid>>
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IMapper mapper;

        public CreatePropertyCommandHandler(IPropertyRepository propertyRepository, IMapper mapper)
        {
            this.propertyRepository = propertyRepository;
            this.mapper = mapper;
        }

        public async Task<Result<Guid>> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            var result = await propertyRepository.CreateAsync(mapper.Map<Domain.Entities.Property>(request));
            if (result.IsSuccess)
            {
                return Result<Guid>.Success(result.Data);
            }
            return Result<Guid>.Failure(result.ErrorMessage );
        }
    }   
}
