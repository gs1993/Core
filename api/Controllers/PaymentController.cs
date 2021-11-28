using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain.Commands;

namespace WebApi.Controllers
{
    public class PaymentController : BaseLogicController
    {
        public PaymentController(IMediator mediator) : base(mediator) 
        { 
        }


        [HttpPost("initiatePayment/{orderId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Sale processed successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> Add(long orderId, CancellationToken cancellationToken)
        {
            var paymentResult = await _mediator.Send(new InitiatePaymentCommand
            {
                OrderId = orderId
            }, cancellationToken);

            return FromResult(paymentResult);
        }
    }
}
