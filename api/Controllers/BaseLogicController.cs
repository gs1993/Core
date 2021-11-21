using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;

namespace WebApi.Controllers
{
    public abstract class BaseLogicController : BaseController
    {
        protected readonly IMediator _mediator;

        public BaseLogicController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }


        public IActionResult FromResult(Result result)
        {
            return result.IsFailure
                ? BadRequest(result.Error)
                : Ok();
        }

        public IActionResult FromResult<T>(Result<T> result)
        {
            return result.IsFailure
                ? BadRequest(result.Error)
                : Ok(result.Value);
        }
    }
}
