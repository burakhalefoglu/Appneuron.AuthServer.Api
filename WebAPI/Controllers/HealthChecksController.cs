using Core.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers;

/// <summary>
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class HealthChecksController : BaseApiController
{
    /// <summary>
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [Consumes("application/json")]
    [Produces("application/json", "text/plain")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
    [HttpGet("get")]
    public Task<IActionResult> CheckHealth()
    {
        return Task.FromResult<IActionResult>(Ok(new SuccessResult()));
    }
}