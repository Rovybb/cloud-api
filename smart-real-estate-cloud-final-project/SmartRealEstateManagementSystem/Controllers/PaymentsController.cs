using Application.Commands.Payment;
using Application.Contracts.Payment;
using Application.DTOs;
using Application.Queries.Payment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [AuthorizeIfNotTest]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator mediator;

        public PaymentsController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetPaymentById(Guid id)
        {
            var query = new GetPaymentByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreatePayment(CreatePaymentCommand command)
        {
            var validator = new CreatePaymentCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetPaymentById), new { Id = result.Data }, result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePayment(Guid id)
        {
            var command = new DeletePaymentCommand { Id = id };
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound(result.ErrorMessage);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePayment(Guid id, UpdatePaymentRequest request)
        {
            var command = new UpdatePaymentCommand { Id = id, Request = request };

            var validator = new UpdatePaymentCommandValidator();
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
            if (result.ErrorMessage == "Payment not found.")
            {
                return NotFound();
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAllPayments()
        {
            var query = new GetAllPaymentsQuery();
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("create-checkout-session")]
        public async Task<ActionResult> CreateStripeCheckout([FromBody] CreateCheckoutCommand command)
        {
            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(new
            {
                CheckoutUrl = result.Data
            });
        }
    }
}
