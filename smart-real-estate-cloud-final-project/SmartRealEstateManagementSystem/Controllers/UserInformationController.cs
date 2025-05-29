using Application.Commands.User;
using Application.Contracts.UserInformation;
using Application.DTOs;
using Application.Queries.UserInformation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [AuthorizeIfNotTest]
    public class UserInformationController : ControllerBase
    {
        private readonly IMediator mediator;

        public UserInformationController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var query = new GetUserInformationByIdQuery { Id = id };
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return NotFound(result.ErrorMessage );
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var query = new GetAllUserInformationsQuery();
            var result = await mediator.Send(query);
            if (result.IsSuccess)
            {
                return Ok(result.Data);
            }
            return BadRequest(result.ErrorMessage );
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateUser(CreateUserInformationCommand command)
        {
            var validator = new CreateUserInformationCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetUserById), new { Id = result.Data }, result.Data);
            }
            return BadRequest(result.ErrorMessage );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateUserInformationRequest request)
        {
            var command = new UpdateUserInformationCommand { Id = id, Request = request };

            var validator = new UpdateUserInformationCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var result = await mediator.Send(command);
            if (!result.IsSuccess)
            {
                if (result.ErrorMessage == "User not found")
                {
                    return NotFound(result.ErrorMessage );
                }
                return BadRequest(result.ErrorMessage );
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(Guid id)
        {
            var command = new DeleteUserInformationCommand { Id = id };
            var result = await mediator.Send(command);
            if (result.IsSuccess)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
