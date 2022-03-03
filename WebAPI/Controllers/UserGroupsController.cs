using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.UserGroups.Commands;
using Business.Handlers.UserGroups.Queries;
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
    public class UserGroupsController : BaseApiController
    {
        /// <summary>
        ///     List UserGroup
        /// </summary>
        /// <remarks>bla bla bla UserGroups</remarks>
        /// <return>Kullanıcı Grup List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<UserGroup>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetUserGroupsQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Add UserGroup.
        /// </summary>
        /// <param name="createUserGroup"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserGroupCommand createUserGroup)
        {
            var result = await Mediator.Send(createUserGroup);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update UserGroup.
        /// </summary>
        /// <param name="updateUserGroup"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserGroupCommand updateUserGroup)
        {
            var result = await Mediator.Send(updateUserGroup);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Delete UserGroup.
        /// </summary>
        /// <param name="deleteUserGroup"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteUserGroupCommand deleteUserGroup)
        {
            var result = await Mediator.Send(deleteUserGroup);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}