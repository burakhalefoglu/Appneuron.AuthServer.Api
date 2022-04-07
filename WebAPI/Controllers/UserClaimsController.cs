using Business.Handlers.UserClaims.Commands;
using Business.Handlers.UserClaims.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers;

/// <summary>
///     If controller methods will not be Authorize, [AllowAnonymous] is used.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class UserClaimsController : BaseApiController
{
    /// <summary>
    ///     List UserClaims
    /// </summary>
    /// <remarks>bla bla bla UserClaims</remarks>
    /// <return>UserClaims List</return>
    /// <response code="200"></response>
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<UserClaim>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpGet("getall")]
    public async Task<IActionResult> GetList()
    {
        var result = await Mediator.Send(new GetUserClaimsQuery());
        if (result.Success) return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    ///     Add GroupClaim.
    /// </summary>
    /// <param name="createUserClaim"></param>
    /// <returns></returns>
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPost]
    public async Task<IActionResult> Add([FromBody] CreateUserClaimCommand createUserClaim)
    {
        var result = await Mediator.Send(createUserClaim);
        if (result.Success) return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    ///     Update GroupClaim.
    /// </summary>
    /// <param name="updateUserClaim"></param>
    /// <returns></returns>
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpPut]
    public async Task<IActionResult> Update([FromBody] UpdateUserClaimCommand updateUserClaim)
    {
        var result = await Mediator.Send(updateUserClaim);
        if (result.Success) return Ok(result);

        return BadRequest(result);
    }

    /// <summary>
    ///     Delete GroupClaim.
    /// </summary>
    /// <param name="deleteUserClaim"></param>
    /// <returns></returns>
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpDelete]
    public async Task<IActionResult> Delete([FromBody] DeleteUserClaimCommand deleteUserClaim)
    {
        var result = await Mediator.Send(deleteUserClaim);
        if (result.Success) return Ok(result);

        return BadRequest(result);
    }
}