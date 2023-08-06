using MassTransit.Mediator;
using MassTransitPoc.UseCases.CreateBrand;
using MassTransitPoc.UseCases.CreateInvite;
using Microsoft.AspNetCore.Mvc;

namespace MassTransitPoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InviteController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InviteController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(Guid))]
        public async Task<IActionResult> CreateInvite([FromBody] InviteRequest request)
        {
            var client = _mediator.CreateRequestClient<CreateInviteRequest>();
            var response = await client.GetResponse<CreateInviteResponse>(new CreateInviteRequest()
            {
                OperationId = request.OperationId
            });
            return Ok(response);
        }
    }
}