using Domain.Utils;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageStorageService
    {
        Task<string> UploadAsync(IFormFile file, Guid imageId);
        Task<Result> DeleteAsync(string imageUrl);
    }
}
