using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.Translates.Commands;
using Business.Handlers.Translates.Queries;
using Core.Entities.Concrete;
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
    public class TranslatesController : BaseApiController
    {
        /// <summary>
        ///     List Translate
        /// </summary>
        /// <remarks>bla bla bla Translates</remarks>
        /// <return>Translates List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<Translate>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetTranslatesQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>bla bla bla </remarks>
        /// <return>Translate List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<Translate>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(long translateId)
        {
            var result = await Mediator.Send(new GetTranslateQuery {Id = translateId});
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Add Translate.
        /// </summary>
        /// <param name="createTranslate"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateTranslateCommand createTranslate)
        {
            var result = await Mediator.Send(createTranslate);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update Translate.
        /// </summary>
        /// <param name="updateTranslate"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateTranslateCommand updateTranslate)
        {
            var result = await Mediator.Send(updateTranslate);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Delete Translate.
        /// </summary>
        /// <param name="deleteTranslate"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteTranslateCommand deleteTranslate)
        {
            var result = await Mediator.Send(deleteTranslate);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}