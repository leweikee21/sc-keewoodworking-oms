using System.Threading.Tasks;
using Application.DTOs.Account;
using Application.Features.Users.Commands;
using Application.Features.Users.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.User;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class UserController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get([FromQuery] GetAllUsersQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await Mediator.Send(new GetUserByIdQuery { UserId = id }));
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post(RegisterRequest request)
        {
            var command = new CreateUserCommand { Request = request };
            return Ok(await Mediator.Send(command));
        }

        // POST api/<controller>/5/activate
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reactivate(string id, ReactivateUserRequest request)
        {
            var command = new ReactivateUserCommand { UserId = id, Request = request };

            return Ok(await Mediator.Send(command));
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(string id, UpdateUserRequest request)
        {
            var command = new UpdateUserCommand { UserId = id, Request = request };

            return Ok(await Mediator.Send(command));
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await Mediator.Send(new DeleteUserByIdCommand { UserId = id }));
        }

        // GET: api/<controller>
        [HttpGet("options")]
        [Authorize]
        public async Task<IActionResult> GetUserNames([FromQuery] GetAllUsersNameQuery query)
        {

            return Ok(await Mediator.Send(query));
        }
    }
}