using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sol_Demo.Features;

namespace Sol_Demo.Controllers
{
    [Route("api/sales")]
    [Produces("application/json")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly IMediator mediator;

        public SalesController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("sales-order")]
        public async Task<IActionResult> GetSalesOrderHeader([FromQuery] GetSalesOrderHeaderSortQuery getSalesOrderHeaderSortQuery, CancellationToken cancellationToken)
        {
            var resultSet = await mediator!.Send(getSalesOrderHeaderSortQuery, cancellationToken);

            if (!resultSet.Any())
                return NotFound();

            return Ok(resultSet);
        }
    }
}