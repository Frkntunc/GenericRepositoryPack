using Domain.Events.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationService.Repositories.Common
{
    public interface IEventStore
    {
        Task SaveAsync(Event @event);
        Task<IEnumerable<Event>> GetEventsAsync(string aggregateId);
    }
}
