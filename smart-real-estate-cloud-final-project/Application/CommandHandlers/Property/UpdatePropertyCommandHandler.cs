using Application.Commands.Property;
using AutoMapper;
using Domain.Repositories;
using Domain.Utils;
using MediatR;

namespace Application.CommandHandlers.Property
{
    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, Result>
    {
        private readonly IPropertyRepository propertyRepository;
        private readonly IMapper mapper;

        public UpdatePropertyCommandHandler(IPropertyRepository propertyRepository, IMapper mapper)
        {
            this.propertyRepository = propertyRepository;
            this.mapper = mapper;
        }

        public async Task<Result> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            var propertyResult = await propertyRepository.GetByIdAsync(request.Id);
            if (!propertyResult.IsSuccess)
            {
                return Result.Failure(propertyResult.ErrorMessage );
            }

            var existingProperty = propertyResult.Data;
            mapper.Map(request.Request, existingProperty);
            existingProperty.UpdatedAt = DateTime.UtcNow;

            var updateResult = await propertyRepository.UpdateAsync(existingProperty);
            if (updateResult.IsSuccess)
            {
                return Result.Success();
            }

            return Result.Failure(updateResult.ErrorMessage );
        }
    }
}
