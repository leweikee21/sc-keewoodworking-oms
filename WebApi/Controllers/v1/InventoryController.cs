using Application.DTOs.Inventory;
using Application.Features.Inventory.Commands;
using Application.Features.Inventory.Queries;
using Application.Features.Users.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InventoryController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAllInventoryQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        // GET: api/<controller>/noSupplier
        [HttpGet("noSupplier")]
        [Authorize]
        public async Task<IActionResult> GetNoSupplier()
        {
            return Ok(await Mediator.Send(new GetAllInventoryWithNoSupplierQuery { }));
        }


        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await Mediator.Send(new GetInventoryByIdQuery { Id = id }));
        }

        // GET api/<controller>/category/material
        [HttpGet("category/{category}")]
        [Authorize]
        public async Task<IActionResult> Get(string category)
        {
            return Ok(await Mediator.Send(new GetInventoryByCategoryQuery { Category = category }));
        }

        // GET api/<controller>/supplier/5
        [HttpGet("supplier/{id}")]
        [Authorize]
        public async Task<IActionResult> GetBySupplier(int id)
        {
            return Ok(await Mediator.Send(new GetInventoryBySupplierQuery { SupplierId = id }));
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Post(CreateInventoryCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // POST api/<controller>/5/activate
        [HttpPost("{id}/activate")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Reactivate(int id, ReactivateInventoryRequest request)
        {
            var command = new ReactivateInventoryCommand { Id = id, Request = request };

            return Ok(await Mediator.Send(command));
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Put(int id, UpdateInventoryCommand command)
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
            return Ok(await Mediator.Send(new DeleteInventoryByIdCommand { Id = id }));
        }

    }
}