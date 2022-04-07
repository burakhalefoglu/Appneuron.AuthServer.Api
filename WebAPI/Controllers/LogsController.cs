using Business.Handlers.Logs.Queries;
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
public class LogsController : BaseApiController
{
    /// <summary>
    ///     List Logs
    /// </summary>
    /// <remarks>bla bla bla Logs</remarks>
    /// <return>Logs List</return>
    /// <response code="200"></response>
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<OperationClaim>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
    [HttpGet("getall")]
    public async Task<IActionResult> GetList()
    {
        var result = await Mediator.Send(new GetLogDtoQuery());
        if (result.Success) return Ok(result);

        return BadRequest(result);
    }
}