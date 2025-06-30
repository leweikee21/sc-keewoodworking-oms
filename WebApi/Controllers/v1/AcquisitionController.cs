using Application.DTOs.Acquisition;
using Application.Features.Acquisition.Commands;
using Application.Features.Acquisition.Queries;
using Application.Features.Order.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class AcquisitionController : BaseApiController
    {
        // GET: api/<controller>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAllAcquisitionQuery query)
        {

            return Ok(await Mediator.Send(query));
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            return Ok(await Mediator.Send(new GetAcquisitionByIdQuery { Id = id }));
        }

        // POST api/<controller>
        [HttpPost]
        [Authorize(Roles = "Invevntory, Admin")]
        public async Task<IActionResult> Post(CreateAcquisitionCommand command)
        {
            return Ok(await Mediator.Send(command));
        }

        // POST: api/<controller>/5/purchaseOrder/generate
        [HttpPost("{id}/purchaseOrder/generate")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> GeneratePurchaseOrder(int id)
        {
            return Ok(await Mediator.Send(new GenerateAcquisitionPdfCommand { Id = id }));
        }

        // POST: api/<controller>/5/purchaseOrder/send
        [HttpPost("{id}/purchaseOrder/send")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> SendInvoice(int id)
        {
            return Ok(await Mediator.Send(new SendAcquisitionEmailCommand { Id = id }));
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Put(int id, UpdateAcquisitionCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }

            return Ok(await Mediator.Send(command));
        }

        // PATCH: api/<controller>/5/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateAcquisitionStatusCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            return Ok(await Mediator.Send(command));
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Inventory, Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteAcquisitionByIdCommand { Id = id }));
        }

    }
}