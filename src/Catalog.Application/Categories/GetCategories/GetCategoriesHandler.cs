using Catalog.Application.Categories.Dto;
using Catalog.Domain.Categories;
using MediatR;
using Joseco.DDD.Core.Results;

namespace Catalog.Application.Categories.GetCategories;

internal class GetCategoriesHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, Result<List<CategoryDto>>>
{
    private readonly ICategoryRepository _categoryRepository = categoryRepository;

    public async Task<Result<List<CategoryDto>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(readOnly: true);

        var dtos = categories.Select(c => new CategoryDto
        {
            CategoryId = c.Id,
            Name = c.Name
        }).ToList();

        return Result.Success(dtos);
    }
}
