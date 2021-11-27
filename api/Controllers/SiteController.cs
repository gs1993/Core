using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain.Commands;
using WebApi.Domain.Queries;
using WebApi.Dto.Site;

namespace WebApi.Controllers
{
    public class SiteController : BaseLogicController
    {
        public SiteController(IMediator mediator) : base(mediator)
        {
        }


        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(IReadOnlyList<SiteDto>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<ActionResult<IReadOnlyList<SiteDto>>> Get(CancellationToken cancellationToken)
        {
            var sites = await _mediator.Send(new GetSiteListQuery { Name = string.Empty }, cancellationToken);
            return Ok(sites);
        }

        [HttpPost("add")]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, "Site added successfully", type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> Add(AddSiteDto dto, CancellationToken cancellationToken)
        {
            var addSiteResult = await _mediator.Send(new CreateSiteCommand
            {
                Name = dto.Name,
                AddressFirstLine = dto.AddressFirstLine,
                AddressSecondLine = dto.AddressSecondLine
            }, cancellationToken);

            return FromResult(addSiteResult);
        }
    }
}
