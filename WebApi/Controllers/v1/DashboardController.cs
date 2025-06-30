using System.Threading.Tasks;
using Application.Features.Supplier.Commands;
using Application.Features.Supplier.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Application.DTOs.Supplier;
using Application.Features.Users.Commands;
using Application.Features.Dashboard.Queries;

namespace WebApi.Controllers.v1
{
    [ApiVersion("1.0")]
    public class DashboardController : BaseApiController
    {
        // GET: api/<controller>/admin
        [HttpGet("admin")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetAdminDashboardQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        // GET api/<controller>/order
        [HttpGet("order")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetOrderDashboardQuery query)
        {
            return Ok(await Mediator.Send(query));
        }

        // GET: api/<controller>/inventory
        [HttpGet("inventory")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery] GetInventoryDashboardQuery query)
        {
            return Ok(await Mediator.Send(query));
        }
    }
}