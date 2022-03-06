using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.Users.Commands;
using Business.Handlers.Users.Queries;
using Core.Entities.Dtos;
using Core.Utilities.Results;
using Entities.Dtos;
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
    public class UsersController : BaseApiController
    {
        /// <summary>
        ///     List Users
        /// </summary>
        /// <remarks>bla bla bla Users</remarks>
        /// <return>Users List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<UserDto>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetUsersQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>bla bla bla </remarks>
        /// <return>Users List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet]
        public async Task<IActionResult> GetById()
        {
            var result = await Mediator.Send(new GetUserQuery());
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Add User.
        /// </summary>
        /// <param name="createUser"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserCommand createUser)
        {
            var result = await Mediator.Send(createUser);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Update User.
        /// </summary>
        /// <param name="updateUser"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserCommand updateUser)
        {
            var result = await Mediator.Send(updateUser);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        ///     Delete User.
        /// </summary>
        /// <param name="deleteUser"></param>
        /// <returns></returns>
        [Consumes("application/json")]
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteUserCommand deleteUser)
        {
            var result = await Mediator.Send(deleteUser);
            if (result.Success) return Ok(result);

            return BadRequest(result);
        }
    }
}