using ApplicationService.Repositories.Common;
using Domain.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Common
{
    public class EventStore : IEventStore
    {
        private readonly List<Event> _events = new();

        public Task SaveAsync(Event @event)
        {
            _events.Add(@event);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Event>> GetEventsAsync(string aggregateId)
        {
            // Burada filtreleme yapabilirsin aggregateId bazında
            return Task.FromResult(_events.AsEnumerable());
        }
    }
}
