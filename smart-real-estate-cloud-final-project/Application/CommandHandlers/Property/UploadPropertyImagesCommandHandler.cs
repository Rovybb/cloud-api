using MediatR;
using Domain.Utils;
using Domain.Repositories;
using Application.Commands.Property;
using Application.Interfaces;
using Domain.Entities;
using System.Text.Json.Nodes;

public class UploadPropertyImagesCommandHandler
    : IRequestHandler<UploadPropertyImagesCommand, Result<JsonArray>>
{
    private readonly IPropertyRepository _propertyRepository;
    private readonly IImageStorageService _imageStorageService;

    public UploadPropertyImagesCommandHandler(
        IPropertyRepository propertyRepository,
        IImageStorageService imageStorageService)
    {
        _propertyRepository = propertyRepository;
        _imageStorageService = imageStorageService;
    }

    public async Task<Result<JsonArray>> Handle(UploadPropertyImagesCommand request, CancellationToken cancellationToken)
    {
        var propertyResult = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (!propertyResult.IsSuccess)
            return Result<JsonArray>.Failure(propertyResult.ErrorMessage);

        var property = propertyResult.Data;

        if (request.Files == null || request.Files.Count == 0)
            return Result<JsonArray>.Failure("No files to upload.");

        var jsonResponse = new JsonArray();

        foreach (var file in request.Files)
        {
            if (file.Length == 0) continue;

            var id = Guid.NewGuid();

            var imageUrl = await _imageStorageService.UploadAsync(file, id);
            if (imageUrl == null)
            {
                return Result<JsonArray>.Failure("Failed to upload image.");
            }

            var imageEntity = new PropertyImage
            {
                Id = id,
                Url = imageUrl,
                PropertyId = request.PropertyId,
            };

            var addImageResult = await _propertyRepository.AddImageAsync(request.PropertyId, imageEntity);
            if (!addImageResult.IsSuccess)
            {
                return Result<JsonArray>.Failure(addImageResult.ErrorMessage);
            }

            jsonResponse.Add(new
            {
                id = id,
                url = imageUrl,
            });
        }

        return Result<JsonArray>.Success(jsonResponse);
    }
}
