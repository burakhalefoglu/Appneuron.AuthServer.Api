﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Business.Handlers.UserProjects.Commands;
using Business.Handlers.UserProjects.Queries;
using Core.Utilities.Results;
using Entities.Concrete;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    ///     UserProjects If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserProjectsController : BaseApiController
    {
        /// <summary>
        ///     List UserProjects
        /// </summary>
        /// <remarks>UserProjects</remarks>
        /// <return>List UserProjects</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<IEnumerable<UserProject>>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getall")]
        public async Task<IActionResult> GetList()
        {
            var result = await Mediator.Send(new GetUserProjectsQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>UserProjects</remarks>
        /// <return>UserProjects List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<UserProject>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getbyid")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await Mediator.Send(new GetUserProjectQuery { Id = id });
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Add UserProject.
        /// </summary>
        /// <param name="createUserProject"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CreateUserProjectCommand createUserProject)
        {
            var result = await Mediator.Send(createUserProject);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Update UserProject.
        /// </summary>
        /// <param name="updateUserProject"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateUserProjectCommand updateUserProject)
        {
            var result = await Mediator.Send(updateUserProject);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     Delete UserProject.
        /// </summary>
        /// <param name="deleteUserProject"></param>
        /// <returns></returns>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IResult))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteUserProjectCommand deleteUserProject)
        {
            var result = await Mediator.Send(deleteUserProject);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        /// <summary>
        ///     It brings the details according to its id.
        /// </summary>
        /// <remarks>UserProjects</remarks>
        /// <return>UserProjects List</return>
        /// <response code="200"></response>
        [Produces("application/json", "text/plain")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IDataResult<UserProject>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(IResult))]
        [HttpGet("getByUserId")]
        public async Task<IActionResult> GetById()
        {
            var result = await Mediator.Send(new GetUserProjectsByUserIdQuery());
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }
}