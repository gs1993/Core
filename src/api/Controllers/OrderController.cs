using Domain.Orders.Commands;
using Domain.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Order;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

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
        public async Task<ActionResult<IReadOnlyList<OrderDto>>> Get([FromBody]SearchOrderDto dto, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetOrderListQuery
            {
                Email = dto.Email,
                ProductName = dto.ProductName,
                MaxOrderItems = dto.MaxOrderItems,
                MinOrderItems = dto.MinOrderItems,
                MaxOrderValue = dto.MaxOrderValue,
                MinOrderValue = dto.MinOrderValue
            }, cancellationToken);

            return Ok(products);
        }

        [HttpPost("create")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Order created successfully", type: typeof(long))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> Create(CreateOrderDto dto, CancellationToken cancellationToken)
        {
            var orderItems = dto.OrderItems?.Select(x => new CreateOrderCommand.OrderItemCommandDto
            {
                ProductId = x.ProductId,
                Quantity = x.Quantity
            });
            var createOrdertResult = await _mediator.Send(new CreateOrderCommand
            {
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                PhoneNumberCountryOrderCode = dto.PhoneNumberCountryOrderCode,
                OrderItems = orderItems
            }, cancellationToken);

            return FromResult(createOrdertResult);
        }

        [HttpPost("AddOrderItem/{orderId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Order item added successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> AddOrderItem(long orderId, CreateOrderItemDto dto, CancellationToken cancellationToken)
        {
            var addOrderItemResult = await _mediator.Send(new AddOrderItemCommand
            {
                OrderId = orderId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            }, cancellationToken);

            return FromResult(addOrderItemResult);
        }

        [HttpPost("SubmitOrder/{orderId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Order processed successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> SubmitOrder(long orderId, CancellationToken cancellationToken)
        {
            var paymentResult = await _mediator.Send(new SubmitOrderCommand
            {
                OrderId = orderId
            }, cancellationToken);

            return FromResult(paymentResult);
        }
    }
}
