using System.Threading.Tasks;
using Business.Handlers.Groups.Commands;
using Business.Handlers.Groups.Queries;
using Core.Entities.Concrete;
using Core.Utilities.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GroupsController : BaseApiController
    {
        /// <summary>
        ///     List Groups
        /// </summary>
        /// <remarks>bla bla bla Groups</remarks>
        /// <return>Grup List</return>
        /// <response code="200"></response>
        //[AllowAnonymous]
        //[Produces("application/json","text/plain")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Group>))]
        //[ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetGroupsQuery());
            if (result.Success) return Ok(result.Data);

            return BadRequest(result.Message);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>bla bla bla </remarks>
        /// <return>Grup List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<Group>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(long groupId)
        {
            var result = await Mediator.Send(new GetGroupQuery {GroupId = groupId});
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Add Group .
        /// </summary>
        /// <param name="createGroup"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateGroupCommand createGroup)
        {
            var result = await Mediator.Send(createGroup);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update Group.
        /// </summary>
        /// <param name="updateGroup"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateGroupCommand updateGroup)
        {
            var result = await Mediator.Send(updateGroup);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Delete Group.
        /// </summary>
        /// <param name="deleteGroup"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteGroupCommand deleteGroup)
        {
            var result = await Mediator.Send(deleteGroup);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}