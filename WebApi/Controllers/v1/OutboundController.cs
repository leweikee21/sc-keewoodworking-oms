using Application.DTOs.Outbound;
using Application.Features.Inbound.Commands;
using Application.Features.Outbound.Commands;
using Application.Features.Outbound.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OutboundController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAllOutboundQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        // POST api/<controller>
        [HttpPost("batch")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Post(CreateOutboundBatchCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // POST: api/<contorller>/5/reverse
        [HttpPost("{id}/reverse")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Reverse(int id, ReverseOutboundCommand command) 
        {
            var request = new ReverseOutboundCommand { Id = id, Reason = command.Reason };

            return Ok(await Mediator.Send(request));
        }
    }
}