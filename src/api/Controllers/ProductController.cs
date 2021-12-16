using Domain.Accounts.Entities;
using Domain.Products.Commands;
using Domain.Products.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Dto.Product;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using WebApi.Helpers;

namespace WebApi.Controllers
{
    public class ProductController : BaseLogicController
    {
        public ProductController(IMediator mediator) : base(mediator)
        {
        }


        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, type: typeof(IReadOnlyList<ProductDto>))]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> Get(SearchProductDto dto, CancellationToken cancellationToken)
        {
            var products = await _mediator.Send(new GetProductListQuery
            {
                Name = dto.Name,
                MinQuantity = dto.MinQuantity,
                MaxQuantity = dto.MaxQuantity,
                MinPrice = dto.MinPrice,
                MaxPrice = dto.MaxPrice,
                Currency = dto.Currency
            }, cancellationToken);

            return Ok(products);
        }

        [HttpPost("create")]
        [Authorize(Role.Admin)]
        [SwaggerResponse((int)HttpStatusCode.OK, "Product created successfully")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, type: typeof(string))]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, type: typeof(string))]
        public async Task<IActionResult> Create(CreateProductDto dto, CancellationToken cancellationToken)
        {
            var createProductResult = await _mediator.Send(new CreateProductCommand
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Currency = dto.Currency,
                Quantity = dto.Quantity
            }, cancellationToken);

            return FromResult(createProductResult);
        }
    }
}
