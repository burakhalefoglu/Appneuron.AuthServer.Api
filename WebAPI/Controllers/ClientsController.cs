﻿
using System;
using Business.Handlers.Clients.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Core.Utilities.Results;
using Core.Utilities.Security.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    /// <summary>
    /// Clients If controller methods will not be Authorize, [AllowAnonymous] is used.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientsController : BaseApiController
    {

        /// <summary>
        /// Creatate Client Token.
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
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);



        }
    }
}