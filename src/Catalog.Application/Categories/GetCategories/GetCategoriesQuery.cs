using Catalog.Application.Categories.Dto;
using MediatR;
using Joseco.DDD.Core.Results;

namespace Catalog.Application.Categories.GetCategories;

public record GetCategoriesQuery : IRequest<Result<List<CategoryDto>>>;
