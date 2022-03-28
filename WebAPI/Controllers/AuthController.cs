using Business.Handlers.Authorizations.Commands;
using Business.Handlers.Authorizations.Queries;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace WebAPI.Controllers;

/// <summary>
///     Make it Authorization operations
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AuthController : BaseApiController
{
    /// <summary>
    ///     Make it User Login operations
    /// </summary>
    /// <param name="loginOrRegisterModel"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<AccessToken>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPost("loginorregister")]
    public async Task<IActionResult> LoginOrRegister([FromBody] LoginOrRegisterUserCommand loginOrRegisterModel)
    {
        var result = await Mediator.Send(loginOrRegisterModel);
        SetCookie("RefreshToken", result.Data.RefreshToken, 60, true, false, SameSiteMode.None);
        if (result.Success) return Ok(result);
        return BadRequest(result);
    }

    /// <summary>
    ///   refresh token
    /// </summary>
    /// <return>Logs token</return>
    /// <response code="200"></response>
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<AccessToken>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpGet("refreshToken")]
    public async Task<IActionResult> Get()
    {
        var result = await Mediator.Send(new GetCustomerTokenQuery());
        if (result.Success) return Ok(result);
        return Unauthorized(result);
    }

    /// <summary>
    ///     Make it Forgot Password operations
    /// </summary>
    /// <return></return>
    /// <response code="200"></response>
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPut("forgotpassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand forgotPassword)
    {
        var result = await Mediator.Send(forgotPassword);

        if (result.Success) return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    ///     Make it Reset Password operations
    /// </summary>
    /// <return></return>
    /// <response code="200"></response>
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPost("resetpassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand resetPassword)
    {
        var result = await Mediator.Send(resetPassword);

        if (result.Success) return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    ///     Make it Change Password operation
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPut("changeuserpassword")]
    public async Task<IActionResult> ChangeUserPassword([FromBody] UserChangePasswordCommand command)
    {
        var result = await Mediator.Send(command);

        if (result.Success) return Ok(result);

        return BadRequest(result);
    }
}