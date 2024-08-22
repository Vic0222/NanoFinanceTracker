using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NanoFinanceTracker.Core.Domain.DomainInteraces
{
    public interface IAggregateRepository
    {
        Task<IEnumerable<(long version, TEvent @event)>> LoadEventsAsync<TEvent>(string id, int? version = null, CancellationToken cancellationToken = default) where TEvent : class;
        Task StoreAsync(string streamId, int version, IEnumerable<object> events, CancellationToken cancellationToken = default);
    }
}
