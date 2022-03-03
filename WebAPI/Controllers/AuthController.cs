using Business.Abstract;
using Business.Handlers.Authorizations.Commands;
using Business.Handlers.Authorizations.Queries;
using Business.Handlers.Users.Commands;
using Core.Entities.ApiModel;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     Make it Authorization operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : BaseApiController
    {

        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        ///     Make it User Login operations
        /// </summary>
        /// <param name="loginModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<AccessToken>))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(Core.Utilities.Results.IResult))]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserQuery loginModel)
        {
            var result = await Mediator.Send(loginModel);

            if (result.Success) return Ok(result);

            return Unauthorized(result);
        }

        /// <summary>
        ///     Make it User Register operations
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<AccessToken>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("register")]
        public async Task<IActionResult> Register(Register register)
        {
            var result = await _authService.Register(register);

            if (result.Success) return Ok(result);
            return Unauthorized("result");
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
}