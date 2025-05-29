using Application.Commands.Property;
using Application.Contracts.Property;
using Application.DTOs;
using Application.Queries.Property;
using Domain.Utils;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AuthorizeIfNotTest]
    public class PropertiesController : ControllerBase
    {
        private readonly IMediator mediator;

        public PropertiesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyDto>> GetPropertyById(Guid id)
        {
            var query = new GetPropertyByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage );
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<PropertyDto>>> GetAllProperties(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Dictionary<string, string>? filters = null)
        {
            var query = new GetAllPropertiesQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                Filters = filters
            };

            var result = await mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProperty(CreatePropertyCommand command)
        {
            var validator = new CreatePropertyCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetPropertyById), new { Id = result.Data }, result.Data);
            }
            return BadRequest(result.ErrorMessage );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Guid>> UpdateProperty(Guid id, UpdatePropertyRequest request)
        {
            var command = new UpdatePropertyCommand { Id = id, Request = request };
            var validator = new UpdatePropertyCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (result == null)
            {
                return NotFound("Property not found.");
            }

            if (result.IsSuccess)
            {
                return NoContent();
            }
            return BadRequest(result.ErrorMessage ); // here is optional to send the error message
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProperty(Guid id)
        {
            var command = new DeletePropertyCommand { Id = id };
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage );
        }

        [HttpPost("{propertyId}/images")]
        public async Task<IActionResult> UploadPropertyImages(Guid propertyId, [FromForm] List<IFormFile> files)
        {
            var command = new UploadPropertyImagesCommand
            {
                PropertyId = propertyId,
                Files = files
            };

            var result = await mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetPropertyById), new { Id = propertyId }, result.Data);
            }
            else
            {
                return BadRequest(result.ErrorMessage);
            }
        }

        [HttpDelete("{propertyId}/images/{imageId}")]
        public async Task<IActionResult> DeletePropertyImage(Guid imageId)
        {
            var command = new DeletePropertyImageCommand
            {
                Id = imageId
            };
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            else
            {
                return NotFound(result.ErrorMessage);
            }
        }

    }
}
