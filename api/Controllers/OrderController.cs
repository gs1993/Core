using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Domain.Commands;
using WebApi.Domain.Queries;
using WebApi.Dto.Order;

namespace WebApi.Controllers
{
    public class OrderController : BaseLogicController
    {
        public OrderController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(IReadOnlyList<OrderDto>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> Get(SearchOrderDto dto, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetOrderListQuery
            {
                Email = dto.Email,
                ProductId = dto.ProductId,
                MaxOrderItems = dto.MaxOrderItems,
                MinOrderItems = dto.MinOrderItems,
                MaxOrderValue = dto.MaxOrderValue,
                MinOrderValue = dto.MinOrderValue
            }, cancellationToken);

            return Ok(products);
        }

        [HttpPost("create")]
        [Authorize]
        [SwaggerResponse((int)HttpStatusCode.OK, "Order created successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> Create(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            var orderItems = dto.OrderItems?.Select(x => new CreateOrderCommand.OrderItemCommandDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity
            });
            var createProductResult = await _mediator.Send(new CreateOrderCommand
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberCountryOrderCode = dto.PhoneNumberCountryOrderCode,
                OrderItems = orderItems
            }, cancellationToken);

            return FromResult(createProductResult);
        }
    }
}
