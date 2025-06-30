using System.Threading.Tasks;
using Application.Features.Order.Commands;
using Application.Features.Order.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Orders;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class OrderController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAllOrderQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await Mediator.Send(new GetOrderByIdQuery { Id = id }));
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> Post(CreateOrderCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // POST: api/<controller>/5/complete
        [HttpPost("{id}/complete")]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> Complete(int id)
        {
            return Ok(await Mediator.Send(new CompleteOrderCommand { Id = id }));
        }

        // POST: api/<controller>/5/invoice/generate
        [HttpPost("{id}/invoice/generate")]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> GenerateInvoice(int id)
        {
            return Ok(await Mediator.Send(new GenerateInvoicePdfCommand { Id = id }));
        }

        // POST: api/<controller>/5/invoice/send
        [HttpPost("{id}/invoice/send")]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> SendInvoice(int id)
        {
            return Ok(await Mediator.Send(new SendIncoiveEmailCommand { Id = id }));
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> Put(int id, UpdateOrderCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(command));
        }

        // PATCH: api/<controller>/5/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            return Ok(await Mediator.Send(command));
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Order, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteOrderByIdCommand { Id = id }));
        }

    }
}