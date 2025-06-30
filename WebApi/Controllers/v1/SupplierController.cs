using System.Threading.Tasks;
using Application.Features.Supplier.Commands;
using Application.Features.Supplier.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Supplier;
using Application.Features.Users.Commands;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class SupplierController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAllSupplierQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await Mediator.Send(new GetSupplierByIdQuery { Id = id }));
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Post(CreateSupplierCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Put(int id, UpdateSupplierCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(command));
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteSupplierByIdCommand { Id = id }));
        }

    }
}