using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IMediator _mediator;

        public PaymentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //[HttpPost("sale")]
        //[Authorize]
        //[SwaggerResponse((int)HttpStatusCode.OK, "Sale processed successfully")]
        //[SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        //[SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        //public async Task<IActionResult> Add(, CancellationToken cancellationToken)
        //{
        //    var addSiteResult = await _mediator.Send(new CreateSiteCommand
        //    {
        //        Name = dto.Name,
        //        AddressFirstLine = dto.AddressFirstLine,
        //        AddressSecondLine = dto.AddressSecondLine
        //    }, cancellationToken);

        //    return addSiteResult.IsFailure
        //        ? BadRequest(addSiteResult.Error)
        //        : Ok();
        //}
    }
}
