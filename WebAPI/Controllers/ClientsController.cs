using System.Threading.Tasks;
using Business.Handlers.Clients.Commands;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     Clients If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : BaseApiController
    {
        /// <summary>
        ///     Creatate Client Token.
        /// </summary>
        /// <param name="createClientToken"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost("getClientToken")]
        public async Task<IActionResult> GetClientToken([FromBody] CreateTokenCommand createClientToken)
        {
            var result = await Mediator.Send(createClientToken);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}