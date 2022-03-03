using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.OperationClaims.Commands;
using Business.Handlers.OperationClaims.Queries;
using Core.Entities.Concrete;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IResult = Core.Utilities.Results.IResult;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OperationClaimsController : BaseApiController
    {
        /// <summary>
        ///     List OperationClaims
        /// </summary>
        /// <remarks>bla bla bla OperationClaims</remarks>
        /// <return>OperationClaims List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<OperationClaim>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetOperationClaimsQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>bla bla bla OperationClaims</remarks>
        /// <return>OperationClaims List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<OperationClaim>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetByid(long id)
        {
            var result = await Mediator.Send(new GetOperationClaimQuery {Id = id});
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     List OperationClaims
        /// </summary>
        /// <remarks>bla bla bla OperationClaims</remarks>
        /// <return>OperationClaims List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<SelectionItem>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getoperationclaimlookup")]
        public async Task<IActionResult> GetOperationClaimLookup()
        {
            var result = await Mediator.Send(new GetOperationClaimLookupQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update OperationClaim .
        /// </summary>
        /// <param name="updateOperationClaim"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateOperationClaimCommand updateOperationClaim)
        {
            var result = await Mediator.Send(updateOperationClaim);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}