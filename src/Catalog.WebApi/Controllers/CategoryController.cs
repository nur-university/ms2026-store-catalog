using Catalog.Application.Categories.CreateCategory;
using Catalog.Application.Categories.GetCategories;
using Catalog.WebApi.Infrastructure;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nur.Store2026.Access.Catalog.Permissions;

namespace Catalog.WebApi.Controllers;

[Route("/api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : CustomController
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [Authorize(Policy = CatalogPermission.CategoryManagement)]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _mediator.Send(new GetCategoriesQuery());
        return BuildResult(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategory(Guid id)
    {
        GetCategoryByIdQuery query = new GetCategoryByIdQuery()
        {
            CategoryId = id
        };
        var result = await _mediator.Send(query);
        return BuildResult(result);
    }

    [HttpPost]
    [Authorize(Policy = CatalogPermission.CategoryManagement)]
    public async Task<IActionResult> CreateCategory([FromBody]CreateCategoryCommand command)
    {
        var result = await _mediator.Send(command);

        return BuildResult(result);
    }
}
