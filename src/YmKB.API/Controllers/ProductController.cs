using Mediator;
using Microsoft.AspNetCore.Mvc;
using YmKB.Application.Features.Products.DTOs;
using YmKB.Application.Features.Products.Queries;
using YmKB.Domain.Entities.Trial;

namespace YmKB.API.Controllers;

/*[ApiController]
[Route("[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpGet]
    public async Task<List<ProductDto>> Get()
    {
        var data = await _mediator.Send(new GetAllProductsQuery());
        return data;
    }
}*/