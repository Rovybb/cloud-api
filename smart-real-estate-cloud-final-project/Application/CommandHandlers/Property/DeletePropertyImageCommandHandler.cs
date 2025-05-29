using Application.Commands.Property;
using Domain.Repositories;
using Domain.Utils;
using MediatR;
using Application.Interfaces; // for IImageStorageService

namespace Application.CommandHandlers.Property
{
    public class DeletePropertyImageCommandHandler
        : IRequestHandler<DeletePropertyImageCommand, Result>
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IImageStorageService _imageStorageService;

        public DeletePropertyImageCommandHandler(
            IPropertyRepository propertyRepository,
            IImageStorageService imageStorageService)
        {
            _propertyRepository = propertyRepository;
            _imageStorageService = imageStorageService;
        }

        public async Task<Result> Handle(DeletePropertyImageCommand request, CancellationToken cancellationToken)
        {
            // 1. Get the property image from DB
            var propertyImageResult = await _propertyRepository.GetImageByIdAsync(request.Id);
            if (!propertyImageResult.IsSuccess)
            {
                return Result.Failure(propertyImageResult.ErrorMessage);
            }
            var propertyImage = propertyImageResult.Data;

            // 2. Remove image from Firebase (using the URL)
            //    This assumes your IImageStorageService has a DeleteAsync(string url) method
            var deleteResult = await _imageStorageService.DeleteAsync(propertyImage.Url);
            if (!deleteResult.IsSuccess)
            {
                // If cloud deletion fails, decide if you want to stop or still remove DB record
                return Result.Failure(deleteResult.ErrorMessage);
            }

            // 3. Remove image record from DB
            var removeImageResult = await _propertyRepository.RemoveImageAsync(
                propertyImage.Id
            );
            if (!removeImageResult.IsSuccess)
            {
                return Result.Failure(removeImageResult.ErrorMessage);
            }

            return Result.Success();
        }
    }
}
