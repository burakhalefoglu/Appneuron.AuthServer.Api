using System.Threading.Tasks;
using Business.Handlers.GroupClaims.Commands;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers
{
    /// <summary>
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GroupClaimsController : BaseApiController
    {
        /// <summary>
        ///     Addded GroupClaim .
        /// </summary>
        /// <param name="createGroupClaim"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateGroupClaimCommand createGroupClaim)
        {
            var result = await Mediator.Send(createGroupClaim);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update GroupClaim.
        /// </summary>
        /// <param name="updateGroupClaim"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateGroupClaimCommand updateGroupClaim)
        {
            var result = await Mediator.Send(updateGroupClaim);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Delete GroupClaim.
        /// </summary>
        /// <param name="deleteGroupClaim"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteGroupClaimCommand deleteGroupClaim)
        {
            var result = await Mediator.Send(deleteGroupClaim);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}