using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var res = await _mediator.Send(command);
        if (res.IsSuccess)
        {
            return Ok(new { Id = res.Data });
        }
        return BadRequest(res.ErrorMessage);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand command)
    {
        var res = await _mediator.Send(command);
        if (res.IsSuccess)
        {
            return Ok(new { Token = res.Data });
        }
        return BadRequest(res.ErrorMessage);
    }
}
