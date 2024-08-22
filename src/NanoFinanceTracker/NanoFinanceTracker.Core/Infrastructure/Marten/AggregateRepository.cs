using CommunityToolkit.Diagnostics;
using Marten;
using Marten.Events;
using Microsoft.Extensions.Logging;
using NanoFinanceTracker.Core.Domain.DomainInteraces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Infrastructure.Marten
{
    public sealed class AggregateRepository : IAggregateRepository
    {
        private readonly IDocumentStore _store;
        private readonly ILogger<AggregateRepository> _logger;

        public AggregateRepository(IDocumentStore store, ILogger<AggregateRepository> logger)
        {
            _store = store;
            _logger = logger;
        }

        public async Task StoreAsync(string streamId, int version, IEnumerable<object> events, CancellationToken cancellationToken = default)
        {
            await using var session = await _store.LightweightSerializableSessionAsync(token: cancellationToken);
            
            session.Events.Append(streamId, version + events.Count(), events);
            await session.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<(long, TEvent)>> LoadEventsAsync<TEvent>(string id, int? version = null, CancellationToken cancellationToken = default) where TEvent : class
        {
            try
            {
                await using var session = await _store.LightweightSerializableSessionAsync(token: cancellationToken);
                var events = await session.Events.FetchStreamAsync(id, version ?? 0);
                return Map<TEvent>(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading events");
                throw;
            }
            
        }

        private IEnumerable<(long, TEvent)> Map<TEvent>(IReadOnlyList<IEvent> events) where TEvent : class
        {
            foreach (var @event in events)
            {
                var data = @event.Data as TEvent;
                if (data == null)
                {
                    _logger.LogError("Event type mismatch. Expected event wtih id {EventId} to be of type {ExpectedType} ", @event.Id, typeof(TEvent).FullName);
                    throw new ApplicationException("Event type mismatch.");
                }
                yield return (@event.Version, data);
            }
        }
    }
}
