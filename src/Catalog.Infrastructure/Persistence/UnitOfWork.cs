using Catalog.Infrastructure.Persistence.DomainModel;
using Joseco.Outbox.Contracts.Model;
using Joseco.Outbox.EFCore.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Joseco.DDD.Core.Abstractions;
using System.Collections.Immutable;

namespace Catalog.Infrastructure.Persistence;

internal class UnitOfWork : IUnitOfWork, IOutboxDatabase<DomainEvent>
{
    private readonly DomainDbContext _dbContext;
    private readonly IPublisher _publisher;

    private const int MaxEventRounds = 10;
    private bool _isCommitting;

    public UnitOfWork(DomainDbContext dbContext, IPublisher publisher)
    {
        _dbContext = dbContext;
        _publisher = publisher;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        // Domain event handlers only stage changes in the DbContext (outbox rows, other aggregates);
        // they must not save. If one calls CommitAsync anyway, ignore it: this outer call publishes
        // whatever it staged and persists everything in a single SaveChanges (one DB transaction).
        if (_isCommitting)
        {
            return;
        }

        _isCommitting = true;
        try
        {
            // Handlers may touch other aggregates that raise further events, so keep draining
            // until a round produces none.
            for (int round = 0; ; round++)
            {
                List<DomainEvent> domainEvents = CollectDomainEvents();
                if (domainEvents.Count == 0)
                {
                    break;
                }
                if (round == MaxEventRounds)
                {
                    throw new InvalidOperationException(
                        $"Domain events cascaded for more than {MaxEventRounds} rounds; probably an event cycle.");
                }

                foreach (DomainEvent e in domainEvents)
                {
                    await _publisher.Publish(e, cancellationToken);
                }
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            // Drop partial changes so a later commit in the same scope can't persist them.
            _dbContext.ChangeTracker.Clear();
            throw;
        }
        finally
        {
            _isCommitting = false;
        }
    }

    private List<DomainEvent> CollectDomainEvents()
    {
        return _dbContext.ChangeTracker
            .Entries<Entity>()
            .Where(x => x.Entity.DomainEvents.Any())
            .SelectMany(x =>
            {
                var events = x.Entity.DomainEvents.ToImmutableArray();
                x.Entity.ClearDomainEvents();
                return events;
            })
            .ToList();
    }

    public DbSet<OutboxMessage<DomainEvent>> GetOutboxMessages()
    {
        return _dbContext.OutboxMessages;
    }
}
