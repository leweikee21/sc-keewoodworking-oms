using Application.DTOs.Inbound;
using Application.Features.Inbound.Commands;
using Application.Features.Inbound.Queries;
using Application.Features.Inventory.Commands;
using Application.Features.Outbound.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class InboundController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAllInboundQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Post(CreateInboundCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // POST api/<controller>
        [HttpPost("batch")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Post(CreateInboundBatchCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // POST: api/<contorller>/5/reverse
        [HttpPost("{id}/reverse")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Reverse(int id, ReverseInboundCommand command)
        {
            var request = new ReverseInboundCommand { Id = id, Reason = command.Reason };

            return Ok(await Mediator.Send(request));
        }

    }
}