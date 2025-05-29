using MediatR;
using Domain.Utils;
using Microsoft.AspNetCore.Http;
using System.Text.Json.Nodes;

namespace Application.Commands.Property
{
    public class UploadPropertyImagesCommand : IRequest<Result<JsonArray>>
    {
        public Guid PropertyId { get; set; }
        public List<IFormFile>? Files { get; set; }
    }
}
