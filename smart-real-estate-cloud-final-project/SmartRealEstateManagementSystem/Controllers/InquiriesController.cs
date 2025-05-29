using Application.Commands.Inquiry;
using Application.Contracts.Inquiry;
using Application.DTOs;
using Application.Queries.Inquiry;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{

    [Route("api/v1/[controller]")]
    [ApiController]
    [AuthorizeIfNotTest]
    public class InquiriesController : ControllerBase
    {
        private readonly IMediator mediator;

        public InquiriesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InquiryDto>> GetInquiryById(Guid id)
        {
            var query = new GetInquiryByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateInquiry(CreateInquiryCommand command)
        {
            var validator = new CreateInquiryCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetInquiryById), new { Id = result.Data }, result.Data);
            }
            return BadRequest(result.ErrorMessage );
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteInquiry(Guid id)
        {
            var command = new DeleteInquiryCommand { Id = id };
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound();
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateInquiry(Guid id, UpdateInquiryRequest request)
        {
            var command = new UpdateInquiryCommand { Id = id, Request = request };

            var validator = new UpdateInquiryCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            if (result.ErrorMessage == "Inquiry not found.")
            {
                return NotFound();
            }
            return BadRequest(result.ErrorMessage );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InquiryDto>>> GetAllInquiries()
        {
            var query = new GetAllInquiriesQuery();
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage );
        }
    }
}
