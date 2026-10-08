using Joseco.DDD.Core.Abstractions;

namespace Catalog.Domain.Products.Events;

public record ProductCreated(Guid ProductId, string Name, string Description, Guid CategoryId, decimal Price = 0) : DomainEvent
{
    public ProductCreated() : this(Guid.Empty, string.Empty, string.Empty, Guid.Empty, 0)
    {
    }
}