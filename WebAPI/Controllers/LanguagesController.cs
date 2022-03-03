using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.Languages.Commands;
using Business.Handlers.Languages.Queries;
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
    public class LanguagesController : BaseApiController
    {
        /// <summary>
        ///     List languages
        /// </summary>
        /// <remarks>bla bla bla Languages</remarks>
        /// <return>Languages List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<Language>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetLanguagesQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>bla bla bla </remarks>
        /// <return>Language List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<Language>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(long languageId)
        {
            var result = await Mediator.Send(new GetLanguageQuery {Id = languageId});
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Add Language.
        /// </summary>
        /// <param name="createLanguage"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateLanguageCommand createLanguage)
        {
            var result = await Mediator.Send(createLanguage);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update Language.
        /// </summary>
        /// <param name="updateLanguage"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLanguageCommand updateLanguage)
        {
            var result = await Mediator.Send(updateLanguage);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Delete Language.
        /// </summary>
        /// <param name="deleteLanguage"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteLanguageCommand deleteLanguage)
        {
            var result = await Mediator.Send(deleteLanguage);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}